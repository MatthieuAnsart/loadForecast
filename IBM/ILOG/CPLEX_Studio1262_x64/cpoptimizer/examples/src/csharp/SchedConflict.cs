// -------------------------------------------------------------- -*- C++ -*-
// File: ./examples/src/csharp/SchedConflict.cs
// --------------------------------------------------------------------------
// Licensed Materials - Property of IBM
//
// 5724-Y48 5724-Y49 5724-Y54 5724-Y55 5725-A06 5725-A29
// Copyright IBM Corporation 1990, 2014. All Rights Reserved.
//
// Note to U.S. Government Users Restricted Rights:
// Use, duplication or disclosure restricted by GSA ADP Schedule
// Contract with IBM Corp.
// --------------------------------------------------------------------------

/* --------------------------------------------------------------------------

Problem Description
-------------------

This model illustrates the use of the CP Optimizer conflict refiner
on an infeasible scheduling problem. 
The problem is an instance of RCPSP (Resource-Constrained Project Scheduling 
Problem) with time windows. Given:

- a set of q resources with given capacities,
- a network of precedence constraints between the activities,
- a set of activities to be executed within a given time window and
- for each activity and each resource the amount of the resource
  required by the activity over its execution,

the goal of the problem is to find a schedule satisfying all the
constraints whose makespan (i.e., the time at which all activities are
finished) is minimal.

The instance is infeasible. The example illustrates 5 scenarios using the 
conflict refiner:

- Scenario 1: Identify a minimal conflict (no matter which one).
- Scenario 2: Identify a minimal conflict preferably using resource capacity 
              constraints.
- Scenario 3: Identify a minimal conflict preferably using precedence 
              constraints.
- Scenario 4: Find a minimal conflict partition that is, a set of disjoint 
              minimal conflicts S1,...,Sn such that when all constraints in 
              S1 U S2 U... U Sn are removed from the model, the model becomes 
              feasible.
- Scenario 5: Identify all minimal conflicts of the problem.

----------------------------------------------------------------------------- */

using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using ILOG.CP;
using ILOG.Concert;

public class SchedConflict {

  public static CP cp;
  private static IConstraint[] capacityCts;
  private static IConstraint[] precedenceCts;
                   
  public SchedConflict(String fileName) {
    cp = new CP();
    buildModel(fileName);
  }
  
  class DataReader
  {
    private int index = -1;
    private string[] datas;

    public DataReader(String filename)
    {
      StreamReader reader = new StreamReader(filename);
      datas = reader.ReadToEnd().Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
    }

    public int Next()
    {
      index++;
      return Convert.ToInt32(datas[index]);
    }
  }
    
  //----- RCPSP Model creation ------------------------------------------------

  private static void buildModel(String fileName) {
    int nbTasks, nbResources;
    DataReader data = new DataReader(fileName);
    try {
      nbTasks = data.Next();
      nbResources = data.Next();
      List<IIntExpr>    ends             = new List<IIntExpr>();
      List<IConstraint> capacityCtList   = new List<IConstraint>();
      List<IConstraint> precedenceCtList = new List<IConstraint>();
      ICumulFunctionExpr[] resources = new ICumulFunctionExpr[nbResources];
      int[] capacities = new int[nbResources];
      for (int j = 0; j < nbResources; j++) {
        capacities[j] = data.Next();
        resources[j] = cp.CumulFunctionExpr();
      }
      IIntervalVar[] tasks = new IIntervalVar[nbTasks];
      for (int i = 0; i < nbTasks; i++) {
        tasks[i] = cp.IntervalVar();
        tasks[i].Name = "ACT" + i;
      }
      for (int i = 0; i < nbTasks; i++) {
        IIntervalVar task = tasks[i];
        int d, smin, emax, nbSucc;
        d    = data.Next();
        smin = data.Next();
        emax = data.Next();
        task.SizeMin  = d;
        task.SizeMax  = d;
        task.StartMin = smin;
        task.EndMax   = emax;
        ends.Add(cp.EndOf(task));
        for (int j = 0; j < nbResources; j++ ) {
          int q = data.Next();
          if (q > 0) {
            resources[j].Add(cp.Pulse(task, q));
          }
        }
        nbSucc = data.Next();
        for (int s = 0; s < nbSucc; s++ ) {
          int succ = data.Next();
          IConstraint pct = cp.EndBeforeStart(task, tasks[succ]);
          cp.Add(pct);
          precedenceCtList.Add(pct);
        }
      }
      for (int j = 0; j < nbResources; j++) {
        IConstraint cct = cp.Le(resources[j], capacities[j]);
        cp.Add(cct);
        capacityCtList.Add(cct);
      }
      precedenceCts = precedenceCtList.ToArray();
      capacityCts   = capacityCtList.ToArray();
      IObjective objective = cp.Minimize(cp.Max(ends.ToArray()));
      cp.Add(objective);
    } catch (ILOG.Concert.Exception e) {
        Console.WriteLine("Error: " + e);
    }
  }
    
  //----- Basic run of conflict refiner  --------------------------------------

  private static void runBasicConflictRefiner()  {
    if (cp.RefineConflict()) {
      cp.WriteConflict(Console.Out);
    } 
  }
    
  //----- Run conflict refiner with a subset of preferred constraints  --------

