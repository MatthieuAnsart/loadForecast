// --------------------------------------------------------------------------
// File: BendersATSP.cs
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
// Example BendersATSP.cs solves a flow MILP model for an
// Asymmetric Traveling Salesman Problem (ATSP) instance
// through Benders decomposition.
// 
// The arc costs of an ATSP instance are read from an input file.
// The flow MILP model is decomposed into a master ILP and a worker LP.
// 
// The master ILP is then solved by adding Benders' cuts during
// the branch-and-cut process via the cut callback classes 
// Cplex.LazyConstraintCallback and Cplex.UserCutCallback.
// The cut callbacks add to the master ILP violated Benders' cuts
// that are found by solving the worker LP.
// 
// The example allows the user to decide if Benders' cuts have to be separated:
// 
// a) Only to separate integer infeasible solutions.
// In this case, Benders' cuts are treated as lazy constraints through the
// class Cplex.LazyConstraintCallback.
// 
// b) Also to separate fractional infeasible solutions.
// In this case, Benders' cuts are treated as lazy constraints through the
// class Cplex.LazyConstraintCallback.
// In addition, Benders' cuts are also treated as user cuts through the
// class Cplex.UserCutCallback.
// 
// 
// To run this example, command line arguments are required:
//     BendersATSP {0|1} [filename]
// where
//     0         Indicates that Benders' cuts are only used as lazy constraints,
//               to separate integer infeasible solutions.
//     1         Indicates that Benders' cuts are also used as user cuts,
//               to separate fractional infeasible solutions.
// 
//     filename  Is the name of the file containing the ATSP instance (arc costs).
//               If filename is not specified, the instance
//               ../../../../examples/data/atsp.dat is read
// 
// 
// ATSP instance defined on a directed graph G = (V, A)
// - V = {0, ..., n-1}, V0 = V \ {0}
// - A = {(i,j) : i in V, j in V, i != j }
// - forall i in V: delta+(i) = {(i,j) in A : j in V}
// - forall i in V: delta-(i) = {(j,i) in A : j in V}
// - c(i,j) = traveling cost associated with (i,j) in A
// 
// Flow MILP model
// 
// Modeling variables:
// forall (i,j) in A:
//    x(i,j) = 1, if arc (i,j) is selected
//           = 0, otherwise
// forall k in V0, forall (i,j) in A:
//    y(k,i,j) = flow of the commodity k through arc (i,j)
// 
// Objective:
// minimize sum((i,j) in A) c(i,j) * x(i,j)
// 
// Degree constraints:
// forall i in V: sum((i,j) in delta+(i)) x(i,j) = 1
// forall i in V: sum((j,i) in delta-(i)) x(j,i) = 1
// 
// Binary constraints on arc variables:
// forall (i,j) in A: x(i,j) in {0, 1}
// 
// Flow constraints:
// forall k in V0, forall i in V:
//    sum((i,j) in delta+(i)) y(k,i,j) - sum((j,i) in delta-(i)) y(k,j,i) = q(k,i)
//    where q(k,i) =  1, if i = 0
//                 = -1, if k == i
//                 =  0, otherwise
// 
// Capacity constraints:
// forall k in V0, for all (i,j) in A: y(k,i,j) <= x(i,j)
// 
// Nonnegativity of flow variables:
// forall k in V0, for all (i,j) in A: y(k,i,j) >= 0
// 

using ILOG.Concert;
using ILOG.CPLEX;


public class BendersATSP
{

   // The class BendersLazyConsCallback 
   // allows to add Benders' cuts as lazy constraints.
   //
   public class BendersLazyConsCallback : Cplex.LazyConstraintCallback
   {
      internal IIntVar[][] x;
      internal WorkerLP workerLP;
      internal int numNodes;

      internal BendersLazyConsCallback(IIntVar[][] x, WorkerLP workerLP)
      {
         this.x = x;
         this.workerLP = workerLP;
         numNodes = x.Length;
      }

