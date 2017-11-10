// -----------------------------------------------------------------*- C# -*-
// File: ./examples/src/csharp/SchedState.cs
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

This is a problem of building five houses. The masonry, roofing,
painting, etc. must be scheduled. Some tasks must necessarily take
place before others and these requirements are expressed through
precedence constraints.

A pool of two workers is available for building the houses. For a
given house, some tasks (namely: plumbing, ceiling and painting)
require the house to be clean whereas other tasks (namely: masonry,
carpentry, roofing and windows) put the house in a dirty state. A
transition time of 1 is needed to clean the house so to change from
state 'dirty' to state 'clean'.

The objective is to minimize the makespan.

------------------------------------------------------------ */

using System;
using System.IO;
using System.Collections.Generic;
using ILOG.CP;
using ILOG.Concert;

namespace SchedState
{
    public class SchedState
    {
        const int nbHouses = 5;
        const int nbTasks = 10;
        const int nbWorkers = 2;

        // Possible house state
        const int clean = 0;
        const int dirty = 1;

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

        static ICumulFunctionExpr workers;

        public static void MakeHouse(CP cp,
                                     int id,
                                     List<IIntExpr> ends,
                                     List<IIntervalVar> allTasks,
                                     IStateFunction houseState)
        {

            /* CREATE THE TIME-INTERVALS. */
            String name;
            IIntervalVar[] tasks = new IIntervalVar[nbTasks];
            for (int i = 0; i < nbTasks; i++)
            {
                name = "H" + id + "-" + taskNames[i];
                tasks[i] = cp.IntervalVar(taskDurations[i], name);
                workers = cp.Sum(workers, cp.Pulse(tasks[i], 1));
                allTasks.Add(tasks[i]);
            }

            /* ADDING PRECEDENCE CONSTRAINTS */
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

            /* ADDING STATE CONSTRAINTS */
            cp.Add(cp.AlwaysEqual(houseState, tasks[masonry], dirty));
            cp.Add(cp.AlwaysEqual(houseState, tasks[carpentry], dirty));
            cp.Add(cp.AlwaysEqual(houseState, tasks[plumbing], clean));
            cp.Add(cp.AlwaysEqual(houseState, tasks[ceiling], clean));
            cp.Add(cp.AlwaysEqual(houseState, tasks[roofing], dirty));
            cp.Add(cp.AlwaysEqual(houseState, tasks[painting], clean));
            cp.Add(cp.AlwaysEqual(houseState, tasks[windows], dirty));


            /* MAKESPAN */
            ends.Add(cp.EndOf(tasks[moving]));
        }

        public static void Main(String[] args)
        {
            CP cp = new CP();
            List<IIntExpr> ends = new List<IIntExpr>();
            List<IIntervalVar> allTasks = new List<IIntervalVar>();
            ITransitionDistance ttime = cp.TransitionDistance(2);
            ttime.SetValue(dirty, clean, 1);
            workers = cp.CumulFunctionExpr();
            IStateFunction[] houseState = new IStateFunction[nbHouses];
            for (int i = 0; i < nbHouses; i++) {
                houseState[i] = cp.StateFunction(ttime);
                MakeHouse(cp, i, ends, allTasks, houseState[i]);
            }
            cp.Add(cp.Le(workers, nbWorkers));
            cp.Add(cp.Minimize(cp.Max(ends.ToArray())));

            /* EXTRACTING THE MODEL AND SOLVING. */
            cp.SetParameter(CP.IntParam.FailLimit, 10000);
            if (cp.Solve()) {
                for (int i = 0; i < allTasks.Count; i++) {
                    Console.WriteLine(cp.GetDomain(allTasks[i]));
                }
                for (int h = 0; h < nbHouses; h++) {
                    for (int i = 0; i < cp.GetNumberOfSegments(houseState[h]); i++) {
                    Console.Write("House " + h + " has state ");
                    int s = cp.GetSegmentValue(houseState[h], i);
                    if (s == clean)                 Console.Write("Clean");
                    else if (s == dirty)            Console.Write("Dirty");
                    else if (s == CP.NoState)       Console.Write("None");
                    else                            Console.Write("Unknown (problem)");
                    Console.Write(" from ");
                    if (cp.GetSegmentStart(houseState[h], i) == CP.IntervalMin)  Console.Write("Min");
                    else Console.Write(cp.GetSegmentStart(houseState[h], i));
                    Console.Write(" to ");
                    if (cp.GetSegmentEnd(houseState[h], i) == CP.IntervalMax)    Console.WriteLine("Max");
                    else Console.WriteLine((cp.GetSegmentEnd(houseState[h], i) - 1));
                    }
                }
            } else {
                Console.WriteLine("No solution found. ");
            }
        }
    }
}

