// --------------------------------------------------------------------------
// File: SocpEx1.cs
// Version 12.6.2
// --------------------------------------------------------------------------
// Licensed Materials - Property of IBM
// 5725-A06 5725-A29 5724-Y48 5724-Y49 5724-Y54 5724-Y55 5655-Y21
// Copyright IBM Corporation 2003, 2015. All Rights Reserved.
//
// US Government Users Restricted Rights - Use, duplication or
// disclosure restricted by GSA ADP Schedule Contract with
// IBM Corp.
// --------------------------------------------------------------------------
//

// SocpEx1.cs -- Solve a second order cone program to optimality, fetch
//               the dual values and test that the primal and dual solution
//               vectors returned by CPLEX satisfy the KKT conditions.
//               The problems that this code can handle are second order
//               cone programs in standard form. A second order cone
//               program in standard form is a problem of the following
//               type (c' is the transpose of c):
//                 min c1'x1 + ... + cr'xr
//                   A1 x1 + ... + Ar xr = b
//                   xi in second order cone (SOC)
//               where xi is a vector of length ni. Note that the
//               different xi are orthogonal. The constraint "xi in SOC"
//               for xi=(xi[1], ..., xi[ni]) is
//                   xi[1] >= |(xi[2],...,xi[ni])|
//               where |.| denotes the Euclidean norm. In CPLEX such a
//               constraint is formulated as
//                   -xi[1]^2 + xi[2]^2 + ... + xi[ni]^2 <= 0
//                    xi[1]                              >= 0
//                              xi[2], ..., xi[ni] free
//               Note that if ni = 1 then the second order cone constraint
//               reduces to xi[1] >= 0.
//

using ILOG.CPLEX;
using ILOG.Concert;

using System.Collections.Generic;

public sealed class SocpEx1 {
   /* Tolerance for testing KKT conditions. */
   private static readonly double TESTTOL = 1e-9;
   /* Tolerance for barrier convergence. */
   private static readonly double CONVTOL = 1e-9;

   /* A comparator to compare two instances of INumVar by name,
    */
   private class NumVarCompare : IComparer<INumVar> {
      public int Compare(INumVar a1, INumVar a2) {
         return a1.Name.CompareTo(a2.Name);
      }
   };
   private static NumVarCompare NumVarComparator = new NumVarCompare();

   /* A comparator to compare two instances of IRange by name,
    */
   private class RangeCompare : IComparer<IRange> {
      public int Compare(IRange a1, IRange a2) {
         return a1.Name.CompareTo(a2.Name);
      }
   };
   private static RangeCompare RangeComparator = new RangeCompare();

   /* Marks variables that are in cone constraints but are not the cone
    * constraint's cone head. */
   private static IRange NOT_CONE_HEAD;
   /* Marks variables that are not in any cone constraint. */
   private static IRange NOT_IN_CONE;

   // NOTE: CPLEX does not provide a function to directly get the dual
   //       multipliers for second order cone constraint.
   //       Example QCPDual.cs illustrates how the dual multipliers for a
   //       quadratic constraint can be computed from that constraint's
   //       slack.
   //       However, for SOCP we can do something simpler: we can read those
   //       multipliers directly from the dual slacks for the
   //       cone head variables. For a second order cone constraint
   //          x[1] >= |(x[2], ..., x[n])|
   //       the dual multiplier is the dual slack value for x[1].
   /// <summary>
   /// Compute dual multipliers for second order cone constraints.
   /// Returns a dictionary with a dual multiplier for each second order cone
   /// constraint.
   /// </summary>
   /// <remarks>
   ///  <c>cplex</c>  is the Cplex instance with the optimal solution.
   ///  <c>vars</c>   is a collection with <em>all</em> variables in the model.
   ///  <c>rngs</c>   is a collection with <em>all</em> constraints in the
   ///                 model.
   ///  <c>dslack</c> is a dictionary in which the function will store the full
   ///                dual slack, provided the argument is not <c>null</c>.
   /// </remarks>
   private static IDictionary<IRange,System.Double> Getsocpconstrmultipliers(Cplex cplex, ICollection<INumVar> vars, ICollection<IRange> rngs, IDictionary<INumVar,System.Double> dslack)
   {
      // Compute full dual slack.
      IDictionary<INumVar,System.Double> dense = new SortedDictionary<INumVar,System.Double>(NumVarComparator);
      foreach (INumVar v in vars) {
         dense[v] = cplex.GetReducedCost(v);
      }
      foreach (IRange r in rngs) {
         INumExpr e = r.Expr;
         if ( (e is IQuadNumExpr) && ((IQuadNumExpr)e).GetQuadEnumerator().MoveNext() ) {
            // Quadratic constraint: pick up dual slack vector
            for (ILinearNumExprEnumerator it = cplex.GetQCDSlack(r).GetLinearEnumerator(); it.MoveNext(); /* nothing */)
            {
                dense[it.NumVar] = dense[it.NumVar] + it.Value;
            }
         }
      }

      // Find the cone head variables and pick up the dual slacks for them.
      IDictionary<IRange,System.Double> socppi = new SortedDictionary<IRange,System.Double>(RangeComparator);
      foreach (IRange r in rngs) {
         INumExpr e = r.Expr;
         if ( (e is IQuadNumExpr) && ((IQuadNumExpr)e).GetQuadEnumerator().MoveNext() ) {
            // Quadratic constraint: pick up dual slack vector
            for (IQuadNumExprEnumerator it = ((IQuadNumExpr)e).GetQuadEnumerator(); it.MoveNext(); /* nothing */)
            {
                if (it.Value < 0)
                {
                   socppi[r] = dense[it.NumVar1];
                   break;
                }
            }
         }
      }

      // Fill in the dense slack if the user asked for it.
      if (dslack != null) {
         dslack.Clear();
         foreach (INumVar v in dense.Keys)
            dslack[v] = dense[v];
      }

      return socppi;
   }

