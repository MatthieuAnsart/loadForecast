// -----------------------------------------------------------------*- C# -*-
// File: ./examples/src/csharp/SchedTime.cs
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

This is a problem of building a house. The masonry, roofing, painting,
etc. must be scheduled.  Some tasks must necessarily take place before
others and these requirements are expressed through precedence
constraints.

Moreover, there are earliness and tardiness costs associated with some
tasks. The objective is to minimize these costs.

------------------------------------------------------------ */

using System;
using System.IO;
using System.Collections.Generic;
using ILOG.CP;
using ILOG.Concert;

namespace SchedTime
{
    public class SchedTime
    {

        static CP cp = new CP();

    public static INumExpr EarlinessCost(IIntervalVar task, int rd, double weight, int useFunction) {
        if (useFunction != 0) {
            double[] arrX = {rd};
            double[] arrV = {-weight, 0.0};
            INumToNumSegmentFunction f = cp.PiecewiseLinearFunction(arrX, arrV, rd, 0.0);
            return cp.StartEval(task,f);
        }
        else
        {
            return cp.Prod(weight, cp.Max(0, cp.Diff(rd, cp.StartOf(task))));
        }
    }

    public static INumExpr TardinessCost(IIntervalVar task, int dd, double weight, int useFunction) {
        if (useFunction != 0) {
            double[] arrX = {dd};
            double[] arrV = {0.0, weight};
            INumToNumSegmentFunction f = cp.PiecewiseLinearFunction(arrX, arrV, dd, 0.0);
            return cp.EndEval(task,f);
        }
        else
        {
            return cp.Prod(weight, cp.Max(0, cp.Diff(cp.EndOf(task),dd)));
        }
    }

        public static void Main(String[] args)
        {

            CP cp = new CP();

            /* CREATE THE TIME-INTERVALS. */
            IIntervalVar masonry   = cp.IntervalVar( 35, "masonry   ");
            IIntervalVar carpentry = cp.IntervalVar( 15, "carpentry ");
            IIntervalVar plumbing  = cp.IntervalVar( 40, "plumbing  ");
            IIntervalVar ceiling   = cp.IntervalVar( 15, "ceiling   ");
            IIntervalVar roofing   = cp.IntervalVar(  5, "roofing   ");
            IIntervalVar painting  = cp.IntervalVar( 10, "painting  ");
            IIntervalVar windows   = cp.IntervalVar(  5, "windows   ");
            IIntervalVar facade    = cp.IntervalVar( 10, "facade    ");
            IIntervalVar garden    = cp.IntervalVar(  5, "garden    ");
            IIntervalVar moving    = cp.IntervalVar(  5, "moving    ");

            /* ADDING TEMPORAL CONSTRAINTS. */
            cp.Add(cp.EndBeforeStart(masonry,   carpentry));
            cp.Add(cp.EndBeforeStart(masonry,   plumbing));
            cp.Add(cp.EndBeforeStart(masonry,   ceiling));
            cp.Add(cp.EndBeforeStart(carpentry, roofing));
            cp.Add(cp.EndBeforeStart(ceiling,   painting));
            cp.Add(cp.EndBeforeStart(roofing,   windows));
            cp.Add(cp.EndBeforeStart(roofing,   facade));
            cp.Add(cp.EndBeforeStart(plumbing,  facade));
            cp.Add(cp.EndBeforeStart(roofing,   garden));
            cp.Add(cp.EndBeforeStart(plumbing,  garden));
            cp.Add(cp.EndBeforeStart(windows,   moving));
            cp.Add(cp.EndBeforeStart(facade,    moving));
            cp.Add(cp.EndBeforeStart(garden,    moving));
            cp.Add(cp.EndBeforeStart(painting,  moving));

            /* DEFINING MINIMIZATION OBJECTIVE */
            int useFunction = 1;
            INumExpr cost = cp.NumExpr();

            cost = cp.Sum(cost,EarlinessCost(masonry,   25, 200.0, useFunction));
            cost = cp.Sum(cost,EarlinessCost(carpentry, 75, 300.0, useFunction));
            cost = cp.Sum(cost,EarlinessCost(ceiling,   75, 100.0, useFunction));
            cost = cp.Sum(cost,TardinessCost(moving,   100, 400.0, useFunction));
            cp.Add(cp.Minimize(cost));

            /* SOLVING. */
            if ( cp.Solve() ) {
                Console.WriteLine("Optimal Value: " + cp.ObjValue);
                Console.WriteLine(cp.GetDomain(masonry));
                Console.WriteLine(cp.GetDomain(carpentry));
                Console.WriteLine(cp.GetDomain(plumbing));
                Console.WriteLine(cp.GetDomain(ceiling));
                Console.WriteLine(cp.GetDomain(roofing));
                Console.WriteLine(cp.GetDomain(painting));
                Console.WriteLine(cp.GetDomain(windows));
                Console.WriteLine(cp.GetDomain(facade));
                Console.WriteLine(cp.GetDomain(garden));
                Console.WriteLine(cp.GetDomain(moving));
            } else {
                Console.WriteLine("No solution found. ");
            }
        }
    }
}

/*
Optimal Value: 5000
masonry   [1: 20 -- 35 --> 55]
carpentry [1: 75 -- 15 --> 90]
plumbing  [1: 55 -- 40 --> 95]
ceiling   [1: 75 -- 15 --> 90]
roofing   [1: 90 -- 5 --> 95]
painting  [1: 90 -- 10 --> 100]
windows   [1: 95 -- 5 --> 100]
facade    [1: 95 -- 10 --> 105]
garden    [1: 95 -- 5 --> 100]
moving    [1: 105 -- 5 --> 110]
*/
