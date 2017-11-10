// -----------------------------------------------------------------*- C# -*-
// File: ./examples/src/csharp/SchedJobShop.cs
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

In the classical Job-Shop Scheduling problem a finite set of jobs is
processed on a finite set of machines. Each job is characterized by a
fixed order of operations, each of which is to be processed on a
specific machine for a specified duration.  Each machine can process
at most one operation at a time and once an operation initiates
processing on a given machine it must complete processing
uninterrupted.  The objective of the problem is to find a schedule
that minimizes the makespan of the schedule.

------------------------------------------------------------ */

using System;
using System.IO;
using System.Collections.Generic;
using ILOG.CP;
using ILOG.Concert;

namespace SchedJobShop
{
    public class SchedJobShop
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
            String filename = "../../../../examples/data/jobshop_default.data";
            int nbJobs, nbMachines;

            if (args.Length > 0)
                filename = args[0];

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
                    int m, d;
                    m = data.Next();
                    d = data.Next();
                    IIntervalVar ti = cp.IntervalVar(d);
                    machines[m].Add(ti);
                    if (j > 0)
                    {
                        cp.Add(cp.EndBeforeStart(prec, ti));
                    }
                    prec = ti;
                }
                ends.Add(cp.EndOf(prec));
            }

            for (int j = 0; j < nbMachines; j++)
                cp.Add(cp.NoOverlap(machines[j].ToArray()));

            IObjective objective = cp.Minimize(cp.Max(ends.ToArray()));
            cp.Add(objective);

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
