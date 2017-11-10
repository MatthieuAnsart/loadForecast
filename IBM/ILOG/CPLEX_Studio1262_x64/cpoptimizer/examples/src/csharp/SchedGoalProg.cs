// -----------------------------------------------------------------*- C# -*-
// File: ./examples/src/csharp/SchedGoalProg.cs
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

This example solves a multi-machine assignment and scheduling problem
([1]), where jobs, with release dates and deadlines, have to be
processed on parallel unrelated machines where processing times depend
on machine assignment. Given a job/machine assignment cost matrix, the
objective is to minimize the total cost while keeping all machine
schedules feasible.

In the instances used in [1], two types of machines can be
distinguished: expensive machines (whose cost for a job equals 1000)
and regular machines (whose cost is much lower, typically less than
50). This example implements a two-steps approach:

- In a first step, the problem is solved by taking all the constraints
into account but neglecting the cost of regular machines and focusing
on minimizing the cost of allocating jobs to expensive machines. 

- A solution to the above problem is also a solution to the main
problem. In a second step, this solution is used as starting point
for solving the main problem.

This two-steps approach permits to focus in the first phase on the
most important part of the cost (the allocation of job to expensive
machines) and, once a good (or optimal) solution has been found, to
use this solution as a starting point for minimizing the other part of
the cost. This approach can be used in most problems where the
components of the objective function are lexically ordered.

By default, the program executes the two-steps approach. If the second
argument (useGoalProgramming) is set to 0, the program executes a
single-step approach. With the same global limit, results with the
two-steps approach are better than the ones of a single-step approach.

[1] R. Sadykov and L. Wolsey. Integer programming and constraint
programming in solving a multi-machine assignment scheduling problem
with deadlines and release dates.  INFORMS Journal on Computing. 2006.

------------------------------------------------------------ */

using ILOG.Concert;
using ILOG.CP;
using System;
using System.IO;


namespace SchedGoalProg
{
    public class SchedGoalProg
    {
        private int nbMachines, nbJobs;
        private IIntervalVar[][] machines;
        private int[][] costs;
        private CP cp;

        public SchedGoalProg(String fileName)
        {
            cp = new CP();
            CreateModel(fileName);
        }

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

        private void CreateModel(String dataFile)
        {

            DataReader data = new DataReader(dataFile);
            nbJobs = data.Next();
            nbMachines = data.Next();

            int[] rel = new int[nbJobs];
            int[] due = new int[nbJobs];

            costs = new int[nbMachines][];
            int[][] dur = new int[nbMachines][];
            for (int j = 0; j < nbMachines; j++)
            {
                costs[j] = new int[nbJobs];
                dur[j] = new int[nbJobs];
            }

            for (int i = 0; i < nbJobs; i++)
                rel[i] = data.Next();

            for (int i = 0; i < nbJobs; i++)
                due[i] = data.Next();

            for (int j = 0; j < nbMachines; j++)
                for (int i = 0; i < nbJobs; i++)
                    costs[j][i] = data.Next();

            for (int j = 0; j < nbMachines; j++)
                for (int i = 0; i < nbJobs; i++)
                    dur[j][i] = data.Next();

            machines = new IIntervalVar[nbMachines][];
            for (int j = 0; j < nbMachines; j++)
                machines[j] = new IIntervalVar[nbJobs];
            for (int i = 0; i < nbJobs; i++)
            {
                IIntervalVar job = cp.IntervalVar();
                job.StartMin = rel[i];
                job.EndMax = due[i];
                job.Name = "Op" + i;
                IIntervalVar[] jobm = new IIntervalVar[nbMachines];
                for (int j = 0; j < nbMachines; j++)
                {
                    jobm[j] = cp.IntervalVar(dur[j][i]);
                    jobm[j].Name = "Alt" + i + "_" + j + "_C" + costs[j][i];
                    jobm[j].SetOptional();
                    machines[j][i] = jobm[j];
                }
                cp.Add(cp.Alternative(job, jobm));
            }
            for (int j = 0; j < nbMachines; j++)
                cp.Add(cp.NoOverlap(machines[j]));

        }

