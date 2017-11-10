// -----------------------------------------------------------------*- C# -*-
// File: ./examples/src/csharp/SchedStochasticJobShop.cs
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

The Stochastic Job-Shop Scheduling problem is a variant of the classical 
deterministic Job-Shop Scheduling problem (see SchedJobShop.cs) where 
the duration of operations is uncertain. 

Scenarios
---------

This example illustrates how to solve a Stochastic Job-Shop Scheduling 
problem using a scenario-based approach. A set of n scenarios is created, 
each scenario represents a particular realization of the durations of 
the operations. 

The instance is a small 6x6 Job-Shop Scheduling problem with 20 scenarios.
In the example we suppose the scenarios are given as input. In practical 
problems, these scenarios may be given by a selection of representative 
past execution of the system or they may be computed by sampling the 
probability distributions of operations duration.

For example the different scenarios give the following durations 
for the 6 operations of the first job:

JOB #1
                 Machine:  M5 -> M1 -> M4 -> M3 -> M0 -> M2
Duration in scenario #00: 218  284  321  201  101  199
Duration in scenario #01: 259  313  311  191   93  211
... 
Duration in scenario #19: 501  309  301  203   95  213

The objective is to find a robust sequencing of operations on machines so
as to minimize the expected makespan across all scenarios.

The problem can be seen as a particular case of Two-Stage Stochastic 
Programming problem where first stage decision variables are the sequences 
of operations on machines and second stage decision variables are the 
actual start/end time of operations that will depend on the actual duration
of operations at execution time.
 
The model proposed here generalizes to any type of stochastic scheduling 
problem where first stage decision variables involve creating robust 
sequences of operations on a machine.

Model
-----

Each scenario is modeled as a particular deterministic Job-Shop Scheduling 
problem.

Let makespan[k] denote the makespan of scenario k and sequence[k][j] denote 
the sequence variable representing the sequencing of operations on machine 
j in scenario k.

A set of 'sameSequence' constraints are posted across all scenarios k to 
state that for a machine j, the sequence of operations should be the same 
for all scenarios. The sequence variable of the first scenario (sequence[0][j]) 
is used as reference:
forall j, forall 0<k: sameSequence(sequence[0][j],sequence[k][j])

The global objective function is the average makespan over the different 
scenarios:
objective: (sum(k) makespan[k]) / nbScenarios

Solution quality
----------------

Solution with expected makespan 4648.4 is optimal. As the sample is small,
this solution can be proved to be optimal by exploring the complete search 
tree in depth first search as follows:
  cp.SetParameter(CP.IntParam.SearchType, CP.ParameterValues.DepthFirst);  
  cp.Solve(cp.SearchPhase(refSequences));

Note that the solution built by using the optimal solution of the 
deterministic Job-Shop Scheduling problem using average operation duration 
yields an expected makespan of 4749.6 which is clearly suboptimal.

Nevertheless, in practical stochastic scheduling problems, a solution 
to a deterministic version of the problem (like the one using an average behavior) 
may provide an interesting starting point for the scenario-based stochastic model 
(See CP Optimizer Starting Point concept).

------------------------------------------------------------ */

using System;
using System.IO;
using System.Collections.Generic;
using ILOG.CP;
using ILOG.Concert;

namespace SchedStochasticJobShop {
  public class SchedStochasticJobShop {
    class DataReader {
      private int index = -1;
      private string[] datas;

      public DataReader(String filename) {
        StreamReader reader = new StreamReader(filename);
        datas = reader.ReadToEnd().Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
      }

      public int Next() {
        index++;
        return Convert.ToInt32(datas[index]);
      }
    }

