// -----------------------------------------------------------------*- C# -*-
// File: ./examples/src/csharp/PlantLocation.cs
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

A ship-building company has a certain number of customers. Each customer is supplied
by exactly one plant. In turn, a plant can supply several customers. The problem is
to decide where to set up the plants in order to supply every customer while minimizing
the cost of building each plant and the transportation cost of supplying the customers.

For each possible plant location there is a fixed cost and a production capacity.
Both take into account the country and the geographical conditions.

For every customer, there is a demand and a transportation cost with respect to
each plant location.

While a first solution of this problem can be found easily by CP Optimizer, it can take
quite some time to improve it to a very good one. We illustrate the warm start capabilities
of CP Optimizer by giving a good starting point solution that CP Optimizer will try to improve.
This solution could be one from an expert or the result of another optimization engine
applied to the problem.

In the solution we only give a value to the variables that determine which plant delivers
a customer. This is sufficient to define a complete solution on all model variables.
CP Optimizer first extends the solution to all variables and then starts to improve it.

------------------------------------------------------------ */

using System;
using System.IO;
using ILOG.CP;
using ILOG.Concert;

namespace PlantLocation{
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
    public class PlantLocation
    {
        static void Main(string[] args)
        {
            String filename;
            if (args.Length > 0)
                filename = args[0];
            else
                filename = "../../../../examples/data/plant_location.data";

            DataReader data = new DataReader(filename);
            int nbCustomer = data.Next();
            int nbLocation = data.Next();

            int[][] cost = new int[nbCustomer][];
            for (int c = 0; c < nbCustomer; c++)
            {
              cost[c] = new int[nbLocation];
              for (int l = 0; l < nbLocation; l++)
                cost[c][l] = data.Next();
            }
            int[] demand = new int[nbCustomer];
            for (int c = 0; c < nbCustomer; c++)
            {
                demand[c] = data.Next();
            }
            int[] fixedCost = new int[nbLocation];
            for (int l = 0; l < nbLocation; l++)
            {
                fixedCost[l] = data.Next();
            }
            int[] capacity = new int[nbLocation];
            for (int l = 0; l < nbLocation; l++)
            {
                capacity[l] = data.Next();
            }
           
            CP cp = new CP();

            IIntVar[] cust = new IIntVar[nbCustomer];
            for (int c = 0; c < nbCustomer; c++)
            {
                cust[c] = cp.IntVar(0, nbLocation - 1);
            }
            IIntVar[] open = new IIntVar[nbLocation];
            IIntVar[] load = new IIntVar[nbLocation];
            for (int l = 0; l < nbLocation; l++)
            {
                open[l] = cp.IntVar(0, 1);
                load[l] = cp.IntVar(0, capacity[l]);
            }

            for (int l = 0; l < nbLocation; l++)
            {
               cp.Add(cp.Eq(open[l], cp.Gt(load[l], 0)));
            }
            cp.Add(cp.Pack(load, cust, demand));
            IIntExpr obj = cp.Prod(open, fixedCost);
            for (int c = 0; c < nbCustomer; c++)
            {
                obj = cp.Sum(obj, cp.Element(cost[c], cust[c]));
            }
            cp.Add(cp.Minimize(obj));

            int[] custValues = {        
                19, 0, 11, 8, 29, 9, 29, 28, 17, 15, 7, 9, 18, 15, 1, 17, 25, 18, 17, 27, 
                22, 1, 26, 3, 22, 2, 20, 27, 2, 16, 1, 16, 12, 28, 19, 2, 20, 14, 13, 27, 
                3, 9, 18, 0, 13, 19, 27, 14, 12, 1, 15, 14, 17, 0, 7, 12, 11, 0, 25, 16, 
                22, 13, 16, 8, 18, 27, 19, 23, 26, 13, 11, 11, 19, 22, 28, 26, 23, 3, 18, 23, 
                26, 14, 29, 18, 9, 7, 12, 27, 8, 20 };

            ISolution sol = cp.Solution();
            for (int c = 0; c < nbCustomer; c++)
                sol.SetValue(cust[c], custValues[c]);

            cp.SetStartingPoint(sol);
            cp.SetParameter(CP.DoubleParam.TimeLimit, 10);
            cp.SetParameter(CP.IntParam.LogPeriod, 10000);
            cp.Solve();
        }
    }
}

