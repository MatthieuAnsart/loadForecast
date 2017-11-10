// --------------------------------------------------------------------------
// File: QPex3.cs
// Version 12.6.2
// --------------------------------------------------------------------------
// Licensed Materials - Property of IBM
// 5725-A06 5725-A29 5724-Y48 5724-Y49 5724-Y54 5724-Y55 5655-Y21
// Copyright IBM Corporation 2001, 2015. All Rights Reserved.
//
// US Government Users Restricted Rights - Use, duplication or
// disclosure restricted by GSA ADP Schedule Contract with
// IBM Corp.
// --------------------------------------------------------------------------
//
// QPex3.cs - Entering and modifying a QP problem
//
// Example QPex3.cs illustrates how to enter and modify a QP problem 
// by using linear quadratic expressions.

using ILOG.Concert;
using ILOG.CPLEX;
using System.Collections;


public class QPex3 {
   public static void Main(string[] args) {
      try {
         // create a QP problem
         Cplex cplex = new Cplex();
         CreateQPModel(cplex);
         cplex.ExportModel("qp1ex3.lp");

         // solve the QP problem
         SolveAndPrint(cplex, "Solving the QP problem ...");

         // Modify the quadratic objective function
         ModifyQuadObjective(cplex);
         cplex.ExportModel("qp2ex3.lp");

         // solve the modified QP problem
         SolveAndPrint(cplex, "Solving the modified QP problem ...");

         cplex.End();
      }
      catch (ILOG.Concert.Exception e) {
         System.Console.WriteLine("Concert exception '" + e + "' caught");
      }
   }


   // Creating a simple QP problem
   internal static ILPMatrix CreateQPModel(IMPModeler model) {
      ILPMatrix lp = model.AddLPMatrix();

      double[]    lb = {0.0, 0.0, 0.0};
      double[]    ub = {40.0, System.Double.MaxValue, System.Double.MaxValue};
      INumVar[] x  = model.NumVarArray(model.ColumnArray(lp, 3), lb, ub);
      int nvars = x.Length;
      for (int j = 0; j < nvars; ++j)
         x[j].Name = "x" +j;

      // - x0 +   x1 + x2 <= 20
      //   x0 - 3*x1 + x2 <= 30
      double[] lhs = { -System.Double.MaxValue, -System.Double.MaxValue };
      double[] rhs = { 20.0, 30.0 };
      double[][] val = { new double[] {-1.0,  1.0,  1.0},
                         new double[] { 1.0, -3.0,  1.0} };
      int[][] ind = { new int[] {0, 1, 2},
                         new int[] {0, 1, 2} };
      lp.AddRows(lhs, rhs, ind, val);

      // minimize - x0 - x1 - x2 + x0*x0 + x1*x1 + x0*x1 + x1*x0 
      ILQNumExpr objExpr = model.LqNumExpr();
      for (int i = 0; i < nvars; ++i) {
         objExpr.AddTerm(-1.0, x[i]);
         for (int j = 0; j < nvars; ++j) {
            objExpr.AddTerm(1.0, x[i], x[j]);
         }
      }
      IObjective obj = model.Minimize(objExpr);
      model.Add(obj);

      // Print out the objective function
      PrintObjective(obj);                    

      return lp;
   }

   
   // Modifying all quadratic terms x[i]*x[j] 
   // in the objective function.
   internal static void ModifyQuadObjective(CplexModeler model) {
      IEnumerator matrixEnum = model.GetLPMatrixEnumerator();
      matrixEnum.MoveNext();
      ILPMatrix lp = (ILPMatrix)matrixEnum.Current;
      INumVar[] x = lp.NumVars;
      int ncols = x.Length;
      IObjective obj = model.GetObjective();

      // Note that the quadratic expression in the objective
      // is normalized: i.e., for all i != j, terms 
      // c(i,j)*x[i]*x[j] + c(j,i)*x[j]*x[i] are normalized as
      // (c(i,j) + c(j,i)) * x[i]*x[j], or 
      // (c(i,j) + c(j,i)) * x[j]*x[i].
      // Therefore you can only modify one of the terms 
      // x[i]*x[j] or x[j]*x[i]. 
      // If you modify both x[i]*x[j] and x[j]*x[i], then 
      // the second modification will overwrite the first one.
      for (int i = 0; i < ncols; ++i) {
         model.SetQuadCoef(obj, x[i], x[i], i*i);
         for (int j = 0; j < i; ++j)
            model.SetQuadCoef(obj, x[i], x[j], -2.0*(i*j));
      }

      // Print out the objective function
      PrintObjective(obj);     
   }


   // Print out the objective function.
   // Note that the quadratic expression in the objective
   // is normalized: i.E., for all i != j, terms 
   // c(i,j)*x[i]*x[j] + c(j,i)*x[j]*x[i] is normalized as
   // (c(i,j) + c(j,i)) * x[i]*x[j], or 
   // (c(i,j) + c(j,i)) * x[j]*x[i].
   internal static void PrintObjective(IObjective obj) {
      System.Console.WriteLine("obj: " + obj);

      // Count the number of linear terms 
      // in the objective function.
      int nlinterms = 0;
      ILinearNumExprEnumerator len = ((ILQNumExpr)obj.Expr).GetLinearEnumerator(); 
      while ( len.MoveNext() ) 
         ++nlinterms;

      // Count the number of quadratic terms 
      // in the objective function.
      int nquadterms = 0;
      int nquaddiag  = 0;
      IQuadNumExprEnumerator qen = ((ILQNumExpr)obj.Expr).GetQuadEnumerator();

      while ( qen.MoveNext() ) {
         ++nquadterms;
         INumVar var1 = qen.GetNumVar1();
         INumVar var2 = qen.GetNumVar2();
         if ( var1.Equals(var2) ) ++nquaddiag;
      }
      
      System.Console.WriteLine("number of linear terms in the objective             : " + nlinterms);
      System.Console.WriteLine("number of quadratic terms in the objective          : " + nquadterms);
      System.Console.WriteLine("number of diagonal quadratic terms in the objective : " + nquaddiag);
      System.Console.WriteLine();
   }


   // Solve the current model and print results
   internal static void SolveAndPrint(Cplex cplex, string msg) {
     System.Console.WriteLine(msg);
     if ( cplex.Solve() ) {
        System.Console.WriteLine();   
        System.Console.WriteLine("Solution status = " + cplex.GetStatus());
        System.Console.WriteLine("Solution value  = " + cplex.ObjValue);

        IEnumerator matrixEnum = cplex.GetLPMatrixEnumerator();
        matrixEnum.MoveNext();
        ILPMatrix lp = (ILPMatrix)matrixEnum.Current;
        double[] x = cplex.GetValues(lp);
        int nvars = x.Length;
        for (int j = 0; j < nvars; ++j) {
           System.Console.WriteLine("Variable " + j +": Value = " + x[j]);
        }
     }
     System.Console.WriteLine();
   }
      
}