      public override void Main()
      {
         // Get the current x solution

         double[][] sol = new double[numNodes][];
         for (int i = 0; i < numNodes; ++i)
            sol[i] = GetValues(x[i]);

         // Benders' cut separation

         IRange cut = workerLP.Separate(sol, x);
         if ( cut != null ) Add(cut);
      }

   } // END BendersLazyConsCallback 


   // The class BendersUserCutCallback 
   // allows to add Benders' cuts as user cuts.
   //
   public class BendersUserCutCallback : Cplex.UserCutCallback
   {
      internal IIntVar[][] x;
      internal WorkerLP workerLP;
      internal int numNodes;

      internal BendersUserCutCallback(IIntVar[][] x, WorkerLP workerLP)
      {
         this.x = x;
         this.workerLP = workerLP;
         numNodes = x.Length;
      }

      public override void Main()
      {
         // Skip the separation if not at the end of the cut loop
       
         if ( !IsAfterCutLoop() )
            return;

         // Get the current x solution

         double[][] sol = new double[numNodes][];
         for (int i = 0; i < numNodes; ++i)
            sol[i] = GetValues(x[i]);

         // Benders' cut separation

         IRange cut = workerLP.Separate(sol, x);
         if ( cut != null ) Add(cut);
      }

   } // END BendersUserCutCallback


   // Data class to read an ATSP instance from an input file
   //
   internal class Data
   {
      internal int numNodes;
      internal double[][] arcCost;

      internal Data(string fileName)
      {
         InputDataReader reader = new InputDataReader(fileName);
         arcCost = reader.ReadDoubleArrayArray();
         numNodes = arcCost.Length;
         for (int i = 0; i < numNodes; ++i)
         {
            if ( arcCost[i].Length != numNodes )
               throw new ILOG.Concert.Exception("inconsistent data in file " + fileName);
            arcCost[i][i] = 0.0;
         }
      }
   } // END Data


   // This class builds the worker LP (i.e., the dual of flow constraints and
   // capacity constraints of the flow MILP) and allows to separate violated
   // Benders' cuts.
   //
   internal class WorkerLP
   {
      internal Cplex cplex;
      internal int numNodes;
      internal INumVar[][][] v;
      internal INumVar[][] u;