        public double SolveGoalProgramming(int branchLimit)
        {

            // Objective 1
            Console.WriteLine();
            Console.WriteLine(" ! ----------------------------------------------------------------------------");
            Console.WriteLine(" ! STEP 1: Minimizing usage of expensive resources");
            Console.WriteLine(" ! ----------------------------------------------------------------------------");
            IIntExpr costExpr = cp.IntExpr();
            for (int i = 0; i < nbJobs; i++)
            {
                for (int j = 0; j < nbMachines; j++)
                {
                    if (costs[j][i] >= 1000)
                    {
                        costExpr = cp.Sum(costExpr, cp.Prod(costs[j][i], cp.PresenceOf(machines[j][i])));
                    }
                }
            }
            IObjective obj1 = cp.Minimize(costExpr);
            cp.Add(obj1);

            cp.SetParameter(CP.IntParam.LogPeriod, 600000);
            cp.SetParameter(CP.IntParam.BranchLimit, 2 * branchLimit / 3);
            cp.SetParameter(CP.IntParam.NoOverlapInferenceLevel, CP.ParameterValues.Extended);
            cp.Solve();

            ISolution startSol = cp.Solution();
            for (int i = 0; i < nbJobs; i++)
            {
                for (int j = 0; j < nbMachines; j++)
                {
                    if (cp.IsPresent(machines[j][i])) {
                        startSol.SetPresent(machines[j][i]);
                        startSol.SetStart(machines[j][i], cp.GetStart(machines[j][i]));
                    }
                }
            }

            // Objective 2
            Console.WriteLine();
            Console.WriteLine(" ! ----------------------------------------------------------------------------");
            Console.WriteLine(" ! STEP 2: Minimizing total cost");
            Console.WriteLine(" ! ----------------------------------------------------------------------------");
            cp.Remove(obj1);
            IIntExpr costExpr2 = cp.IntExpr();
            for (int i = 0; i < nbJobs; i++)
            {
                for (int j = 0; j < nbMachines; j++)
                {
                    costExpr2 = cp.Sum(costExpr2, cp.Prod(costs[j][i], cp.PresenceOf(machines[j][i])));
                }
            }
            IObjective obj2 = cp.Minimize(costExpr2);
            cp.Add(obj2);
            cp.SetParameter(CP.IntParam.BranchLimit, branchLimit / 3);
            cp.SetStartingPoint(startSol);
            cp.Solve();
            double cost = cp.ObjValue;
            return cost;
        }

        public double SolveBasic(int branchLimit)
        {

            // Objective
            Console.WriteLine();
            Console.WriteLine(" ! ----------------------------------------------------------------------------");
            Console.WriteLine(" ! Minimizing total cost");
            Console.WriteLine(" ! ----------------------------------------------------------------------------");
            IIntExpr costExpr = cp.IntExpr();
            int nbMachines = machines.Length;
            int nbJobs = machines[0].Length;
            for (int i = 0; i < nbJobs; i++)
            {
                for (int j = 0; j < nbMachines; j++)
                {
                    costExpr = cp.Sum(costExpr, cp.Prod(costs[j][i], cp.PresenceOf(machines[j][i])));
                }
            }
            IObjective obj = cp.Minimize(costExpr);
            cp.Add(obj);

            cp.SetParameter(CP.IntParam.LogPeriod, 600000);
            cp.SetParameter(CP.IntParam.BranchLimit, branchLimit);
            cp.SetParameter(CP.IntParam.NoOverlapInferenceLevel, CP.ParameterValues.Extended);
            cp.Solve();
            double cost = cp.ObjValue;
            return cost;
        }

        public static void Main(String[] args)
        {

            String fileName = "../../../../examples/data/goalprog_8_40_0.6_3.data";
            bool useGoalProgramming = true;
            int branchLimit = 300000;

            if (args.Length > 0)
                fileName = args[0];
            if (args.Length > 1)
                useGoalProgramming = Convert.ToBoolean(args[1]);
            if (args.Length > 3)
                branchLimit = Convert.ToInt32(args[2]);

            try
            {
                double cost = 0;
                Console.WriteLine("Data file: " + fileName);
                SchedGoalProg gp = new SchedGoalProg(fileName);
                if (useGoalProgramming)
                {
                    Console.WriteLine("Solving in two steps using goal programming ...");
                    cost = gp.SolveGoalProgramming(branchLimit);
                }
                else
                {
                    Console.WriteLine("Solving in a single step  ...");
                    cost = gp.SolveBasic(branchLimit);
                }

                Console.WriteLine();
                Console.WriteLine(" ! ----------------------------------------------------------------------------");
                Console.WriteLine(" ! Cost= " + cost);
                Console.WriteLine(" ! ----------------------------------------------------------------------------");
            }
            catch (ILOG.Concert.Exception e)
            {
                Console.WriteLine("ERROR:" + e);
            }
        }



    }
}