    public static IIntExpr MakeScenarioSubmodel(CP                     cp,
                                                int                    nbJobs,
                                                int                    nbMachines,
                                                int[][]                machines,
                                                int[][]                durations,
                                                IIntervalSequenceVar[] sequences)  {
      List<IIntervalVar>[] machinesOps = new List<IIntervalVar>[nbMachines];
      String name;
      int i,j;
      for (j = 0; j < nbMachines; j++)
        machinesOps[j] = new List<IIntervalVar>();
      IIntExpr[] ends = new IIntExpr[nbJobs];
      for (i = 0; i < nbJobs; i++) {
        IIntervalVar prec = cp.IntervalVar();
        for (j = 0; j < nbMachines; j++) {
          name = "J" + i + "_O" + j; 
          IIntervalVar ti = cp.IntervalVar(durations[i][j], name);
          machinesOps[machines[i][j]].Add(ti);
          if (j > 0)
            cp.Add(cp.EndBeforeStart(prec, ti));
          prec = ti;
        }
        ends[i] = cp.EndOf(prec);
      }
      for (j = 0; j < nbMachines; j++) {
        name = "M" + j;
        sequences[j] = cp.IntervalSequenceVar(machinesOps[j].ToArray(), name);
        cp.Add(cp.NoOverlap(sequences[j]));
      }
      return cp.Max(ends);
    }
  
    public static void Main(String[] args) {
      String filename = "../../../../examples/data/stochastic_jobshop_default.data";
      int failLimit = 250000;

      if (args.Length > 0)
        filename = args[0];
      if (args.Length > 1)
        failLimit = Convert.ToInt32(args[1]);

      // Data reading
      DataReader data = new DataReader(filename);
      int nbJobs, nbMachines, nbScenarios;
      nbJobs      = data.Next();
      nbMachines  = data.Next();
      nbScenarios = data.Next();
      int i,j,k;
      // machines[i][j]: machine used by jth operation of job i
      int[][] machines = new int[nbJobs][]; 
      for (i = 0; i < nbJobs; i++) {
        machines[i] = new int[nbMachines];
        for (j = 0; j < nbMachines; j++) {
          machines[i][j] = data.Next();
        }
      }
      // durations[k][i][j]: duration of jth operation of job i in scenario k
      int[][][] durations = new int[nbScenarios][][]; 
      for (k = 0; k < nbScenarios; k++) {
        durations[k] = new int[nbJobs][];
        for (i = 0; i < nbJobs; i++) {
          durations[k][i] = new int[nbMachines];
          for (j = 0; j < nbMachines; j++) {
            durations[k][i][j] = data.Next();
          }
        }
      }
 
      CP cp = new CP();
      IIntervalSequenceVar[] refSequences = new IIntervalSequenceVar[nbMachines];
      IIntExpr sumMakespan = cp.IntExpr();    
      for (k = 0; k < nbScenarios; k++) {
        IIntervalSequenceVar[] scenarioSequences = new IIntervalSequenceVar[nbMachines];
        IIntExpr scenarioMakespan = 
          MakeScenarioSubmodel(cp, nbJobs, nbMachines,
                               machines, durations[k],
                               scenarioSequences);
        // Objective function is aggregated
        sumMakespan = cp.Sum(sumMakespan, scenarioMakespan);
        // For each machine, a sameSequence constraint is posted across all scenarios
        if (0==k) {
          refSequences = scenarioSequences;
        } else {
          for (j = 0; j < nbMachines; j++) {
            cp.Add(cp.SameSequence(refSequences[j], scenarioSequences[j]));
          }
        }
      }
      // Objective function is expected makespan
      INumExpr expectedMakespan = cp.Quot(sumMakespan, nbScenarios);
      IObjective objective = cp.Minimize(expectedMakespan);
      cp.Add(objective);
      cp.SetParameter(CP.IntParam.FailLimit, failLimit);
      cp.SetParameter(CP.IntParam.LogPeriod, 1000000);
      Console.WriteLine("Instance \t: " + filename);
      if (cp.Solve()) {
        Console.WriteLine("Expected makespan \t: " + cp.ObjValue);
        for (j=0; j<nbMachines; ++j) {
          IIntervalSequenceVar s = refSequences[j];
          Console.Write(s.Name + ":\t");
          IIntervalVar op = cp.GetFirst(s);
          for (; !op.Equals(cp.GetLast(s)); op = cp.GetNext(s, op))
            Console.Write(op.Name+ "\t");
          Console.WriteLine(op.Name+ "\t");
        }
      } else {
        Console.WriteLine("No solution found.");
      }
    }
  }
}