   /* Test KKT conditions on the solution.
    * The function returns true if the tested KKT conditions are satisfied
    * and false otherwise.
    */
   private static bool Checkkkt (Cplex cplex,
                                 IObjective obj,
                                 ICollection<INumVar> vars,
                                 ICollection<IRange> rngs,
                                 IDictionary<INumVar,IRange> cone,
                                 double tol)
   {
      System.IO.TextWriter error = cplex.Output();
      System.IO.TextWriter output = cplex.Output();

      IDictionary<INumVar,System.Double> dslack = new SortedDictionary<INumVar,System.Double>(NumVarComparator);
      IDictionary<INumVar,System.Double> x  = new SortedDictionary<INumVar,System.Double>(NumVarComparator);
      IDictionary<IRange,System.Double> pi = new SortedDictionary<IRange,System.Double>(RangeComparator);
      IDictionary<IRange,System.Double> slack = new SortedDictionary<IRange,System.Double>(RangeComparator);

      // Read primal and dual solution information.
      foreach (INumVar v in vars) {
         x[v] = cplex.GetValue(v);
      }
      pi = Getsocpconstrmultipliers(cplex, vars, rngs, dslack);
      foreach (IRange r in rngs) {
         slack[r] = cplex.GetSlack(r);
         INumExpr e = r.Expr;
         if ( !(e is IQuadNumExpr) || !((IQuadNumExpr)e).GetQuadEnumerator().MoveNext() )
         {
            // Linear constraint: get the dual value
            pi[r] = cplex.GetDual(r);
         }
      }

      // Print out the data we just fetched.
      output.Write("x      = [");
      foreach (INumVar v in vars)
         output.Write(" {0,7:0.000}", x[v]);
      output.WriteLine(" ]");
      output.Write("dslack = [");
      foreach (INumVar v in vars)
         output.Write(" {0,7:0.000}", dslack[v]);
      output.WriteLine(" ]");
      output.Write("pi     = [");
      foreach (IRange r in rngs)
         output.Write(" {0,7:0.000}", pi[r]);
      output.WriteLine(" ]");
      output.Write("slack  = [");
      foreach (IRange r in rngs)
         output.Write(" {0,7:0.000}", slack[r]);
      output.WriteLine(" ]");

      // Test primal feasibility.
      // This example illustrates the use of dual vectors returned by CPLEX
      // to verify dual feasibility, so we do not test primal feasibility
      // here.

      // Test dual feasibility.
      // We must have
      // - for all <= constraints the respective pi value is non-negative,
      // - for all >= constraints the respective pi value is non-positive,
      // - the dslack value for all non-cone variables must be non-negative.
      // Note that we do not support ranged constraints here.
      foreach (INumVar v in vars) {
         if ( cone[v] == NOT_IN_CONE && dslack[v] < -tol ) {
            error.WriteLine("Dual multiplier for " + v + " is not feasible: " + dslack[v]);
            return false;
         }
      }
      foreach (IRange r in rngs) {
         if ( System.Math.Abs (r.LB - r.UB) <= tol ) {
            // Nothing to check for equality constraints.
         }
         else if ( r.LB > -System.Double.MaxValue && pi[r] > tol ) {
            error.WriteLine("Dual multiplier " + pi[r] + " for >= constraint");
            error.WriteLine(" " + r);
            error.WriteLine("not feasible.");
            return false;
         }
         else if ( r.UB < System.Double.MaxValue && pi[r] < -tol ) {
            error.WriteLine("Dual multiplier " + pi[r] + " for <= constraint");
            error.WriteLine(" " + r);
            error.WriteLine("not feasible.");
            return false;
         }
      }

      // Test complementary slackness.
      // For each constraint either the constraint must have zero slack or
      // the dual multiplier for the constraint must be 0. We must also
      // consider the special case in which a variable is not explicitly
      //  contained in a second order cone constraint.
      foreach (INumVar v in vars) {
         if ( cone[v] == NOT_IN_CONE ) {
            if ( System.Math.Abs(x[v]) > tol && dslack[v] > tol ) {
               error.WriteLine("Invalid complementary slackness for " + v + ":");
               error.WriteLine(" " + x[v] + " and " + dslack[v]);
               return false;
            }
         }
      }
      foreach (IRange r in rngs) {
         if ( System.Math.Abs(slack[r]) > tol && System.Math.Abs(pi[r]) > tol ) {
            error.WriteLine("Invalid complementary slackness for");
            error.WriteLine(" " + r);
            error.WriteLine(" " + slack[r] + " and " + pi[r]);
            return false;
         }
      }

      // Test stationarity.
      // We must have
      //  c - g[i]'(X)*pi[i] = 0
      // where c is the objective function, g[i] is the i-th constraint of the
      // problem, g[i]'(x) is the derivate of g[i] with respect to x and X is the
      // optimal solution.
      // We need to distinguish the following cases:
      // - linear constraints g(x) = ax - b. The derivative of such a
      //   constraint is g'(x) = a.
      // - second order constraints g(x[1],...,x[n]) = -x[1] + |(x[2],...,x[n])|
      //   the derivative of such a constraint is
      //     g'(x) = (-1, x[2]/|(x[2],...,x[n])|, ..., x[n]/|(x[2],...,x[n])|
      //   (here |.| denotes the Euclidean norm).
      // - bound constraints g(x) = -x for variables that are not explicitly
      //   contained in any second order cone constraint. The derivative for
      //   such a constraint is g'(x) = -1.
      // Note that it may happen that the derivative of a second order cone
      // constraint is not defined at the optimal solution X (this happens if
      // X=0). In this case we just skip the stationarity test.
      IDictionary<INumVar,System.Double> sum = new SortedDictionary<INumVar,System.Double>(NumVarComparator);
      foreach (INumVar v in vars)
         sum[v] = 0.0;
      for (ILinearNumExprEnumerator it = ((ILinearNumExpr)cplex.GetObjective().Expr).GetLinearEnumerator(); it.MoveNext(); /* nothing */)
         sum[it.NumVar] = it.Value;

      foreach (INumVar v in vars) {
         if ( cone[v] == NOT_IN_CONE )
            sum[v] = sum[v] - dslack[v];
      }

      foreach (IRange r in rngs) {
         INumExpr e = r.Expr;
         if ( (e is IQuadNumExpr) && ((IQuadNumExpr)e).GetQuadEnumerator().MoveNext() ) {
            double norm = 0.0;
            for (IQuadNumExprEnumerator q = ((IQuadNumExpr)e).GetQuadEnumerator(); q.MoveNext(); /* nothing */) {
               if ( q.Value > 0 )
                  norm += x[q.NumVar1] * x[q.NumVar1];
            }
            norm = System.Math.Sqrt(norm);
            if ( System.Math.Abs(norm) <= tol ) {
               cplex.Warning().WriteLine("Cannot check stationarity at non-differentiable point");
               return true;
            }
            for (IQuadNumExprEnumerator q = ((IQuadNumExpr)e).GetQuadEnumerator(); q.MoveNext(); /* nothing */) {
               INumVar v = q.NumVar1;
               if ( q.Value < 0 )
                  sum[v] = sum[v] - pi[r];
               else if ( q.Value > 0 )
                  sum[v] = sum[v] + pi[r] * x[v] / norm;
            }
         }
         else if ( e is ILinearNumExpr ) {
            for (ILinearNumExprEnumerator it = ((ILinearNumExpr)e).GetLinearEnumerator(); it.MoveNext(); /* nothing */) {
               INumVar v = it.NumVar;
               sum[v] = sum[v] - pi[r] * it.Value;
            }
         }
      }

      // Now test that all elements in sum[] are 0.
      foreach (INumVar v in vars) {
         if ( System.Math.Abs(sum[v]) > tol ) {
            error.WriteLine("Invalid stationarity " + sum[v] + " for " + v);
            return false;
         }
      }
      
      return true;   
   }

