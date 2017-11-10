// -----------------------------------------------------------------*- C# -*-
// File: ./examples/src/csharp/SchedRCPSPMM.cs
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

The MMRCPSP (Multi-Mode Resource-Constrained Project Scheduling
Problem) is a generalization of the Resource-Constrained Project
Scheduling problem (see SchedRCPSP.cs). In the MMRCPSP, each
activity can be performed in one out of several modes. Each mode of an
activity represents an alternative way of combining different levels
of resource requirements with a related duration. Renewable and
no-renewable resources are distinguished. While renewable resources
have a limited instantaneous availability such as manpower and
machines, non renewable resources are limited for the entire project,
allowing to model, e.g., a budget for the project.  The objective is
to find a mode and a start time for each activity such that the
schedule is makespan minimal and feasible with regard to the
precedence and resource constraints.

------------------------------------------------------------ */

using System;
using System.IO;
using System.Collections.Generic;
using ILOG.CP;
using ILOG.Concert;

namespace SchedRCPSPMM
{
    public class SchedRCPSPMM
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
            String filename = "../../../../examples/data/rcpspmm_default.data";
            int failLimit = 30000;
            int nbTasks, nbRenewable, nbNonRenewable;

            if (args.Length > 0)
                filename = args[0];
            if (args.Length > 1)
                failLimit = Convert.ToInt32(args[1]);

            CP cp = new CP();
            DataReader data = new DataReader(filename);

            nbTasks = data.Next();
            nbRenewable = data.Next();
            nbNonRenewable = data.Next();
            ICumulFunctionExpr[] renewables = new ICumulFunctionExpr[nbRenewable];
            IIntExpr[] nonRenewables = new IIntExpr[nbNonRenewable];
            int[] capRenewables = new int[nbRenewable];
            int[] capNonRenewables = new int[nbNonRenewable];
            for (int j = 0; j < nbRenewable; j++)
            {
                renewables[j] = cp.CumulFunctionExpr();
                capRenewables[j] = data.Next();
            }
            for (int j = 0; j < nbNonRenewable; j++)
            {
                nonRenewables[j] = cp.IntExpr();
                capNonRenewables[j] = data.Next();
            }

            IIntervalVar[] tasks = new IIntervalVar[nbTasks];
            List<IIntervalVar>[] modes = new List<IIntervalVar>[nbTasks];
            for (int i = 0; i < nbTasks; i++)
            {
                tasks[i] = cp.IntervalVar();
                modes[i] = new List<IIntervalVar>();
            }
            List<IIntExpr> ends = new List<IIntExpr>();
            for (int i = 0; i < nbTasks; i++)
            {
                IIntervalVar task = tasks[i];
                int d = data.Next();
                int nbModes = data.Next();
                int nbSucc = data.Next();
                for (int k = 0; k < nbModes; k++)
                {
                    IIntervalVar alt = cp.IntervalVar();
                    alt.SetOptional();
                    modes[i].Add(alt);
                }
                cp.Add(cp.Alternative(task, modes[i].ToArray()));
                ends.Add(cp.EndOf(task));
                for (int s = 0; s < nbSucc; s++)
                {
                    int succ = data.Next();
                    cp.Add(cp.EndBeforeStart(task, tasks[succ]));
                }
            }
            for (int i = 0; i < nbTasks; i++)
            {
                IIntervalVar task = tasks[i];
                List<IIntervalVar> imodes = modes[i];
                for (int k = 0; k < imodes.Count; k++)
                {
                    int taskId = data.Next();
                    int modeId = data.Next();
                    int d = data.Next();
                    imodes[k].SizeMin = d;
                    imodes[k].SizeMax = d;
                    int q;
                    for (int j = 0; j < nbNonRenewable; j++)
                    {
                        q = data.Next();
                        if (0 < q)
                        {
                            renewables[j].Add(cp.Pulse(imodes[k], q));
                        }
                    }
                    for (int j = 0; j < nbNonRenewable; j++)
                    {
                        q = data.Next();
                        if (0 < q)
                        {
                            nonRenewables[j] = cp.Sum(nonRenewables[j], cp.Prod(q, cp.PresenceOf(imodes[k])));
                        }
                    }
                }
            }

            for (int j = 0; j < nbRenewable; j++)
            {
                cp.Add(cp.Le(renewables[j], capRenewables[j]));
            }

            for (int j = 0; j < nbRenewable; j++)
            {
                cp.Add(cp.Le(nonRenewables[j], capNonRenewables[j]));
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
    }
}
