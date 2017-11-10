// -----------------------------------------------------------------*- C# -*-
// File: ./examples/src/csharp/SchedPFlowShop.cs
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

The general Flow-Shop scheduling problem is a production problem where
a set of n jobs have to be processed with identical flow pattern on m
machines (see SchedFlowShop.cs). In permutation Flow-Shops the
sequence of jobs is the same on all machines.

------------------------------------------------------------ */

using System;
using System.IO;
using System.Collections.Generic;
using ILOG.CP;
using ILOG.Concert;

namespace SchedPFlowShop
{
    public class SchedPFlowShop
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
        String filename = "../../../../examples/data/flowshop_default.data";
        int failLimit = 10000;
        int nbJobs, nbMachines;

        if (args.Length > 0)
            filename = args[0];
        if (args.Length > 1)
            failLimit = Convert.ToInt32(args[1]);

        CP cp = new CP();

        DataReader data = new DataReader(filename);

        nbJobs = data.Next();
        nbMachines = data.Next();

        List<IIntExpr> ends = new List<IIntExpr>();
        List<IIntervalVar>[] machines = new List<IIntervalVar>[nbMachines];
        for (int j = 0; j < nbMachines; j++)
            machines[j] = new List<IIntervalVar>();
        for (int i = 0; i < nbJobs; i++)
        {
            IIntervalVar prec = cp.IntervalVar();
            for (int j = 0; j < nbMachines; j++)
            {
                int d = data.Next();
                IIntervalVar ti = cp.IntervalVar(d);
                machines[j].Add(ti);
                if (j > 0)
                {
                    cp.Add(cp.EndBeforeStart(prec, ti));
                }
                prec = ti;
            }
            ends.Add(cp.EndOf(prec));
        }

        IIntervalSequenceVar[] seqs = new IIntervalSequenceVar[nbMachines];
        for (int j = 0; j < nbMachines; j++)
        {
            seqs[j] = cp.IntervalSequenceVar(machines[j].ToArray());
            cp.Add(cp.NoOverlap(seqs[j]));
            if (0 < j)
            {
                cp.Add(cp.SameSequence(seqs[0], seqs[j]));
            }
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