   // This function creates the following model:
   //   Minimize
   //    obj: x1 + x2 + x3 + x4 + x5 + x6
   //   Subject To
   //    c1: x1 + x2      + x5      = 8
   //    c2:           x3 + x5 + x6 = 10
   //    q1: [ -x1^2 + x2^2 + x3^2 ] <= 0
   //    q2: [ -x4^2 + x5^2 ] <= 0
   //   Bounds
   //    x2 Free
   //    x3 Free
   //    x5 Free
   //   End
   // which is a second order cone program in standard form.
   private static IObjective Createmodel(Cplex cplex,
                                         ICollection<INumVar> vars,
                                         ICollection<IRange> rngs,
                                         IDictionary<INumVar,IRange> cone) {
      System.Double pinf = System.Double.PositiveInfinity;
      System.Double ninf = System.Double.NegativeInfinity;
      INumVar x1 = cplex.NumVar(   0, pinf, "x1");
      INumVar x2 = cplex.NumVar(ninf, pinf, "x2");
      INumVar x3 = cplex.NumVar(ninf, pinf, "x3");
      INumVar x4 = cplex.NumVar(   0, pinf, "x4");
      INumVar x5 = cplex.NumVar(ninf, pinf, "x5");
      INumVar x6 = cplex.NumVar(   0, pinf, "x6");
      
      IObjective obj = cplex.AddMinimize(cplex.Sum(cplex.Sum(cplex.Sum(x1, x2),
                                                             cplex.Sum(x3, x4)),
                                                   cplex.Sum(x5, x6)),
                                         "obj");

      IRange c1 = cplex.AddEq(cplex.Sum(x1, cplex.Sum(x2, x5)), 8, "c1");
      IRange c2 = cplex.AddEq(cplex.Sum(x3, cplex.Sum(x5, x6)), 10, "c2");
      
      IRange q1 = cplex.AddLe(cplex.Sum(cplex.Prod(-1, cplex.Prod(x1, x1)),
                                        cplex.Sum(cplex.Prod(x2, x2),
                                        cplex.Prod(x3, x3))), 0, "q1");
      cone[x1] = q1;
      cone[x2] = NOT_CONE_HEAD;
      cone[x3] = NOT_CONE_HEAD;
      IRange q2 = cplex.AddLe(cplex.Sum(cplex.Prod(-1, cplex.Prod(x4, x4)),
                                        cplex.Prod(x5, x5)), 0, "q2");
      cone[x4] = q2;
      cone[x5] = NOT_CONE_HEAD;

      cone[x6] = NOT_IN_CONE;

      vars.Add(x1);
      vars.Add(x2);
      vars.Add(x3);
      vars.Add(x4);
      vars.Add(x5);
      vars.Add(x6);

      rngs.Add(c1);
      rngs.Add(c2);
      rngs.Add(q1);
      rngs.Add(q2);

      return obj;
   }