  private static void runConflictRefinerWithPreferences(CP cp, 
                                                 IConstraint[] preferredCts, 
                                                 IConstraint[] otherCts) {
    int nbPreferred = preferredCts.Length;
    int nbOther     = otherCts.Length;
    IConstraint[] cts   = new IConstraint[nbPreferred+nbOther];  
    double[]      prefs = new double[nbPreferred+nbOther];
    for (int i=0; i<nbOther; ++i) {
      cts  [i] = otherCts[i];
      prefs[i] = 1.0; // Normal preference
    }
    for (int i=0; i<nbPreferred; ++i) {
      cts  [nbOther+i]= preferredCts[i];
      prefs[nbOther+i]= 2.0; // Higher preference
    }
    if (cp.RefineConflict(cts, prefs)) {
      cp.WriteConflict(Console.Out);
    } 
  }

  //----- Use conflict refiner to compute a minimal conflict partition --------

  private static void runConflictRefinerPartition(CP cp, IConstraint[] cts) {     
    int n = cts.Length;
    double[] prefs = new double[n];   
    for (int i=0; i<n; ++i) {
      prefs[i]=1.0; // Normal preference
    }
    while (cp.RefineConflict(cts, prefs)) {
      cp.WriteConflict(Console.Out);
      for (int i=0; i<n; ++i) {
        if (cp.GetConflict(cts[i]) == CP.ConflictStatus.ConflictMember) {
          prefs[i]=-1.0; // Next run will ignore constraints of the current conflict
        }
      }
    }
  }

  //----- Use conflict refiner to compute all minimal conflicts  --------------
  //----- Note: the simple search below is complete but not systematic: -------
  //----- it may find the same conflict several times.                  -------
  
  private static void runConflictRefinerAllConflicts(CP cp, IConstraint[] cts) {     
    int n = cts.Length;
    double[] prefs = new double[n];   
    int i;  
    List<List<int>> forbiddenCts = new List<List<int>>();
    // Initial iteration: no forbidden constraints
    forbiddenCts.Add(new List<int>());
    while (0<forbiddenCts.Count) {
      List<int> forbidden = forbiddenCts[0];
      forbiddenCts.RemoveAt(0);
      for (i=0; i<n; ++i) {
        prefs[i]=1.0; // Initialize to normal preference
      }
      for (i=0; i<forbidden.Count; ++i) {
        prefs[forbidden[i]]=-1.0; // Remove constraint
      }
      if (cp.RefineConflict(cts, prefs)) {
        cp.WriteConflict(Console.Out);
        // Next iterations
        for (i=0; i<n; ++i) {
          if (cp.GetConflict(cts[i]) == CP.ConflictStatus.ConflictMember) {
            List<int> newforbidden = new List<int>();
            newforbidden.Add(i);
            newforbidden.AddRange(forbidden);           
            forbiddenCts.Add(newforbidden);
          }
        }
      }
    }
  }
                                       
                                       
  public static void Main(String[] args) {
    String fileName = "../../../../examples/data/sched_conflict.data";
    int failLimit = 10000;
    if (args.Length > 0)
      fileName = args[0];
    if (args.Length > 1)
      failLimit = Convert.ToInt32(args[1]);
    try {
      SchedConflict problem = new SchedConflict(fileName);
      int nbCapacityCts   = capacityCts.Length;
      int nbPrecedenceCts = precedenceCts.Length;
      IConstraint[] allCts = new IConstraint[nbCapacityCts+nbPrecedenceCts];  
      for (int i=0; i<nbCapacityCts; ++i) {
        allCts[i] = capacityCts[i];
      }
      for (int i=0; i<nbPrecedenceCts; ++i) {
        allCts[nbCapacityCts+i] = precedenceCts[i];
      }
      cp.SetParameter(CP.IntParam.FailLimit, failLimit);
      cp.SetParameter(CP.IntParam.CumulFunctionInferenceLevel, CP.ParameterValues.Extended);
      cp.SetParameter(CP.IntParam.ConflictRefinerOnVariables, CP.ParameterValues.On);
      Console.WriteLine("Instance \t: " + fileName);
      if (cp.Solve()) {
        // A solution was found
        Console.WriteLine("Solution found with makespan : " + cp.ObjValue);
      } else {
        int status = cp.GetInfo(CP.IntInfo.FailStatus);
        if (status != CP.ParameterValues.SearchHasFailedNormally.Value) {
          // No solution found but problem was not proved infeasible
          Console.WriteLine("No solution found but problem was not proved infeasible.");
        } else {
          // Run conflict refiner only if problem was proved infeasible
          Console.WriteLine("Infeasible problem, running conflict refiner ...");
          Console.WriteLine("SCENARIO 1: Basic conflict refiner:");
          runBasicConflictRefiner();    
          cp.SetParameter(CP.IntParam.LogVerbosity, CP.ParameterValues.Quiet);
          Console.WriteLine("SCENARIO 2: Conflict refiner with preference on resource capacity constraints:");
          runConflictRefinerWithPreferences(cp, capacityCts, precedenceCts);
          Console.WriteLine("SCENARIO 3: Conflict refiner with preference on precedence constraints:");
          runConflictRefinerWithPreferences(cp, precedenceCts, capacityCts);
          Console.WriteLine("SCENARIO 4: Conflict partition:"); 
          runConflictRefinerPartition(cp, allCts);
          Console.WriteLine("SCENARIO 5: All conflicts:");
          runConflictRefinerAllConflicts(cp, allCts);
        }
      }
    } catch (ILOG.Concert.Exception e) {
      Console.WriteLine("Error: " + e);
    }
  }
}
