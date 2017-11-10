// -----------------------------------------------------------------*- C# -*-
// File: ./examples/src/csharp/FacilityExplanations.cs
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

A company has 10 stores.  Each store must be supplied by one warehouse. The 
company has five possible locations where it has property and can build a 
supplier warehouse: Bonn, Bordeaux, London, Paris, and Rome. The warehouse 
locations have different capacities. A warehouse built in Bordeaux or Rome 
could supply only one store. A warehouse built in London could supply two 
stores; a warehouse built in Bonn could supply three stores; and a warehouse 
built in Paris could supply four stores. 

The supply costs vary for each store, depending on which warehouse is the 
supplier. For example, a store that is located in Paris would have low supply 
costs if it were supplied by a warehouse also in Paris.  That same store would 
have much higher supply costs if it were supplied by the other warehouses.

The cost of building a warehouse varies depending on warehouse location.

The problem is to find the most cost-effective solution to this problem, while
making sure that each store is supplied by a warehouse.

------------------------------------------------------------ */

using System;
using System.IO;
using ILOG.CP;
using ILOG.Concert;

namespace FacilityExplanations{

    public class Facility
    {
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

        static void Main(string[] args)
        {
            String filename;
            if (args.Length > 0)
                filename = args[0];
            else
                filename = "../../../../examples/data/facility.data";

            CP cp = new CP();
            int i, j;

            DataReader data = new DataReader(filename);
            int nbLocations = data.Next();
            int nbStores = data.Next();
            int[] capacity = new int[nbLocations];
            int[] fixedCost = new int[nbLocations];
            int[][] cost = new int[nbStores][];

            for (i = 0; i < nbStores; i++)
                cost[i] = new int[nbLocations];

            for (j = 0; j < nbLocations; j++)
                capacity[j] = data.Next();

            for (j = 0; j < nbLocations; j++)
                fixedCost[j] = data.Next();

            for (i = 0; i < nbStores; i++)
                for (j = 0; j < nbLocations; j++)
                    cost[i][j] = data.Next();

            IIntVar[] supplier = cp.IntVarArray(nbStores, 0, nbLocations - 1);
            IIntVar[] open = cp.IntVarArray(nbLocations, 0, 1);

            for (i = 0; i < nbStores; i++)
                cp.Add(cp.Eq(cp.Element(open, supplier[i]), 1));

            for (j = 0; j < nbLocations; j++)
                cp.Add(cp.Le(cp.Count(supplier, j), capacity[j]));

            IIntExpr obj = cp.ScalProd(open, fixedCost);
            for (i = 0; i < nbStores; i++)
                obj = cp.Sum(obj, cp.Element(cost[i], supplier[i]));

            cp.Add(cp.Minimize(obj));

            cp.SetParameter(CP.IntParam.Workers, 1);
            cp.SetParameter(CP.IntParam.SearchType, CP.ParameterValues.DepthFirst);
            cp.SetParameter(CP.IntParam.LogPeriod, 1);
            cp.SetParameter(CP.IntParam.LogSearchTags, CP.ParameterValues.On);

            cp.ClearExplanations();
            cp.ExplainFailure(15);
            cp.ExplainFailure(20); 
            int[] failureArray = { 3, 10, 11, 12 };
            cp.ExplainFailure(failureArray);

            cp.Solve();

            Console.WriteLine();
            Console.WriteLine("Optimal value: " + cp.GetValue(obj));
            for (j = 0; j < nbLocations; j++)
            {
                if (cp.GetValue(open[j]) == 1)
                {
                    Console.Write("Facility " + j + " is open, it serves stores ");
                    for (i = 0; i < nbStores; i++)
                    {
                        if (cp.GetValue(supplier[i]) == j)
                            Console.Write(i + " ");
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}