   public static void Main(string[] args) {
      IDictionary<INumVar,IRange> cone = new SortedDictionary<INumVar,IRange>(NumVarComparator);
      int retval = -1;
      Cplex cplex = null;
      try {
         cplex = new Cplex();

         // Initialize the two special (empty) marker ranges.
         NOT_CONE_HEAD = cplex.Range(0, 0);
         NOT_IN_CONE = cplex.Range(0, 0);

         // Create the model.
         IObjective obj;
         ICollection<INumVar> vars = new List<INumVar>();
         ICollection<IRange> rngs = new List<IRange>();
         obj = Createmodel(cplex, vars, rngs, cone);

         // Apply parameter settings.
         cplex.SetParam(Cplex.Param.Barrier.QCPConvergeTol, CONVTOL);

         // Solve the problem. If we cannot find an _optimal_ solution then
         // there is no point in checking the KKT conditions and we throw an
         // exception.
         if ( !cplex.Solve() || cplex.GetStatus() != Cplex.Status.Optimal )
            throw new System.SystemException("Failed to solve problem to optimality");
 
         // Test the KKT conditions on the solution.
         if ( !Checkkkt (cplex, obj, vars, rngs, cone, TESTTOL) ) {
            cplex.Output().WriteLine("Testing of KKT conditions failed.");
         }
         else {
            cplex.Output().WriteLine("KKT conditions are satisfied.");
            retval = 0;
         }
      } catch (ILOG.Concert.Exception e) {
         if ( cplex != null ) {
            cplex.Output().WriteLine("IloException: " + e);
            cplex.Output().WriteLine(e.StackTrace);
         }
         else {
            System.Console.WriteLine("IloException: " + e);
            System.Console.WriteLine(e.StackTrace);
         }
         retval = -1;
      } finally {
         if ( cplex != null )
            cplex.End();
      }
      System.Environment.Exit(retval);
   }
}
