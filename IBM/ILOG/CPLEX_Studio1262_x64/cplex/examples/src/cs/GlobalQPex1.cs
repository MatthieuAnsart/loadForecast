// --------------------------------------------------------------------------
// File: GlobalQPex1.cs
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
// GlobalQPex1.cs - Reading in and optimizing a convex or nonconvex (mixed-integer) QP
// with convex, first order or global optimizer.
//
// To run this example, command line arguments are required.
// That is:   GlobalQPex1  filename solutiontarget
// where 
//     filename is the name of the file, with .mps, .lp, or .sav extension
//     solutiontarget   is the optimality target
//                 c          for a convex qp miqp
//                 f          for a first order solution (only qp)
//                 g          for the global optimum
//
// Example:
//     GlobalQPex1  nonconvexqp.lp g

using ILOG.Concert;
using ILOG.CPLEX;
using System.Collections;


public class GlobalQPex1 {
   internal static void Usage() {
      System.Console.WriteLine("usage:  GlobalQPex1 <filename> <optimality target>");
      System.Console.WriteLine("          c       convex QP or MIQP");
      System.Console.WriteLine("          f       first order solution (only QP)");
      System.Console.WriteLine("          g       global optimum");
   }

   public static void Main(string[] args) {
      if ( args.Length != 2 ) {
         Usage();
         return;
      }
      bool ismip = false;
      try {
         Cplex cplex = new Cplex();

         // Evaluate command line option and set optimization method accordingly.
         switch ( args[1].ToCharArray()[0] ) {
         case 'c': cplex.SetParam(Cplex.Param.OptimalityTarget,
                                  Cplex.OptimalityTarget.OptimalConvex);
                   break;
         case 'f': cplex.SetParam(Cplex.Param.OptimalityTarget,
                                  Cplex.OptimalityTarget.FirstOrder);
                   break;
         case 'g': cplex.SetParam(Cplex.Param.OptimalityTarget,
                                  Cplex.OptimalityTarget.OptimalGlobal);
                   break;
         default:  Usage();
                   return;
         }

         cplex.ImportModel(args[0]);

         ismip = cplex.IsMIP();

         if ( cplex.Solve() ) {
            System.Console.WriteLine("Solution status = " + cplex.GetStatus());
            System.Console.WriteLine("Solution value  = " + cplex.ObjValue);

            IEnumerator matrixEnum = cplex.GetLPMatrixEnumerator();
            matrixEnum.MoveNext();

            ILPMatrix lp = (ILPMatrix)matrixEnum.Current;

            double[] x = cplex.GetValues(lp);
            for (int j = 0; j < x.Length; ++j) {
               System.Console.WriteLine("Variable " + j + ": Value = " + x[j]);
            }
         }
         cplex.End();
      }
      catch (ILOG.CPLEX.CplexModeler.Exception e) {
         if ( args[1].ToCharArray()[0] == 'c' &&
              e.GetStatus() == 5002             ) {
            /* Status 5002 is CPXERR_Q_NOT_POS_DEF */
           if (ismip) {
              System.Console.WriteLine("Problem is not convex. Use argument g to get global optimum.");
           }
           else {
              System.Console.Write("Problem is not convex. Use argument f to get local optimum ");
              System.Console.WriteLine("or g to get global optimum.");
           }
         }
         else if ( args[1].ToCharArray()[0] == 'f'&&
              e.GetStatus() == 1017               &&
              ismip                                 ) {
            /* Status 1017 is CPXERR_NOT_FOR_MIP */
            System.Console.Write("Problem is a MIP, cannot compute local optima satifying ");
            System.Console.WriteLine("the first order KKT.");
            System.Console.WriteLine("Use argument g to get the global optimum.");
         }
         else  {
            System.Console.WriteLine("Cplex exception '" + e + "' caught");
         }
      }
      catch (ILOG.Concert.Exception e) {
         System.Console.WriteLine("Concert exception caught: " + e);
      }
   }
}
