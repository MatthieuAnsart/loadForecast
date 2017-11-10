// -----------------------------------------------------------------*- C# -*-
// File: ./examples/src/csharp/SchedSequence.cs
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

There are two workers, and each task requires a specific worker.  The
time required for the workers to travel between houses must be taken
into account.  

Moreover, there are tardiness costs associated with some tasks as well
as a cost associated with the length of time it takes to build each
house.  The objective is to minimize these costs.

------------------------------------------------------------ */

using System;
using System.IO;
using System.Collections.Generic;
using ILOG.CP;
using ILOG.Concert;

namespace SchedSequence
{
    public class SchedSequence
    {
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
        static INumExpr cost = cp.NumExpr();

        public static INumExpr tardinessCost(IIntervalVar task, int dd, double weight)
        {
            return cp.Prod(weight, cp.Max(0, cp.Diff(cp.EndOf(task), dd)));
        }

    public static void MakeHouse (
        List<IIntervalVar> allTasks,
        List<IIntervalVar> joeTasks,
        List<IIntervalVar> jimTasks,
        List<Int32> joeLocations,
        List<Int32> jimLocations,
        int loc,
        int rd,
        int dd,
        double weight)
 {

        /* CREATE THE TIME-INTERVALS. */
        String name = "H" + loc;

        IIntervalVar[] tasks = new IIntervalVar[nbTasks];
        for (int i = 0; i < nbTasks; i++ ) {
            name = "H" + loc + "-" + taskNames[i];
            tasks[i] = cp.IntervalVar(taskDurations[i], name);
            allTasks.Add(tasks[i]);
        }

        /* SPAN CONSTRAINT */
        IIntervalVar house = cp.IntervalVar(name);
        cp.Add(cp.Span(house, tasks));

        /* ADDING TEMPORAL CONSTRAINTS. */
        house.StartMin = rd;
        cp.Add(cp.EndBeforeStart(tasks[masonry],   tasks[carpentry]));
        cp.Add(cp.EndBeforeStart(tasks[masonry],   tasks[plumbing]));
        cp.Add(cp.EndBeforeStart(tasks[masonry],   tasks[ceiling]));
        cp.Add(cp.EndBeforeStart(tasks[carpentry], tasks[roofing]));
        cp.Add(cp.EndBeforeStart(tasks[ceiling],   tasks[painting]));
        cp.Add(cp.EndBeforeStart(tasks[roofing],   tasks[windows]));
        cp.Add(cp.EndBeforeStart(tasks[roofing],   tasks[facade]));
        cp.Add(cp.EndBeforeStart(tasks[plumbing],  tasks[facade]));
        cp.Add(cp.EndBeforeStart(tasks[roofing],   tasks[garden]));
        cp.Add(cp.EndBeforeStart(tasks[plumbing],  tasks[garden]));
        cp.Add(cp.EndBeforeStart(tasks[windows],   tasks[moving]));
        cp.Add(cp.EndBeforeStart(tasks[facade],    tasks[moving]));
        cp.Add(cp.EndBeforeStart(tasks[garden],    tasks[moving]));
        cp.Add(cp.EndBeforeStart(tasks[painting],  tasks[moving]));

        /* ALLOCATING TASKS TO WORKERS */
        joeTasks.Add(tasks[masonry]);
        joeLocations.Add(loc);
        joeTasks.Add(tasks[carpentry]);
        joeLocations.Add(loc);
        jimTasks.Add(tasks[plumbing]);
        jimLocations.Add(loc);
        jimTasks.Add(tasks[ceiling]);
        jimLocations.Add(loc);
        joeTasks.Add(tasks[roofing]);
        joeLocations.Add(loc);
        jimTasks.Add(tasks[painting]);
        jimLocations.Add(loc);
        jimTasks.Add(tasks[windows]);
        jimLocations.Add(loc);
        joeTasks.Add(tasks[facade]);
        joeLocations.Add(loc);
        joeTasks.Add(tasks[garden]);
        joeLocations.Add(loc);
        jimTasks.Add(tasks[moving]);
        jimLocations.Add(loc);

        /* DEFINING MINIMIZATION OBJECTIVE */
        cost = cp.Sum(cost, tardinessCost(house, dd, weight));
        cost = cp.Sum(cost, cp.LengthOf(house));
    }
        
        public static void Main(String[] args)
        {
        try {
            cp = new CP();
            cost = cp.NumExpr();
            List<IIntervalVar> allTasks = new List<IIntervalVar>();
            List<IIntervalVar> joeTasks = new List<IIntervalVar>();
            List<IIntervalVar> jimTasks = new List<IIntervalVar>();

            List<Int32> joeLocations = new List<Int32>();
            List<Int32> jimLocations = new List<Int32>();

            MakeHouse( allTasks, joeTasks, jimTasks, joeLocations, jimLocations, 0, 0,   120, 100.0);
            MakeHouse( allTasks, joeTasks, jimTasks, joeLocations, jimLocations, 1, 0,   212, 100.0);
            MakeHouse( allTasks, joeTasks, jimTasks, joeLocations, jimLocations, 2, 151, 304, 100.0);
            MakeHouse( allTasks, joeTasks, jimTasks, joeLocations, jimLocations, 3, 59,  181, 200.0);
            MakeHouse( allTasks, joeTasks, jimTasks, joeLocations, jimLocations, 4, 243, 425, 100.0);

            ITransitionDistance tt = cp.TransitionDistance(5);
            for (int i = 0; i < 5; ++i)
                for (int j = 0; j < 5; ++j)
                    tt.SetValue(i, j, Math.Abs(i - j));

            IIntervalSequenceVar joe = cp.IntervalSequenceVar(joeTasks.ToArray(), joeLocations.ToArray(), "Joe");
            IIntervalSequenceVar jim = cp.IntervalSequenceVar(jimTasks.ToArray(), jimLocations.ToArray(), "Jim");

            cp.Add(cp.NoOverlap(joe, tt));
            cp.Add(cp.NoOverlap(jim, tt));

            cp.Add(cp.Minimize(cost));


            cp.SetParameter(CP.IntParam.FailLimit, 50000);
            /* EXTRACTING THE MODEL AND SOLVING. */
            if (cp.Solve()) {
                for (int i = 0; i < allTasks.Count; ++i)
                    Console.WriteLine(cp.GetDomain(allTasks[i]));
            } else {
                Console.WriteLine("No solution found.");
            }
        } catch (ILOG.Concert.Exception e) {
            Console.WriteLine(" ERROR: " + e);
        }
        }
    }
}
