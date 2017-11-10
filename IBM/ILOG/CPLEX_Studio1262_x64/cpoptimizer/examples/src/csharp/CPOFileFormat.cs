// -----------------------------------------------------------------*- C# -*-
// File: ./examples/src/csharp/CPOFileFormat.cs
// --------------------------------------------------------------------------
// Licensed Materials - Property of IBM
//
// 5724-Y48 5724-Y49 5724-Y54 5724-Y55 5725-A06 5725-A29
// Copyright IBM Corporation 1990, 2014. All Rights Reserved.
//
// Note to U.S. Government Users Restricted Rights:
// Use, duplication or disclosure restricted by GSA ADP Schedule
// Contract with IBM Corp.
// -------------------------------------------------------------------------

/* ------------------------------------------------------------

Problem Description
-------------------

This example illustrate importModel/dumpModel functionality on a simple map
coloring problem. Function createModel builds a model using Concert .NET API
and then, instead of solving it, it dumps the model into text file (the
default file name is cpofileformat.cpo). After that, function SolveModel
imports the model from the file into another instance of IloCP, modifies
domain of one of the variables (France must be blue), solves the model and
prints the solution.

------------------------------------------------------------ */

using System;
using System.IO;
using ILOG.CP;
using ILOG.Concert;

namespace CPOFileFormat {
  public class CPOFileFormat {
  
    public static string[] Names = {"blue", "white", "red", "green"}; 
    
    static private void CreateModel(string filename) {
      try {
        CP cp = new CP();
        IIntVar Belgium     = cp.IntVar(0, 3, "Belgium");
        IIntVar Denmark     = cp.IntVar(0, 3, "Denmark"); 
        IIntVar France      = cp.IntVar(0, 3, "France");
        IIntVar Germany     = cp.IntVar(0, 3, "Germany"); 
        IIntVar Netherlands = cp.IntVar(0, 3, "Netherlands");
        IIntVar Luxembourg  = cp.IntVar(0, 3, "Luxembourg");
        cp.Add(cp.Neq(Belgium,  France)); 
        cp.Add(cp.Neq(Belgium,  Germany)); 
        cp.Add(cp.Neq(Belgium,  Netherlands)); 
        cp.Add(cp.Neq(Belgium,  Luxembourg));
        cp.Add(cp.Neq(Denmark,  Germany)); 
        cp.Add(cp.Neq(France,   Germany)); 
        cp.Add(cp.Neq(France,   Luxembourg));  
        cp.Add(cp.Neq(Germany , Luxembourg)); 
        cp.Add(cp.Neq(Germany,  Netherlands));
        cp.DumpModel(filename);
      } catch (ILOG.Concert.Exception e) {
        Console.WriteLine("ERROR:" + e);
      }  
    }
    
    static private void SolveModel(string filename) {
      try {
        CP cp = new CP();
        cp.ImportModel(filename);
        // Force blue color (zero) for France:
        IIntVar varFrance = cp.GetIIntVar("France");
        varFrance.Max = 0;
        // Search for a solution
        if (cp.Solve()) {  
          Console.WriteLine("Solution: ");
          IIntVar[] vars = cp.GetAllIIntVars();   
          for (int i = 0; i < vars.Length; i++) {        
            Console.WriteLine(vars[i].Name + ": " + Names[ cp.GetIntValue( vars[i] ) ] );
          }
        }
      } catch (ILOG.Concert.Exception e) {
        Console.WriteLine("ERROR:" + e);
      }  
    }
    
    static void Main() {   
      string filename = "cpofileformat.cpo";
      CreateModel(filename);
      SolveModel(filename);
    }
  }
}
