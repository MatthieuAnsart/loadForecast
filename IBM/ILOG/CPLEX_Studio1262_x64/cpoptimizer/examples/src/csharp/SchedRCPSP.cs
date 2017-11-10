// -----------------------------------------------------------------*- C# -*-
// File: ./examples/src/csharp/SchedRCPSP.cs
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

The RCPSP (Resource-Constrained Project Scheduling Problem) is a
generalization of the production-specific Job-Shop (see
SchedJobShop.cs), Flow-Shop (see SchedFlowShop.cs) and Open-Shop (see
SchedOpenShop.cs) scheduling problems. Given:

- a set of q resources with given capacities,
- a network of precedence constraints between the activities, and
- for each activity and each resource the amount of the resource
  required by the activity over its execution,

the goal of the RCPSP is to find a schedule meeting all the
constraints whose makespan (i.e., the time at which all activities are
finished) is minimal.

------------------------------------------------------------ */

using System;
using System.IO;
using System.Collections.Generic;
using ILOG.CP;
using ILOG.Concert;

namespace SchedRCPSP
{
    public class SchedRCPSP
    {
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

        public static void Main(String[] args)
        {
            String filename = "../../../../examples/data/rcpsp_default.data";
            int failLimit = 10000;
            int nbTasks, nbResources;

            if (args.Length > 0)
                filename = args[0];
            if (args.Length > 1)
                failLimit = Convert.ToInt32(args[1]);

            CP cp = new CP();
            DataReader data = new DataReader(filename);
            try
            {
                nbTasks = data.Next();
                nbResources = data.Next();
                List<IIntExpr> ends = new List<IIntExpr>();
                ICumulFunctionExpr[] resources = new ICumulFunctionExpr[nbResources];
                int[] capacities = new int[nbResources];

                for (int j = 0; j < nbResources; j++)
                {
                    capacities[j] = data.Next();
                    resources[j] = cp.CumulFunctionExpr();
                }
                IIntervalVar[] tasks = new IIntervalVar[nbTasks];
                for (int i = 0; i < nbTasks; i++)
                {
                    tasks[i] = cp.IntervalVar();
                }
                for (int i = 0; i < nbTasks; i++)
                {
                    IIntervalVar task = tasks[i];
                    int d, nbSucc;
                    d = data.Next();
                    task.SizeMin = d;
                    task.SizeMax = d;
                    ends.Add(cp.EndOf(task));
                    for (int j = 0; j < nbResources; j++)
                    {
                        int q = data.Next();
                        if (q > 0)
                            resources[j].Add(cp.Pulse(task, q));
                    }
                    nbSucc = data.Next();
                    for (int s = 0; s < nbSucc; s++)
                    {
                        int succ = data.Next();
                        cp.Add(cp.EndBeforeStart(task, tasks[succ - 1]));
                    }
                }

                for (int j = 0; j < nbResources; j++)
                {
                    cp.Add(cp.Le(resources[j], capacities[j]));
                }

                IObjective objective = cp.Minimize(cp.Max(ends.ToArray()));
                cp.Add(objective);

                cp.SetParameter(CP.IntParam.FailLimit, failLimit);
                Console.WriteLine("Instance \t: " + filename);
                if (cp.Solve())
                {
                    Console.WriteLine("Makespan \t: " + cp.ObjValue);
                }
                else
                {
                    Console.WriteLine("No solution found.");
                }
            }
            catch (ILOG.Concert.Exception e)
            {
                Console.WriteLine(" ERROR: " + e);
            }
        }
    }
}
