// -----------------------------------------------------------------*- C# -*-
// File: ./examples/src/csharp/SchedSetup.cs
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

This example solves a scheduling problem on two alternative
heterogeneous machines. A set of tasks {a_1,...,a_n} has to be
executed on either one of the two machines. Different types of tasks
are distinguished, the type of task a_i is denoted tp_i.  

A machine m needs a sequence dependent setup time setup(tp,tp') to
switch from a task of type tp to the next task of type
tp'. Furthermore some transitions tp->tp' are forbidden.

The two machines are different: they process tasks with different
speed and have different setup times and forbidden transitions.

The objective is to minimize the makespan.

The model uses transition distances and noOverlap constraints to model
machines setup times. The noOverlap constraint is specified to enforce
transition distance between immediate successors on the
sequence. Forbidden transitions are modeled with a very large
transition distance.

------------------------------------------------------------ */

using System;
using ILOG.CP;
using ILOG.Concert;

namespace SchedSetup
{
    public class SchedSetup
    {

        const int NbTypes = 5;

        // Setup times of machine M1; -1 means forbidden transition
        static int[] SetupM1 = {
        0,  26,  8,  3, -1,
        22,  0, -1,  4, 22,
        28,  0,  0, 23,  9,
        29, -1, -1,  0,  8,
        26, 17, 11,  7,  0
        };

        // Setup times of machine M2; -1 means forbidden transition
        static int[] SetupM2 = {
        0,  5, 28, -1,  2,
        -1, 0, -1,  7, 10,
        19, 22,  0, 28, 17,
        7, 26, 13,  0, -1,
        13, 17, 26, 20, 0
        };

        const int NbTasks = 50;

        // Task type
        static int[] TaskType = {
        3, 3, 1, 1, 1, 1, 2, 0, 0, 2,
        4, 4, 3, 3, 2, 3, 1, 4, 4, 2,
        2, 1, 4, 2, 2, 0, 3, 3, 2, 1,
        2, 1, 4, 3, 3, 0, 2, 0, 0, 3,
        2, 0, 3, 2, 2, 4, 1, 2, 4, 3
        };

        // Task duration if executed on machine M1
        static int[] TaskDurM1 = {
        4, 17,  4,  7, 17, 14,  2, 14,  2,  8,
        11, 14,  4, 18,  3,  2,  9,  2,  9, 17,
        18, 19,  5,  8, 19, 12, 17, 11,  6,  3,
        13,  6, 19,  7,  1,  3, 13,  5,  3,  6,
        11, 16, 12, 14, 12, 17,  8,  8,  6,  6
        };

        // Task duration if executed on machine M2
        static int[] TaskDurM2 = {
        12,  3, 12, 15,  4,  9, 14,  2,  5,  9,
        10, 14,  7,  1, 11,  3, 15, 19,  8,  2,
        18, 17, 19, 18, 15, 14,  6,  6,  1,  2,
        3, 19, 18,  2,  7, 16,  1, 18, 10, 14,
        2,  3, 14,  1,  1,  6, 19,  5, 17,  4
        };

    public static void Main(String[] args)
        {
            try
            {
                CP cp = new CP();
                ITransitionDistance setup1 = cp.TransitionDistance(NbTypes);
                ITransitionDistance setup2 = cp.TransitionDistance(NbTypes);
                int i, j;
                for (i = 0; i < NbTypes; ++i)
                {
                    for (j = 0; j < NbTypes; ++j)
                    {
                        int d1 = SetupM1[NbTypes * i + j];
                        if (d1 < 0)
                            d1 = CP.IntervalMax; // Forbidden transition
                        setup1.SetValue(i, j, d1);
                        int d2 = SetupM2[NbTypes * i + j];
                        if (d2 < 0)
                            d2 = CP.IntervalMax; // Forbidden transition
                        setup2.SetValue(i, j, d2);
                    }
                }
                int[] tp = new int[NbTasks];
                IIntervalVar[] a = new IIntervalVar[NbTasks];
                IIntervalVar[] a1 = new IIntervalVar[NbTasks];
                IIntervalVar[] a2 = new IIntervalVar[NbTasks];
                IIntExpr[] ends = new IIntExpr[NbTasks];

                String name;
                for (i = 0; i < NbTasks; ++i)
                {
                    int type = TaskType[i];
                    int d1 = TaskDurM1[i];
                    int d2 = TaskDurM2[i];
                    tp[i] = type;
                    name = "A" + i + "_TP" + type;
                    a[i] = cp.IntervalVar(name);
                    IIntervalVar[] alt = new IIntervalVar[2];
                    name = "A" + i + "_M1_TP" + type;
                    a1[i] = cp.IntervalVar(d1, name);
                    a1[i].SetOptional();
                    alt[0] = a1[i];
                    name = "A" + i + "_M2_TP" + type;
                    a2[i] = cp.IntervalVar(d2, name);
                    a2[i].SetOptional();
                    alt[1] = a2[i];
                    cp.Add(cp.Alternative(a[i], alt));
                    ends[i] = cp.EndOf(a[i]);
                }

                IIntervalSequenceVar s1 = cp.IntervalSequenceVar(a1, tp);
                IIntervalSequenceVar s2 = cp.IntervalSequenceVar(a2, tp);
                cp.Add(cp.NoOverlap(s1, setup1, true));
                cp.Add(cp.NoOverlap(s2, setup2, true));
                cp.Add(cp.Minimize(cp.Max(ends)));

                cp.SetParameter(CP.IntParam.FailLimit, 100000);
                cp.SetParameter(CP.IntParam.LogPeriod, 10000);
                if (cp.Solve())
                {
                    Console.WriteLine("Machine 1: ");
                    IIntervalVar x;
                    for (x = cp.GetFirst(s1); !x.Equals(cp.GetLast(s1)); x = cp.GetNext(s1, x))
                        Console.WriteLine(cp.GetDomain(x));
                    Console.WriteLine(cp.GetDomain(x));
                    Console.WriteLine("Machine 2: ");
                    for (x = cp.GetFirst(s2); !x.Equals(cp.GetLast(s2)); x = cp.GetNext(s2, x))
                        Console.WriteLine(cp.GetDomain(x));
                    Console.WriteLine(cp.GetDomain(x));
                    Console.WriteLine("Makespan \t: " + cp.ObjValue);
                }
                else
                {
                    Console.WriteLine("No solution found.");
                }
            }
            catch (IloException e)
            {
                Console.WriteLine("Error: " + e);
            }
        }
    }
}