      // The constructor sets up the Cplex instance to solve the worker LP, 
      // and creates the worker LP (i.e., the dual of flow constraints and
      // capacity constraints of the flow MILP)
      //
      // Modeling variables:
      // forall k in V0, i in V:
      //    u(k,i) = dual variable associated with flow constraint (k,i)
      //
      // forall k in V0, forall (i,j) in A:
      //    v(k,i,j) = dual variable associated with capacity constraint (k,i,j)
      //
      // Objective:
      // minimize sum(k in V0) sum((i,j) in A) x(i,j) * v(k,i,j)
      //          - sum(k in V0) u(k,0) + sum(k in V0) u(k,k)
      //
      // Constraints:
      // forall k in V0, forall (i,j) in A: u(k,i) - u(k,j) <= v(k,i,j)
      //
      // Nonnegativity on variables v(k,i,j)
      // forall k in V0, forall (i,j) in A: v(k,i,j) >= 0
      //
      internal WorkerLP(int numNodes)
      {
         this.numNodes = numNodes;
         int i, j, k;

         // Set up Cplex instance to solve the worker LP

         cplex = new Cplex();
         cplex.SetOut(null);

         // Turn off the presolve reductions and set the CPLEX optimizer
         // to solve the worker LP with primal simplex method.

         cplex.SetParam(Cplex.Param.Preprocessing.Reduce, 0);
         cplex.SetParam(Cplex.Param.RootAlgorithm, Cplex.Algorithm.Primal);

         // Create variables v(k,i,j) forall k in V0, (i,j) in A
         // For simplicity, also dummy variables v(k,i,i) are created.
         // Those variables are fixed to 0 and do not partecipate to 
         // the constraints.

         v = new INumVar[numNodes-1][][];
         for (k = 1; k < numNodes; ++k)
         {
            v[k-1] = new INumVar[numNodes][];
            for (i = 0; i < numNodes; ++i)
            {
               v[k-1][i] = new INumVar[numNodes];
               for (j = 0; j < numNodes; ++j)
               {
                  v[k-1][i][j] = cplex.NumVar(0.0, System.Double.MaxValue, "v." + k + "." + i + "." + j);
                  cplex.Add(v[k-1][i][j]); 
               }
               v[k-1][i][i].UB = 0.0; 
            }
         }

         // Create variables u(k,i) forall k in V0, i in V

         u = new INumVar[numNodes-1][]; 
         for (k = 1; k < numNodes; ++k)
         {
            u[k-1] = new INumVar[numNodes];
            for (i = 0; i < numNodes; ++i)
            {
               u[k-1][i] = cplex.NumVar(-System.Double.MaxValue, System.Double.MaxValue, "u." + k + "." + i);
               cplex.Add(u[k-1][i]);
            }
         }

         // Initial objective function is empty

         cplex.AddMinimize();

         // Add constraints:
         // forall k in V0, forall (i,j) in A: u(k,i) - u(k,j) <= v(k,i,j)

         for (k = 1; k < numNodes; ++k)
         {
            for (i = 0; i < numNodes; ++i)
            {
               for (j = 0; j < numNodes; ++j)
               {
                  if ( i != j )
                  {
                     ILinearNumExpr expr = cplex.LinearNumExpr();
                     expr.AddTerm(v[k-1][i][j], -1.0);
                     expr.AddTerm(u[k-1][i], 1.0);
                     expr.AddTerm(u[k-1][j], -1.0); 
                     cplex.AddLe(expr, 0.0);
                  }
               }
            }
         }

      } // END WorkerLP

      public void End() { cplex.End(); }

      // This method separates Benders' cuts violated by the current x solution.
      // Violated cuts are found by solving the worker LP
      //
      public IRange Separate(double[][] xSol, IIntVar[][] x)
      {

         int i, j, k;

         IRange cut = null;

         // Update the objective function in the worker LP:
         // minimize sum(k in V0) sum((i,j) in A) x(i,j) * v(k,i,j)
         //          - sum(k in V0) u(k,0) + sum(k in V0) u(k,k)

         ILinearNumExpr objExpr = cplex.LinearNumExpr();
         for (k = 1; k < numNodes; ++k)
         {
            for (i = 0; i < numNodes; ++i)
            {
               for (j = 0; j < numNodes; ++j)
               {
                  objExpr.AddTerm(v[k-1][i][j], xSol[i][j]);
               }
            }
         }
         for (k = 1; k < numNodes; ++k)
         {
            objExpr.AddTerm(u[k-1][k], 1.0);
            objExpr.AddTerm(u[k-1][0], -1.0);
         }
         cplex.GetObjective().Expr = objExpr;
         
         // Solve the worker LP

         cplex.Solve();

         // A violated cut is available iff the solution status is Unbounded 

         if ( cplex.GetStatus().Equals(Cplex.Status.Unbounded) )
         {

            // Get the violated cut as an unbounded ray of the worker LP

            ILinearNumExpr rayExpr = cplex.Ray; 

            // Compute the cut from the unbounded ray. The cut is:
            // sum((i,j) in A) (sum(k in V0) v(k,i,j)) * x(i,j) >=
            // sum(k in V0) u(k,0) - u(k,k)

            ILinearNumExpr cutLhs = cplex.LinearNumExpr();
            double cutRhs = 0.0;
            ILinearNumExprEnumerator rayEnum = rayExpr.GetLinearEnumerator(); 

            while ( rayEnum.MoveNext() ) {
               INumVar var = rayEnum.NumVar;
               bool varFound = false;
               for (k = 1; k < numNodes && !varFound; ++k) {
                  for (i = 0; i < numNodes && !varFound; ++i) {
                     for (j = 0; j < numNodes && !varFound; ++j) {
                        if ( var.Equals(v[k-1][i][j]) ) {
                           cutLhs.AddTerm(x[i][j], rayEnum.Value);
                           varFound = true;
                        }
                     }
                  }
               }
               for (k = 1; k < numNodes && !varFound; ++k) {
                  for (i = 0; i < numNodes && !varFound; ++i) {
                     if ( var.Equals(u[k-1][i]) ) {
                        if ( i == 0 )
                           cutRhs += rayEnum.Value;
                        else if ( i == k )
                           cutRhs -= rayEnum.Value;
                         varFound = true;
                     }
                  }
               }
            }

            cut = cplex.Ge(cutLhs, cutRhs); 
         }

         return cut;
      } // END Separate

   } // END WorkerLP

