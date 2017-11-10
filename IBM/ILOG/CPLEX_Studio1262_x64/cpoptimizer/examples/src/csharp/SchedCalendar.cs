// -----------------------------------------------------------------*- C# -*-
// File: ./examples/src/csharp/SchedCalendar.cs
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
painting, etc. must be scheduled.  Some tasks must necessarily take
place before others and these requirements are expressed through
precedence constraints.

There are two workers and each task requires a specific worker.  The
worker has a calendar of days off that must be taken into account. The
objective is to minimize the overall completion date.

------------------------------------------------------------ */

using System;
using System.IO;
using System.Collections.Generic;
using ILOG.CP;
using ILOG.Concert;

namespace SchedCalendar
{
    public class SchedCalendar
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

        public static void MakeHouse(
            CP cp,
            int id,
            List<IIntExpr> ends,
            List<IIntervalVar> allTasks,
            List<IIntervalVar> joeTasks,
            List<IIntervalVar> jimTasks
            )
        {
            /// CREATE THE TIME-INTERVALS. ///
            String name;
            IIntervalVar[] tasks = new IIntervalVar[nbTasks];
            for (int i = 0; i < nbTasks; i++)
            {
                name = "H" + id + "-" + taskNames[i];
                tasks[i] = cp.IntervalVar(taskDurations[i], name);
                allTasks.Add(tasks[i]);
            }

            /// ADDING PRECEDENCE CONSTRAINTS. ///
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

            /// ADDING WORKER TASKS. ///
            joeTasks.Add(tasks[masonry]);
            joeTasks.Add(tasks[carpentry]);
            jimTasks.Add(tasks[plumbing]);
            jimTasks.Add(tasks[ceiling]);
            joeTasks.Add(tasks[roofing]);
            jimTasks.Add(tasks[painting]);
            jimTasks.Add(tasks[windows]);
            joeTasks.Add(tasks[facade]);
            joeTasks.Add(tasks[garden]);
            jimTasks.Add(tasks[moving]);

            /// DEFINING MINIMIZATION OBJECTIVE ///
            ends.Add(cp.EndOf(tasks[moving]));
        }

        public static void Main(String[] args)
        {
            CP cp = new CP();

            int nbHouses = 5;
            List<IIntExpr> ends = new List<IIntExpr>();
            List<IIntervalVar> allTasks = new List<IIntervalVar>();
            List<IIntervalVar> joeTasks = new List<IIntervalVar>();
            List<IIntervalVar> jimTasks = new List<IIntervalVar>();

            for (int h = 0; h < nbHouses; h++)
            {
                MakeHouse(cp, h, ends, allTasks, joeTasks, jimTasks);
            }

            IIntervalVar[] test = joeTasks.ToArray();
            IConstraint cont = cp.NoOverlap(test);

            cp.Add(cp.NoOverlap(joeTasks.ToArray()));
            cp.Add(cp.NoOverlap(jimTasks.ToArray()));

            INumToNumStepFunction joeCalendar = cp.NumToNumStepFunction();
            joeCalendar.SetValue(0, 2 * 365, 100);
            INumToNumStepFunction jimCalendar = cp.NumToNumStepFunction();
            jimCalendar.SetValue(0, 2 * 365, 100);

            // Week ends
            for (int w = 0; w < 2 * 52; w++)
            {
                joeCalendar.SetValue(5 + (7 * w), 7 + (7 * w), 0);
                jimCalendar.SetValue(5 + (7 * w), 7 + (7 * w), 0);
            }
            // Holidays
            joeCalendar.SetValue(5, 12, 0);
            joeCalendar.SetValue(124, 131, 0);
            joeCalendar.SetValue(215, 236, 0);
            joeCalendar.SetValue(369, 376, 0);
            joeCalendar.SetValue(495, 502, 0);
            joeCalendar.SetValue(579, 600, 0);
            jimCalendar.SetValue(26, 40, 0);
            jimCalendar.SetValue(201, 225, 0);
            jimCalendar.SetValue(306, 313, 0);
            jimCalendar.SetValue(397, 411, 0);
            jimCalendar.SetValue(565, 579, 0);

            for (int i = 0; i < joeTasks.Count; i++)
            {
                joeTasks[i].SetIntensity(joeCalendar, 100);
                cp.Add(cp.ForbidStart(joeTasks[i], joeCalendar));
                cp.Add(cp.ForbidEnd(joeTasks[i], joeCalendar));
            }
            for (int i = 0; i < jimTasks.Count; i++)
            {
                jimTasks[i].SetIntensity(jimCalendar, 100);
                cp.Add(cp.ForbidStart(jimTasks[i], jimCalendar));
                cp.Add(cp.ForbidEnd(jimTasks[i], jimCalendar));
            }

            cp.Add(cp.Minimize(cp.Max(ends.ToArray())));

            /// EXTRACTING THE MODEL AND SOLVING.///
            cp.SetParameter(CP.IntParam.FailLimit, 10000);
            if (cp.Solve())
            {
                for (int i = 0; i < allTasks.Count; i++)
                {
                    Console.WriteLine(cp.GetDomain(allTasks[i]));
                }
            }
            else
            {
                Console.WriteLine("No Solution found.");
            }
        }
    }
}

