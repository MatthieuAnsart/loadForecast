// --------------------------------------------------------------------------
// File: DistMIPEx1.cs
// Version 12.6.2
// --------------------------------------------------------------------------
// Licensed Materials - Property of IBM
// 5725-A06 5725-A29 5724-Y48 5724-Y49 5724-Y54 5724-Y55 5655-Y21
// Copyright IBM Corporation 2013, 2015. All Rights Reserved.
//
// US Government Users Restricted Rights - Use, duplication or
// disclosure restricted by GSA ADP Schedule Contract with
// IBM Corp.
// --------------------------------------------------------------------------
//
// DistMIPEx1.cs - Reading a MIP problem from a file and solving
//                 it via distributed parallel optimization.
// See the usage() function for details about how to run this.

using ILOG.Concert;
using ILOG.CPLEX;


public class DistMIPEx1 {
   private static void Usage() {
      string program = "DistMIPEx1.exe";
      System.Console.WriteLine("Usage: " + program + " <vmc> <model>");
      System.Console.WriteLine("   Solves a model specified by a model file using");
      System.Console.WriteLine("   distributed parallel MIP.");
      System.Console.WriteLine("   Arguments:");
      System.Console.WriteLine("    <vmc>    The virtual machine configuration file");
      System.Console.WriteLine("             that describes the machine that can be");
      System.Console.WriteLine("             used for parallel distributed MIP.");
      System.Console.WriteLine("    <model>  Model file with the model to be solved.");
      System.Console.WriteLine("   Example:");
      System.Console.WriteLine("     " + program + " process.vmc model.lp");
   }
   public static void Main(string[] args) {
      // Check length of command line
      if ( args.Length != 2 ) {
         Usage();
         System.Environment.Exit(-1);
      }

      // Pick up VMC from command line.
      string vmconfig = args[0];

      // Solve the model.
      Cplex cplex = null;
      try {
         // Create CPLEX solver and load model.
         cplex = new Cplex();
         cplex.ImportModel(args[1]);

         // Load the virtual machine configuration.
         // This will force Solve() to use distributed parallel MIP.
         cplex.ReadVMConfig(vmconfig);

         // Solve the model and print some solution information.
         if ( cplex.Solve() )
            System.Console.WriteLine("Solution value  = " + cplex.ObjValue);
         else
            System.Console.WriteLine("No solution available");
         System.Console.WriteLine("Solution status = " + cplex.GetStatus());
      }
      catch (ILOG.Concert.Exception e) {
         System.Console.WriteLine("Concert exception caught '" + e + "' caught");
         if ( cplex != null )
            cplex.End();
         System.Environment.Exit(-1);
      }
      finally {
         if ( cplex != null )
            cplex.End();
      }
   }
}
