/* --------------------------------------------------------------------------
 * File: QCPDual.cs
 * Version 12.6.2
 * --------------------------------------------------------------------------
 * Licensed Materials - Property of IBM
 * 5725-A06 5725-A29 5724-Y48 5724-Y49 5724-Y54 5724-Y55 5655-Y21
 * Copyright IBM Corporation 2003, 2015. All Rights Reserved.
 *
 * US Government Users Restricted Rights - Use, duplication or
 * disclosure restricted by GSA ADP Schedule Contract with
 * IBM Corp.
 * --------------------------------------------------------------------------
 */

/*
 * QCPDual.cs - Illustrates how to query and analyze dual values of QCPs
 */
using ILOG.CPLEX;
using ILOG.Concert;

using System.Collections.Generic;

public sealed class QCPDual
{
    /// <summary>
    /// Tolerance applied when testing values for zero.
    /// </summary>
    public static readonly double ZEROTOL = 1e-6;

    /// Create a string representation <c>array<c>.
    private static string arrayToString(double[] array)
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("[");
        for (int i = 0; i < array.Length; ++i)
        {
            result.Append(string.Format(" {0,7:0.000}", array[i]));
        }
        result.Append(" ]");
        return result.ToString();
    }

    /* ***************************************************************** *
     *                                                                   *
     *    C A L C U L A T E   D U A L S   F O R   Q U A D S              *
     *                                                                   *
     *   CPLEX does not give us the dual multipliers for quadratic       *
     *   constraints directly. This is because they may not be properly  *
     *   defined at the cone top and deciding whether we are at the cone *
     *   top or not involves (problem specific) tolerance issues. CPLEX  *
     *   instead gives us all the values we need in order to compute the *
     *   dual multipliers if we are not at the cone top.                 *
     *                                                                   *
     * ***************************************************************** */
    /// <summary>
    /// Calculate dual multipliers for quadratic constraints from dual
    /// slack vectors and optimal solutions.
    /// The dual multiplier is essentially the dual slack divided
    /// by the derivative evaluated at the optimal solution. If the optimal
    /// solution is 0 then the derivative at this point is not defined (we are
    /// at the cone top) and we cannot compute a dual multiplier.
    /// </summary>
    /// <remarks>
    ///   <c>cplex</c> is the Cplex instance that holds the optimal
    ///                solution.
    ///   <c>xval</c>  is the optimal solution vector.
    ///   <c>tol</c>   is the tolerance used to decide whether we are at
    ///                the cone
    ///   <c>x</c>     is the array of variables in the model.
    ///   <c>qs</c>    is the array of quadratic constraints for which duals
    ///                shall be calculated.
    /// </remarks>
    private static double[] getqconstrmultipliers(Cplex cplex, double[] xval, double tol, INumVar[] x, IRange[] qs) {
        double[] qpi = new double[qs.Length];
        // Store solution in map so that we can look up by variable.
        IDictionary<INumVar, System.Double> sol = new Dictionary<INumVar, System.Double>();
        for (int j = 0; j < x.Length; ++j)
        {
            sol.Add(x[j], xval[j]);
        }

        for (int i = 0; i < qs.Length; ++i)
        {
            IDictionary<INumVar, System.Double> dslack = new Dictionary<INumVar, System.Double>();
            for (ILinearNumExprEnumerator it = cplex.GetQCDSlack(qs[i]).GetLinearEnumerator(); it.MoveNext(); /* nothing */)
            {
                dslack[it.NumVar] = it.Value;
            }

            IDictionary<INumVar, System.Double> grad = new Dictionary<INumVar, System.Double>();
            bool conetop = true;
            for (IQuadNumExprEnumerator it = ((ILQNumExpr)qs[i].Expr).GetQuadEnumerator();
                 it.MoveNext(); /* nothing */)
            {
                INumVar x1 = it.NumVar1;
                INumVar x2 = it.NumVar2;
                if (sol[x1] > tol || sol[x2] > tol)
                    conetop = false;
                System.Double old;
                if (!grad.TryGetValue(x1, out old)) { old = 0.0; }
                grad[x1] = old + sol[x2] * it.Value;
                if (!grad.TryGetValue(x2, out old)) { old = 0.0; }
                grad[x2] = old + sol[x1] * it.Value;
            }
            if (conetop)
            {
                throw new System.SystemException("Cannot compute dual multiplier at cone top!");
            }

            // Compute qpi[i] as slack/gradient.
            // We may have several indices to choose from and use the one
            // with largest absolute value in the denominator.
            bool ok = false;
            double maxabs = -1.0;
            foreach (INumVar v in x)
            {
                System.Double g;
                if (grad.TryGetValue(v, out g))
                {
                    if (System.Math.Abs(g) > tol)
                    {
                        if (System.Math.Abs(g) > maxabs)
                        {
                            System.Double d;
                            qpi[i] = (dslack.TryGetValue(v, out d) ? d : 0.0) / g;
                            maxabs = System.Math.Abs(g);
                        }
                        ok = true;
                    }
                }
            }
            if (!ok)
            {
                // Dual slack is all 0. qpi[i] can be anything, just set to 0.
                qpi[i] = 0;
            }
        }
        return qpi;
    }

    /// <summary>
    /// The example's main function.
    /// </summary>
    public static void Main(string[] args)
    {
        int retval = 0;
        Cplex cplex = null;
        try
        {
            cplex = new Cplex();

            /* ***************************************************************** *
             *                                                                   *
             *    S E T U P   P R O B L E M                                      *
             *                                                                   *
             *  We create the following problem:                                 *
             * Minimize                                                          *
             *  obj: 3x1 - x2 + 3x3 + 2x4 + x5 + 2x6 + 4x7                       *
             * Subject To                                                        *
             *  c1: x1 + x2 = 4                                                  *
             *  c2: x1 + x3 >= 3                                                 *
             *  c3: x6 + x7 <= 5                                                 *
             *  c4: -x1 + x7 >= -2                                               *
             *  q1: [ -x1^2 + x2^2 ] <= 0                                        *
             *  q2: [ 4.25x3^2 -2x3*x4 + 4.25x4^2 - 2x4*x5 + 4x5^2  ] + 2 x1 <= 9.0
             *  q3: [ x6^2 - x7^2 ] >= 4                                         *
             * Bounds                                                            *
             *  0 <= x1 <= 3                                                     *
             *  x2 Free                                                          *
             *  0 <= x3 <= 0.5                                                   *
             *  x4 Free                                                          *
             *  x5 Free                                                          *
             *  x7 Free                                                          *
             * End                                                               *
             *                                                                   *
             * ***************************************************************** */

            INumVar[] x = new INumVar[7];
            x[0] = cplex.NumVar(0, 3, "x1");
            x[1] = cplex.NumVar(System.Double.NegativeInfinity, System.Double.PositiveInfinity, "x2");
            x[2] = cplex.NumVar(0, 0.5, "x3");
            x[3] = cplex.NumVar(System.Double.NegativeInfinity, System.Double.PositiveInfinity, "x4");
            x[4] = cplex.NumVar(System.Double.NegativeInfinity, System.Double.PositiveInfinity, "x5");
            x[5] = cplex.NumVar(0, System.Double.PositiveInfinity, "x6");
            x[6] = cplex.NumVar(System.Double.NegativeInfinity, System.Double.PositiveInfinity, "x7");

            IRange[] linear = new IRange[4];
            linear[0] = cplex.AddEq(cplex.Sum(x[0], x[1]), 4.0, "c1");
            linear[1] = cplex.AddGe(cplex.Sum(x[0], x[2]), 3.0, "c2");
            linear[2] = cplex.AddLe(cplex.Sum(x[5], x[6]), 5.0, "c3");
            linear[3] = cplex.AddGe(cplex.Diff(x[6], x[0]), -2.0, "c4");

            IRange[] quad = new IRange[3];
            quad[0] = cplex.AddLe(cplex.Sum(cplex.Prod(-1, x[0], x[0]),
                                            cplex.Prod(x[1], x[1])), 0.0, "q1");
            quad[1] = cplex.AddLe(cplex.Sum(cplex.Prod(4.25, x[2], x[2]),
                cplex.Prod(-2,x[2],x[3]),
                cplex.Prod(4.25,x[3],x[3]),
                cplex.Prod(-2,x[3],x[4]),
                cplex.Prod(4,x[4],x[4]),
                cplex.Prod(2,x[0])), 9.0, "q2");
            quad[2] = cplex.AddGe(cplex.Sum(cplex.Prod(x[5], x[5]),
                                            cplex.Prod(-1, x[6], x[6])), 4.0, "q3");

            cplex.AddMinimize(cplex.Sum(cplex.Prod(3.0, x[0]),
                                        cplex.Prod(-1.0, x[1]),
                                        cplex.Prod(3.0, x[2]),
                                        cplex.Prod(2.0, x[3]),
                                        cplex.Prod(1.0, x[4]),
                                        cplex.Prod(2.0, x[5]),
                                        cplex.Prod(4.0, x[6])), "obj");

            /* ***************************************************************** *
             *                                                                   *
             *    O P T I M I Z E   P R O B L E M                                *
             *                                                                   *
             * ***************************************************************** */
            cplex.SetParam(Cplex.Param.Barrier.QCPConvergeTol, 1e-10);
            cplex.Solve();

            /* ***************************************************************** *
             *                                                                   *
             *    Q U E R Y   S O L U T I O N                                    *
             *                                                                   *
             * ***************************************************************** */
            double[] xval = cplex.GetValues(x);
            double[] slack = cplex.GetSlacks(linear);
            double[] qslack = cplex.GetSlacks(quad);
            double[] cpi = cplex.GetReducedCosts(x);
            double[] rpi = cplex.GetDuals(linear);
            double[] qpi = getqconstrmultipliers(cplex, xval, ZEROTOL, x, quad);
            // Also store solution in a dictionary so that we can look up
            // values by variable and not only by index.
            IDictionary<INumVar, System.Double> xmap = new Dictionary<INumVar, System.Double>();
            for (int j = 0; j < x.Length; ++j)
            {
                xmap.Add(x[j], xval[j]);
            }

            /* ***************************************************************** *
             *                                                                   *
             *    C H E C K   K K T   C O N D I T I O N S                        *
             *                                                                   *
             *    Here we verify that the optimal solution computed by CPLEX     *
             *    (and the qpi[] values computed above) satisfy the KKT          *
             *    conditions.                                                    *
             *                                                                   *
             * ***************************************************************** */

            // Primal feasibility: This example is about duals so we skip this test.

            // Dual feasibility: We must verify
            // - for <= constraints (linear or quadratic) the dual
            //   multiplier is non-positive.
            // - for >= constraints (linear or quadratic) the dual
            //   multiplier is non-negative.
            for (int i = 0; i < linear.Length; ++i)
            {
                if (linear[i].LB <= System.Double.NegativeInfinity)
                {
                    // <= constraint
                    if (rpi[i] > ZEROTOL)
                        throw new System.SystemException("Dual feasibility test failed for row " + linear[i]
                                                   + ": " + rpi[i]);
                }
                else if (linear[i].UB >= System.Double.PositiveInfinity)
                {
                    // >= constraint
                    if (rpi[i] < -ZEROTOL)
                        throw new System.SystemException("Dual feasibility test failed for row " + linear[i]
                                                   + ": " + rpi[i]);
                }
                else
                {
                    // nothing to do for equality constraints
                }
            }
            for (int i = 0; i < quad.Length; ++i)
            {
                if (quad[i].LB <= System.Double.NegativeInfinity)
                {
                    // <= constraint
                    if (qpi[i] > ZEROTOL)
                        throw new System.SystemException("Dual feasibility test failed for quad " + quad[i]
                                                   + ": " + qpi[i]);
                }
                else if (quad[i].UB >= System.Double.PositiveInfinity)
                {
                    // >= constraint
                    if (qpi[i] < -ZEROTOL)
                        throw new System.SystemException("Dual feasibility test failed for quad " + quad[i]
                                                   + ": " + qpi[i]);
                }
                else
                {
                    // nothing to do for equality constraints
                }
            }

            // Complementary slackness.
            // For any constraint the product of primal slack and dual multiplier
            // must be 0.
            for (int i = 0; i < linear.Length; ++i)
            {
                if (System.Math.Abs(linear[i].UB - linear[i].LB) > ZEROTOL &&
                     System.Math.Abs(slack[i] * rpi[i]) > ZEROTOL)
                    throw new System.SystemException("Complementary slackness test failed for row " + linear[i]
                                               + ": " + System.Math.Abs(slack[i] * rpi[i]));
            }
            for (int i = 0; i < quad.Length; ++i)
            {
                if (System.Math.Abs(quad[i].UB - quad[i].LB) > ZEROTOL &&
                     System.Math.Abs(qslack[i] * qpi[i]) > ZEROTOL)
                    throw new System.SystemException("Complementary slackness test failed for quad " + quad[i]
                                               + ": " + System.Math.Abs(qslack[i] * qpi[i]));
            }
            for (int j = 0; j < x.Length; ++j)
            {
                if (x[j].UB < System.Double.PositiveInfinity)
                {
                    double slk = x[j].UB - xval[j];
                    double dual = cpi[j] < -ZEROTOL ? cpi[j] : 0.0;
                    if (System.Math.Abs(slk * dual) > ZEROTOL)
                        throw new System.SystemException("Complementary slackness test failed for column " + x[j]
                                                   + ": " + System.Math.Abs(slk * dual));
                }
                if (x[j].LB > System.Double.NegativeInfinity)
                {
                    double slk = xval[j] - x[j].LB;
                    double dual = cpi[j] > ZEROTOL ? cpi[j] : 0.0;
                    if (System.Math.Abs(slk * dual) > ZEROTOL)
                        throw new System.SystemException("Complementary slackness test failed for column " + x[j]
                                                   + ": " + System.Math.Abs(slk * dual));
                }
            }

            // Stationarity.
            // The difference between objective function and gradient at optimal
            // solution multiplied by dual multipliers must be 0, i.E., for the
            // optimal solution x
            // 0 == c
            //      - sum(r in rows)  r'(x)*rpi[r]
            //      - sum(q in quads) q'(x)*qpi[q]
            //      - sum(c in cols)  b'(x)*cpi[c]
            // where r' and q' are the derivatives of a row or quadratic constraint,
            // x is the optimal solution and rpi[r] and qpi[q] are the dual
            // multipliers for row r and quadratic constraint q.
            // b' is the derivative of a bound constraint and cpi[c] the dual bound
            // multiplier for column c.
            IDictionary<INumVar, System.Double> kktsum = new Dictionary<INumVar, System.Double>();
            for (int j = 0; j < x.Length; ++j)
            {
                kktsum.Add(x[j], 0.0);
            }

            // Objective function.
            for (ILinearNumExprEnumerator it = ((ILinearNumExpr)cplex.GetObjective().Expr).GetLinearEnumerator();
                 it.MoveNext(); /* nothing */)
            {
                kktsum[it.NumVar] = it.Value;
            }

            // Linear constraints.
            // The derivative of a linear constraint ax - b (<)= 0 is just a.
            for (int i = 0; i < linear.Length; ++i)
            {
                for (ILinearNumExprEnumerator it = ((ILinearNumExpr)linear[i].Expr).GetLinearEnumerator();
                     it.MoveNext(); /* nothing */)
                {
                    kktsum[it.NumVar] = kktsum[it.NumVar] - rpi[i] * it.Value;
                }
            }

            // Quadratic constraints.
            // The derivative of a constraint xQx + ax - b <= 0 is
            // Qx + Q'x + a.
            for (int i = 0; i < quad.Length; ++i)
            {
                for (ILinearNumExprEnumerator it = ((ILinearNumExpr)quad[i].Expr).GetLinearEnumerator();
                     it.MoveNext(); /* nothing */)
                {
                    kktsum[it.NumVar] = kktsum[it.NumVar] - qpi[i] * it.Value;
                }
                for (IQuadNumExprEnumerator it = ((IQuadNumExpr)quad[i].Expr).GetQuadEnumerator();
                     it.MoveNext(); /* nothing */)
                {
                    INumVar v1 = it.NumVar1;
                    INumVar v2 = it.NumVar2;
                    kktsum[v1] = kktsum[v1] - qpi[i] * xmap[v2] * it.Value;
                    kktsum[v2] = kktsum[v2] - qpi[i] * xmap[v1] * it.Value;
                }
            }

            // Bounds.
            // The derivative for lower bounds is -1 and that for upper bounds
            // is 1.
            // CPLEX already returns dj with the appropriate sign so there is
            // no need to distinguish between different bound types here.
            for (int j = 0; j < x.Length; ++j)
            {
                kktsum[x[j]] = kktsum[x[j]] - cpi[j];
            }

            foreach (INumVar v in x)
            {
                if (System.Math.Abs(kktsum[v]) > ZEROTOL)
                    throw new System.SystemException("Stationarity test failed at " + v
                        + ": " + System.Math.Abs(kktsum[v]));
            }

            // KKT conditions satisfied. Dump out the optimal solutions and
            // the dual values.
            System.Console.WriteLine("Optimal solution satisfies KKT conditions.");
            System.Console.WriteLine("   x[] = " + arrayToString(xval));
            System.Console.WriteLine(" cpi[] = " + arrayToString(cpi));
            System.Console.WriteLine(" rpi[] = " + arrayToString(rpi));
            System.Console.WriteLine(" qpi[] = " + arrayToString(qpi));
        }
        catch (ILOG.Concert.Exception e)
        {
            System.Console.WriteLine("IloException: " + e.Message);
            System.Console.WriteLine(e.StackTrace);
            retval = -1;
        }
        finally
        {
            if (cplex != null)
                cplex.End();
        }
        System.Environment.Exit(retval);
    }
}
