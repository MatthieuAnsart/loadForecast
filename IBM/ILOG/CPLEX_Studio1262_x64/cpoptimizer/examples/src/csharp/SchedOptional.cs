// -----------------------------------------------------------------*- C# -*-
// File: ./examples/src/csharp/SchedOptional.cs
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

There are three workers, and each worker has a given non-negative
skill level for each task.  Each task requires one worker that will
have to be selected among the ones who have a non null skill level for
that task.  A worker can be assigned to only one task at a time.  Each
house has a deadline. The objective is to maximize the skill levels of
the workers assigned to the tasks while respecting the deadlines.

------------------------------------------------------------ */


using System;
using System.IO;
using System.Collections.Generic;
using ILOG.CP;
using ILOG.Concert;

namespace SchedOptional
{
    public class SchedOptional
    {

        const int nbWorkers = 3;
        const int nbTasks = 10;

        const int joe = 0;
        const int jack = 1;
        const int jim = 2;

        static String[] workerNames = {
        "Joe",
        "Jack",
        "Jim"
    };

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
        "masonry",
        "carpentry",
        "plumbing",
        "ceiling",
        "roofing",
        "painting",
        "windows",
        "facade",
        "garden",
        "moving"
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
        05
    };

        static int[] skillsMatrix = {
        // Joe, Jack, Jim
        9, 5, 0, // masonry
        7, 0, 5, // carpentry
        0, 7, 0, // plumbing
        5, 8, 0, // ceiling
        6, 7, 0, // roofing
        0, 9, 6, // painting
        8, 0, 5, // windows
        5, 5, 0, // facade
        5, 5, 9, // garden
        6, 0, 8  // moving
    };

        public static bool HasSkill(int w, int i)
        {
            return (0 < skillsMatrix[nbWorkers * i + w]);
        }

        public static int SkillLevel(int w, int i)
        {
            return skillsMatrix[nbWorkers * i + w];
        }

        static CP cp = new CP();

        static INumExpr skill;

        public static void MakeHouse(List<IIntervalVar> allTasks,
                                     List<IIntervalVar>[] workerTasks,
                                     int id,
                                     int deadline)
        {

            /* CREATE THE INTERVAL VARIABLES. */
            String name;
            IIntervalVar[] tasks = new IIntervalVar[nbTasks];
            IIntervalVar[,] taskMatrix = new IIntervalVar[nbTasks, nbWorkers];

            for (int i = 0; i < nbTasks; i++)
            {
                name = "H" + id + "-" + taskNames[i];
                tasks[i] = cp.IntervalVar(taskDurations[i], name);

                /* ALLOCATING TASKS TO WORKERS. */
                List<IIntervalVar> alttasks = new List<IIntervalVar>();
                for (int w = 0; w < nbWorkers; w++)
                {
                    if (HasSkill(w, i))
                    {
                        name = "H" + id + "-" + taskNames[i] + "-" + workerNames[w];
                        IIntervalVar wtask = cp.IntervalVar(taskDurations[i], name);
                        wtask.SetOptional();
                        alttasks.Add(wtask);
                        taskMatrix[i, w] = wtask;
                        workerTasks[w].Add(wtask);
                        allTasks.Add(wtask);
                        /* DEFINING MAXIMIZATION OBJECTIVE. */
                        skill = cp.Sum(skill, cp.Prod(SkillLevel(w, i), cp.PresenceOf(wtask)));
                    }
                }
                cp.Add(cp.Alternative(tasks[i], alttasks.ToArray()));
            }

            /* ADDING PRECEDENCE CONSTRAINTS. */
            tasks[moving].EndMax = deadline;
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

            /* ADDING SAME-WORKER CONSTRAINTS. */
            cp.Add(cp.Add(cp.Equiv(cp.PresenceOf(taskMatrix[masonry, joe]),
                       cp.PresenceOf(taskMatrix[carpentry, joe]))));
            cp.Add(cp.Add(cp.Equiv(cp.PresenceOf(taskMatrix[roofing, jack]),
                       cp.PresenceOf(taskMatrix[facade, jack]))));
            cp.Add(cp.Add(cp.Equiv(cp.PresenceOf(taskMatrix[carpentry, joe]),
                       cp.PresenceOf(taskMatrix[roofing, joe]))));
            cp.Add(cp.Add(cp.Equiv(cp.PresenceOf(taskMatrix[garden, jim]),
                       cp.PresenceOf(taskMatrix[moving, jim]))));
        }

        public static void Main(String[] args)
        {

            int nbHouses = 5;
            int deadline = 318;
            skill = cp.IntExpr();
            List<IIntervalVar> allTasks = new List<IIntervalVar>();
            List<IIntervalVar>[] workerTasks = new List<IIntervalVar>[nbWorkers];
            for (int w = 0; w < nbWorkers; w++)
            {
                workerTasks[w] = new List<IIntervalVar>();
            }

            for (int h = 0; h < nbHouses; h++)
            {
                MakeHouse(allTasks, workerTasks, h, deadline);
            }

            for (int w = 0; w < nbWorkers; w++)
            {
                IIntervalSequenceVar seq = cp.IntervalSequenceVar(workerTasks[w].ToArray(), workerNames[w]);
                cp.Add(cp.NoOverlap(seq));
            }

            cp.Add(cp.Maximize(skill));

            /* EXTRACTING THE MODEL AND SOLVING. */
            cp.SetParameter(CP.IntParam.FailLimit, 10000);
            if (cp.Solve())
            {
                Console.WriteLine("Solution with objective " + cp.ObjValue + ":");
                for (int i = 0; i < allTasks.Count; i++)
                {
                    IIntervalVar var = (IIntervalVar)allTasks[i];
                    if (cp.IsPresent(allTasks[i]))
                        Console.WriteLine(cp.GetDomain((IIntervalVar)allTasks[i]));
                }
            }
            else
            {
                Console.WriteLine("No solution found. ");
            }
        }
    }
}
