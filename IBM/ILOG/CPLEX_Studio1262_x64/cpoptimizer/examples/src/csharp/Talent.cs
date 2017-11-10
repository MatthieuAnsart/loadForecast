// -----------------------------------------------------------------*- C# -*-
// File: ./examples/src/csharp/Talent.cs
// --------------------------------------------------------------------------
// Licensed Materials - Property of IBM
//
// 5724-Y48 5724-Y49 5724-Y54 5724-Y55 5725-A06 5725-A29
// Copyright IBM Corporation 1990, 2014. All Rights Reserved.
//
// Note to U.S. Government Users Restricted Rights:
// Use, duplication or disclosure restricted by GSA ADP Schedule
// Contract with IBM Corp.
// --------------------------------------------------------------------------

/* ------------------------------------------------------------

Problem Description
-------------------

This example is inspired from the talent hold cost scheduling problem
described in:

T.C.E Cheng, J. Diamond, B.M.T. Lin.  Optimal scheduling in film
production to minimize talent holding cost.  Journal of Optimization
Theory and Applications, 79:197-206, 1993.

of which the 'Rehearsal problem' is a specific case:

Barbara M. Smith.  Constraint Programming In Practice: Scheduling
                   a Rehearsal.  Report APES-67-2003, September 2003.

See: http://www.csplib.org/Problems/prob039/


------------------------------------------------------------ */

using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using ILOG.CP;
using ILOG.Concert;

public class Talent
{
    public static CP cp;

    private int numActors, numScenes;
    private int[] actorPay, sceneDuration;
    private IIntSet[] actorInScene;
    private IIntExpr idleCost;
    private IIntVar[] scene;

    public Talent(String fileName)
    {
        cp = new CP();
        this.ReadData(fileName);
        this.BuildModel();
    }

    class DataReader
    {
        private int index = -1;
        private string[] datas;

        public DataReader(String filename)
        {
            StreamReader reader = new StreamReader(filename);
            datas = reader.ReadToEnd().Split((char[])null,StringSplitOptions.RemoveEmptyEntries);
        }

        public int Next()
        {
            index++;
            return Convert.ToInt32(datas[index]);
        }
    }

    private void ReadData(String fileName)
    {
        DataReader data = new DataReader(fileName);

        numActors = data.Next();
        actorPay = new int[numActors];
        for (int a = 0; a < numActors; a++)
            actorPay[a] = data.Next();

        numScenes = data.Next();
        sceneDuration = new int[numScenes];
        for (int s = 0; s < numScenes; s++)
            sceneDuration[s] = data.Next();

        actorInScene = new IIntSet[numActors];
        for (int a = 0; a < numActors; a++)
        {
            int[] inScene = new int[numScenes];
            int nbScene = 0;
            for (int s = 0; s < numScenes; s++)
            {
                inScene[s] = data.Next();
                if (inScene[s] != 0)
                    nbScene++;
            }
            int[] playScene = new int[nbScene];
            int n = 0;
            for (int s = 0; s < numScenes; s++)
            {
                if (inScene[s] != 0)
                {
                    playScene[n] = s;
                    n++;
                }
            }
            actorInScene[a] = cp.IntSet(playScene);
        }
    }

    private void BuildModel()
    {
        // Create the decision variables, cost, and the model
        scene = new IIntVar[numScenes];
        for (int s = 0; s < numScenes; s++)
            scene[s] = cp.IntVar(0, numScenes - 1);

        // Expression representing the global cost
        idleCost = cp.IntExpr();

        // Make the slot-based secondary model
        IIntVar[] slot = new IIntVar[numScenes];
        for (int s = 0; s < numScenes; s++)
            slot[s] = cp.IntVar(0, numScenes - 1);
        cp.Add(cp.Inverse(scene, slot));

        // Loop over all actors, building cost
        for (int a = 0; a < numActors; a++)
        {
            // Expression for the waiting time for this actor
            IIntExpr actorWait = cp.IntExpr();

            // Calculate the first and last slots where this actor plays
            List<IIntVar> position = new List<IIntVar>();

            System.Collections.IEnumerator en = actorInScene[a].GetEnumerator();
            while (en.MoveNext())
                position.Add(slot[(int)en.Current]);

            IIntExpr firstSlot = cp.Min(position.ToArray());
            IIntExpr lastSlot = cp.Max(position.ToArray());

            // If an actor is not in a scene, he waits
            // if he is on set when the scene is filmed
            for (int s = 0; s < numScenes; s++)
            {
                if (!actorInScene[a].Contains(s))
                { // not in scene
                    IIntExpr wait = cp.And(cp.Le(firstSlot, slot[s]), cp.Le(
                            slot[s], lastSlot));
                    actorWait = cp.Sum(actorWait, cp.Prod(sceneDuration[s],
                            wait));
                }
            }

            // Accumulate the cost of waiting time for this actor
            idleCost = cp.Sum(idleCost, cp.Prod(actorPay[a], actorWait));
        }
        cp.Add(cp.Minimize(idleCost));
    }

    private void Display()
    {
        Console.WriteLine("Solution of idle cost "
                + (int)cp.GetValue(idleCost));
        Console.Write("Order:");
        for (int s = 0; s < numScenes; s++)
            Console.Write(" " + ((int)cp.GetValue(scene[s]) + 1));
        Console.WriteLine();

        // Give more detailed information on the schedule
        for (int a = 0; a < numActors; a++)
        {
            Console.Write("|");
            for (int s = 0; s < numScenes; s++)
            {
                int sc = (int)cp.GetValue(scene[s]);
                for (int d = 0; d < sceneDuration[sc]; d++)
                {
                    if (actorInScene[a].Contains(sc))
                        Console.Write("X");
                    else
                        Console.Write(".");
                }
                Console.Write("|");
            }
            Console.WriteLine("  Rate = " + actorPay[a] + ")");
        }
    }

    public static void Main(String[] args)
    {

        String inputFile = "../../../../examples/data/rehearsal.data";
        double tlim = 10.0;
        if (args.Length > 1)
            inputFile = args[1];
        if (args.Length > 2)
            tlim = Double.Parse(args[2]);

        Talent talent = new Talent(inputFile);

        cp.SetParameter(CP.DoubleParam.TimeLimit, tlim);
        cp.Solve();

        talent.Display();
    }
}