   // This method creates the master ILP (arc variables x and degree constraints).
   //
   // Modeling variables:
   // forall (i,j) in A:
   //    x(i,j) = 1, if arc (i,j) is selected
   //           = 0, otherwise
   //
   // Objective:
   // minimize sum((i,j) in A) c(i,j) * x(i,j)
   //
   // Degree constraints:
   // forall i in V: sum((i,j) in delta+(i)) x(i,j) = 1
   // forall i in V: sum((j,i) in delta-(i)) x(j,i) = 1
   //
   // Binary constraints on arc variables:
   // forall (i,j) in A: x(i,j) in {0, 1}
   //
   internal static void CreateMasterILP(IModeler model,
                                        Data data,
                                        IIntVar[][] x)
   {
      int i, j;
      int numNodes = data.numNodes;

      // Create variables x(i,j) for (i,j) in A 
      // For simplicity, also dummy variables x(i,i) are created.
      // Those variables are fixed to 0 and do not partecipate to 
      // the constraints.

      for (i = 0; i < numNodes; ++i)
      {
         x[i] = new IIntVar[numNodes]; 
         for (j = 0; j < numNodes; ++j)
         {
            x[i][j] = model.BoolVar("x." + i + "." + j);
            model.Add(x[i][j]);
         }
         x[i][i].UB = 0;
      }

      // Create objective function: minimize sum((i,j) in A ) c(i,j) * x(i,j)

      ILinearNumExpr objExpr = model.LinearNumExpr();
      for (i = 0; i < numNodes; ++i)
         objExpr.Add(model.ScalProd(x[i], data.arcCost[i]));
      model.AddMinimize(objExpr);

      // Add the out degree constraints.
      // forall i in V: sum((i,j) in delta+(i)) x(i,j) = 1

      for (i = 0; i < numNodes; ++i)
      {
         ILinearNumExpr expr = model.LinearNumExpr();
         for (j = 0; j < i; ++j) expr.AddTerm(x[i][j], 1.0);
         for (j = i + 1; j < numNodes; ++j) expr.AddTerm(x[i][j], 1.0);
         model.AddEq(expr, 1.0);
      }

      // Add the in degree constraints.
      // forall i in V: sum((j,i) in delta-(i)) x(j,i) = 1

      for (i = 0; i < numNodes; ++i)
      {
         ILinearNumExpr expr = model.LinearNumExpr();
         for (j = 0; j < i; ++j) expr.AddTerm(x[j][i], 1.0);
         for (j = i + 1; j < numNodes; ++j) expr.AddTerm(x[j][i], 1.0);
         model.AddEq(expr, 1.0);
      }

   } // END CreateMasterILP

