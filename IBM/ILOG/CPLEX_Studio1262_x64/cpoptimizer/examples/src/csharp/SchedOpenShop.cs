// -----------------------------------------------------------------*- C# -*-
// File: ./examples/src/csharp/SchedOpenShop.cs
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

This problem can be described as follows: a finite set of operations
has to be processed on a given set of machines. Each operation has a
specific processing time during which it may not be interrupted.
Operations are grouped in jobs, so that each operation belongs to
exactly one job. Furthermore, each operation requires exactly one
machine for processing.

The objective of the problem is to schedule all operations, i.e., to
determine their start time, so as to minimize the maximum completion
time (makespan) given the additional constraints that: operations
which belong to the same job and operations which use the same machine
cannot be processed simultaneously.

------------------------------------------------------------ */

using System;
using System.IO;
using System.Collections.Generic;
using ILOG.CP;
using ILOG.Concert;

namespace SchedOpenShop
{
    public class SchedopenShop
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
            String filename = "../../../../examples/data/openshop_default.data";
            int failLimit = 10000;

            if (args.Length > 0)
                filename = args[0];
            if (args.Length > 1)
                failLimit = Convert.ToInt32(args[1]);

            CP cp = new CP();

            DataReader data = new DataReader(filename);
            int nbJobs = data.Next();
            int nbMachines = data.Next();

            List<IIntervalVar>[] jobs = new List<IIntervalVar>[nbJobs];
            for (int i = 0; i < nbJobs; i++)
                jobs[i] = new List<IIntervalVar>();
            List<IIntervalVar>[] machines = new List<IIntervalVar>[nbMachines];
            for (int j = 0; j < nbMachines; j++)
                machines[j] = new List<IIntervalVar>();

            List<IIntExpr> ends = new List<IIntExpr>();
            for (int i = 0; i < nbJobs; i++)
            {
                for (int j = 0; j < nbMachines; j++)
                {
                    int pt = data.Next();
                    IIntervalVar ti = cp.IntervalVar(pt);
                    jobs[i].Add(ti);
                    machines[j].Add(ti);
                    ends.Add(cp.EndOf(ti));
                }
            }

            for (int i = 0; i < nbJobs; i++)
                cp.Add(cp.NoOverlap(jobs[i].ToArray()));

            for (int j = 0; j < nbMachines; j++)
                cp.Add(cp.NoOverlap(machines[j].ToArray()));

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
