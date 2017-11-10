// -----------------------------------------------------------------*- C# -*-
// File: ./examples/src/csharp/Ppp.cs
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

 For a description of the problem and resolution methods:

    The Progressive Party Problem: Integer Linear Programming
    and Constraint Programming Compared

    Proceedings of the First International Conference on Principles
    and Practice of Constraint Programming table of contents

    Lecture Notes In Computer Science; Vol. 976, pages 36-52, 1995
    ISBN:3-540-60299-2

------------------------------------------------------------ */

using System;
using System.IO;
using ILOG.CP;
using ILOG.Concert;


namespace ppp {
  public class ppp {
    //
    // Matrix operations
    //
    static private IIntVar[][] Transpose(IIntVar[][] x) {
      int m = x.Length;
      int n = x[0].Length;
      IIntVar[][] y = new IIntVar[n][];
      for (int i = 0; i < n; i++) {
        y[i] = new IIntVar[m];
        for (int j = 0; j < m; j++)
          y[i][j] = x[j][i];
      }
      return y;
    }

    //
    // Data
    //
    static int numBoats = 42;

    static int[] boatSize = {
      7, 8, 12, 12, 12, 12, 12, 10, 10, 10,
      10, 10, 8, 8, 8, 12, 8, 8, 8, 8,
      8, 8, 7, 7, 7, 7, 7, 7, 6, 6,
      6, 6, 6, 6, 6, 6, 6, 6, 9, 2,
      3, 4
    };
    static int[] crewSize = {
      2, 2, 2, 2, 4, 4, 4, 1, 2, 2,
      2, 3, 4, 2, 3, 6, 2, 2, 4, 2,
      4, 5, 4, 4, 2, 2, 4, 5, 2, 4,
      2, 2, 2, 2, 2, 2, 4, 5, 7, 2,
      3, 4
    };

    static public void Main(string[] args) {
      int numPeriods = 6;
      if (args.Length > 0)
        numPeriods = Int32.Parse(args[0]);

      CP cp = new CP();
      //
      // Variables
      //

      // Host boat choice
      IIntVar[] host = new IIntVar[numBoats];
      for (int j = 0; j < numBoats; j++) {
        host[j] = cp.IntVar(0, 1, String.Format("H{0}", j));
      }
    
      // Who is where each time period (time- and boat-based views)
      IIntVar[][] timePeriod = new IIntVar[numPeriods][];
      for (int i = 0; i < numPeriods; i++) {
        timePeriod[i] = new IIntVar[numBoats];
        for (int j = 0; j < numBoats; j++) {
          timePeriod[i][j] = cp.IntVar(0, numBoats-1, String.Format("T{0},{1}", i, j));
        }
      }
      IIntVar[][]  visits = Transpose(timePeriod);

      //
      // Objective
      //
      IIntVar numHosts = cp.IntVar(numPeriods, numBoats);
      cp.Add(cp.Eq(numHosts, cp.Sum(host)));
      cp.Add(cp.Minimize(numHosts));

      //
      // Constraints
      //

      // Stay in my boat (host) or only visit other boats (guest)
      for (int i = 0; i < numBoats; i++)
        cp.Add(cp.Eq(cp.Count(visits[i], i), cp.Prod(host[i], numPeriods)));

      // Capacity constraints: only hosts have capacity
      for (int p = 0; p < numPeriods; p++) {
        IIntVar[] load = new IIntVar[numBoats];
        for (int j = 0; j < numBoats; j++) {
          load[j] = cp.IntVar(0, boatSize[j], String.Format("L{0},{1}", p, j));
          cp.Add(cp.Le(load[j], cp.Prod(host[j], boatSize[j])));
        }
        cp.Add(cp.Pack(load, timePeriod[p], crewSize, numHosts));
      }

      // No two crews meet more than once
      for (int i = 0; i < numBoats; i++) {
        for (int j = i + 1; j < numBoats; j++) {
          IIntExpr timesMet = cp.Constant(0);
          for (int p = 0; p < numPeriods; p++)
            timesMet = cp.Sum(timesMet, cp.Eq(visits[i][p], visits[j][p]));
          cp.Add(cp.Le(timesMet, 1));
        }
      } 

      // Host and guest boat constraints: given in problem spec
      cp.Add(cp.Eq(host[0], 1));
      cp.Add(cp.Eq(host[1], 1));
      cp.Add(cp.Eq(host[2], 1));
      cp.Add(cp.Eq(host[39], 0));
      cp.Add(cp.Eq(host[40], 0));
      cp.Add(cp.Eq(host[41], 0));

      //
      // Solving
      //
      if (cp.Solve()) {
        Console.WriteLine("Solution at cost = {0}", cp.GetValue(numHosts));
        Console.Write("Hosts: ");
        for (int i = 0; i < numBoats; i++) 
          Console.Write(cp.GetValue(host[i]));
        Console.WriteLine();
    
        for (int p = 0; p < numPeriods; p++) {
          Console.WriteLine("Period {0}", p);
          for (int h = 0; h < numBoats; h++) {
            if (cp.GetValue(host[h]) > 0) {
              Console.Write("\tHost {0} : ", h);
              int load = 0;
              for (int i = 0; i < numBoats; i++) {
                if (cp.GetValue(visits[i][p]) == h) {
                  load += crewSize[i];
                  Console.Write("{0} ({1}) ",i, crewSize[i]);
                }
              }
              Console.WriteLine(" --- {0} / {1}", load, boatSize[h]);
            }
          }
        }
        Console.WriteLine();
      }
    }
  }
}