   public static void Main(string[] args)
   {
      try
      {
         string fileName = "../../../../examples/data/atsp.dat";

         // Check the command line arguments 

         if ( args.Length != 1 && args.Length != 2 )
         {
            Usage();
            return;
         }

         if ( ! (args[0].Equals("0") || args[0].Equals("1")) ) 
         {
            Usage();
            return;
         }

         bool sepFracSols = (args[0].ToCharArray()[0] == '0' ? false : true);

         if ( sepFracSols )
         {
            System.Console.WriteLine("Benders' cuts separated to cut off: " +
                                     "Integer and fractional infeasible solutions.");
         }
         else
         {
            System.Console.WriteLine("Benders' cuts separated to cut off: " +
                                     "Only integer infeasible solutions.");
         }

         if ( args.Length == 2 ) fileName = args[1];

         // Read arc_costs from data file (17 city problem)

         Data data = new Data(fileName);

         // create master ILP

         int numNodes = data.numNodes;
         Cplex cplex = new Cplex();
         IIntVar[][] x = new IIntVar[numNodes][]; 
         CreateMasterILP(cplex, data, x);
         
         // Create workerLP for Benders' cuts separation

         WorkerLP workerLP = new WorkerLP(numNodes);

         // Set up the cut callback to be used for separating Benders' cuts

         cplex.SetParam(Cplex.Param.Preprocessing.Presolve, false);

         // Set the maximum number of threads to 1. 
         // This instruction is redundant: If MIP control callbacks are registered, 
         // then by default CPLEX uses 1 (one) thread only.
         // Note that the current example may not work properly if more than 1 threads 
         // are used, because the callback functions modify shared global data.
         // We refer the user to the documentation to see how to deal with multi-thread 
         // runs in presence of MIP control callbacks. 

         cplex.SetParam(Cplex.Param.Threads, 1);

         // Turn on traditional search for use with control callbacks

         cplex.SetParam(Cplex.Param.MIP.Strategy.Search, Cplex.MIPSearch.Traditional);

         cplex.Use(new BendersLazyConsCallback(x, workerLP));
         if ( sepFracSols )
            cplex.Use(new BendersUserCutCallback(x, workerLP));

         // Solve the model and write out the solution

         if ( cplex.Solve() )
         {
            System.Console.WriteLine();
            System.Console.WriteLine("Solution status: " + cplex.GetStatus());
            System.Console.WriteLine("Objective value: " + cplex.ObjValue);

            if ( cplex.GetStatus().Equals(Cplex.Status.Optimal) )
            {
               // Write out the optimal tour

               int i, j;
               double[][] sol = new double[numNodes][];
               int[] succ = new int[numNodes];
               for (j = 0; j < numNodes; ++j)
                  succ[j] = -1;

               for (i = 0; i < numNodes; ++i)
               {
                  sol[i] = cplex.GetValues(x[i]);
                  for (j = 0; j < numNodes; ++j)
                  {
                     if ( sol[i][j] > 1e-03 ) succ[i] = j;
                  }
               }

               System.Console.WriteLine("Optimal tour:");
               i = 0;
               while ( succ[i] != 0 )
               {
                  System.Console.Write(i + ", ");
                  i = succ[i];
               }
               System.Console.WriteLine(i);
            }
            else
            {
               System.Console.WriteLine("Solution status is not Optimal");
            }
         }
         else
         {
            System.Console.WriteLine("No solution available");
         }

         workerLP.End();
         cplex.End();
      }
      catch (ILOG.Concert.Exception ex)
      {
         System.Console.WriteLine("Concert Error: " + ex);
      }
      catch (InputDataReader.InputDataReaderException ex)
      {
         System.Console.WriteLine("Data Error: " + ex);
      }
      catch (System.IO.IOException ex)
      {
         System.Console.WriteLine("IO Error: " + ex);
      }

   } // END Main


   static void Usage()
   {
      System.Console.WriteLine("Usage:     BendersATSP {0|1} [filename]");
      System.Console.WriteLine(" 0:        Benders' cuts only used as lazy constraints,");
      System.Console.WriteLine("           to separate integer infeasible solutions.");
      System.Console.WriteLine(" 1:        Benders' cuts also used as user cuts,");
      System.Console.WriteLine("           to separate fractional infeasible solutions.");
      System.Console.WriteLine(" filename: ATSP instance file name.");
      System.Console.WriteLine("           File ../../../../examples/data/atsp.dat used " +
                               "if no name is provided.");
   } // END Usage 

} // END BendersATSP
