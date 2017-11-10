// -----------------------------------------------------------------*- C# -*-
// File: ./examples/src/csharp/Color.cs
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

The problem involves choosing colors for the countries on a map in 
such a way that at most four colors (blue, white, yellow, green) are 
used and no neighboring countries are the same color. In this exercise, 
you will find a solution for a map coloring problem with six countries: 
Belgium, Denmark, France, Germany, Luxembourg, and the Netherlands. 

------------------------------------------------------------ */

using System;
using System.IO;
using ILOG.CP;
using ILOG.Concert;

namespace Color {
  public class Color {
    public static string[] Names = {"blue", "white", "red", "green"}; 
    static void Main() {   
      CP cp = new CP();
      IIntVar Belgium = cp.IntVar(0, 3);
      IIntVar Denmark = cp.IntVar(0, 3); 
      IIntVar France = cp.IntVar(0, 3);
      IIntVar Germany = cp.IntVar(0, 3); 
      IIntVar Netherlands = cp.IntVar(0, 3);
      IIntVar Luxembourg = cp.IntVar(0, 3);
 
      cp.Add(cp.Neq(Belgium, France)); 
      cp.Add(cp.Neq(Belgium, Germany)); 
      cp.Add(cp.Neq(Belgium, Netherlands)); 
      cp.Add(cp.Neq(Belgium, Luxembourg));
      cp.Add(cp.Neq(Denmark, Germany)); 
      cp.Add(cp.Neq(France, Germany)); 
      cp.Add(cp.Neq(France, Luxembourg));  
      cp.Add(cp.Neq(Germany , Luxembourg)); 
      cp.Add(cp.Neq(Germany, Netherlands));

      // Search for a solution
      if (cp.Solve()) {    
        Console.WriteLine("Solution: ");
        Console.WriteLine("Belgium:     " + Names[ cp.GetIntValue( Belgium ) ] );
        Console.WriteLine("Denmark:     " + Names[ cp.GetIntValue( Denmark ) ] );
        Console.WriteLine("France:      " + Names[ cp.GetIntValue( France ) ] );
        Console.WriteLine("Germany:     " + Names[ cp.GetIntValue( Germany ) ] );
        Console.WriteLine("Netherlands: " + Names[ cp.GetIntValue( Netherlands ) ] );
        Console.WriteLine("Luxembourg:  " + Names[ cp.GetIntValue( Luxembourg ) ] );
      }
    }
  }
}
