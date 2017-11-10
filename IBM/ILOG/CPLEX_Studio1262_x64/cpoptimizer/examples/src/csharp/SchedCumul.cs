// -----------------------------------------------------------------*- C# -*-
// File: ./examples/src/csharp/SchedCumul.cs
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

This is a problem of building five houses in different locations. The
masonry, roofing, painting, etc. must be scheduled. Some tasks must
necessarily take place before others and these requirements are
expressed through precedence constraints.

There are three workers, and each task requires a worker.  There is
also a cash budget which starts with a given balance.  Each task costs
a given amount of cash per day which must be available at the start of
the task.  A cash payment is received periodically.  The objective is
to minimize the overall completion date.

------------------------------------------------------------ */

using System;
using System.IO;
using System.Collections.Generic;
using ILOG.CP;
using ILOG.Concert;

namespace SchedCumul
{
    public class SchedCumul
    {
        const int nbWorkers = 3;
        const int nbTasks = 10;

        const int masonry = 0;
        const int carpentry = 1;
        const int plumbing = 2;
        const int ceiling = 3;
        const int roofing = 4;
        const int painting = 5;
        const int windows = 6;
        const int facade = 7;
        const int garden = 8;
        const int moving = 9;

        static String[] taskNames = {
        "masonry  ",
        "carpentry",
        "plumbing ",
        "ceiling  ",
        "roofing  ",
        "painting ",
        "windows  ",
        "facade   ",
        "garden   ",
        "moving   "
    };

        static int[] taskDurations = {
        35,
        15,
        40,
        15,
        05,
        10,
        05,
        10,
        05,
        05,
    };

        static CP cp = new CP();

        static ICumulFunctionExpr workersUsage;
        static ICumulFunctionExpr cash;


        public static void MakeHouse(
                    int id,
                    int rd,
                    List<IIntExpr> ends,
                    List<IIntervalVar> allTasks)
        {

            /* CREATE THE TIME-INTERVALS. */
            String name;
            IIntervalVar[] tasks = new IIntervalVar[nbTasks];
            for (int i = 0; i < nbTasks; ++i)
            {
                name = "H" + id + "-" + taskNames[i];
                IIntervalVar task = cp.IntervalVar(taskDurations[i], name);
                tasks[i] = task;
                allTasks.Add(task);
                workersUsage = cp.Sum(workersUsage, cp.Pulse(task, 1));
                cash = cp.Diff(cash, cp.StepAtStart(task, 200 * taskDurations[i]));
            }

            /* ADDING TEMPORAL CONSTRAINTS. */
            tasks[masonry].StartMin = rd;
            cp.Add(cp.EndBeforeStart(tasks[masonry], tasks[carpentry]));
            cp.Add(cp.EndBeforeStart(tasks[masonry], tasks[plumbing]));
            cp.Add(cp.EndBeforeStart(tasks[masonry], tasks[ceiling]));
            cp.Add(cp.EndBeforeStart(tasks[carpentry], tasks[roofing]));
            cp.Add(cp.EndBeforeStart(tasks[ceiling], tasks[painting]));
            cp.Add(cp.EndBeforeStart(tasks[roofing], tasks[windows]));
            cp.Add(cp.EndBeforeStart(tasks[roofing], tasks[facade]));
            cp.Add(cp.EndBeforeStart(tasks[plumbing], tasks[facade]));
            cp.Add(cp.EndBeforeStart(tasks[roofing], tasks[garden]));
            cp.Add(cp.EndBeforeStart(tasks[plumbing], tasks[garden]));
            cp.Add(cp.EndBeforeStart(tasks[windows], tasks[moving]));
            cp.Add(cp.EndBeforeStart(tasks[facade], tasks[moving]));
            cp.Add(cp.EndBeforeStart(tasks[garden], tasks[moving]));
            cp.Add(cp.EndBeforeStart(tasks[painting], tasks[moving]));

            /* DEFINING MINIMIZATION OBJECTIVE */
            ends.Add(cp.EndOf(tasks[moving]));
        }

        public static void Main(String[] args)
        {

            workersUsage = cp.CumulFunctionExpr();
            cash = cp.CumulFunctionExpr();
            List<IIntExpr> ends = new List<IIntExpr>();
            List<IIntervalVar> allTasks = new List<IIntervalVar>();

            /* CASH PAYMENTS */
            for (int p = 0; p < 5; ++p)
                cash.Add(cp.Step(60 * p, 30000));

            MakeHouse(0, 31, ends, allTasks);
            MakeHouse(1, 0, ends, allTasks);
            MakeHouse(2, 90, ends, allTasks);
            MakeHouse(3, 120, ends, allTasks);
            MakeHouse(4, 90, ends, allTasks);

            cp.Add(cp.Le(0, cash));

            cp.Add(cp.Le(workersUsage, nbWorkers));

            cp.Add(cp.Minimize(cp.Max(ends.ToArray())));

            /* EXTRACTING THE MODEL AND SOLVING. */
            cp.SetParameter(CP.IntParam.FailLimit, 10000);
            if (cp.Solve())
            {
                Console.WriteLine("Solution with objective " + cp.ObjValue + ":");
                for (int i = 0; i < allTasks.Count; i++)
                {
                    Console.WriteLine(cp.GetDomain(allTasks[i]));
                }
                int segs = cp.GetNumberOfSegments(cash);
                for (int i = 0; i < segs; i++)
                {
                    Console.WriteLine(
                      "Cash is " + cp.GetSegmentValue(cash, i) +
                      " from " + cp.GetSegmentStart(cash, i) +
                      " to " + (cp.GetSegmentEnd(cash, i) - 1)
                    );
                }
            }
            else
            {
                Console.WriteLine("No Solution found.");
            }
        }
    }
}

