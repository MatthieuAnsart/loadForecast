
// -----------------------------------------------------------------*- C# -*-
// File: ./examples/src/csharp/Truckfleet.cs
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
The problem is to deliver some orders to several clients with a single truck. 
Each order consists of a given quantity of a product of a certain type (called
its color).
The truck must be configured in order to handle one, two or three different colors of products.
The cost for configuring the truck from a configuration A to a configuration B depends on A and B.
The configuration of the truck determines its capacity and its loading cost. 
A truck can only be loaded with orders for the same customer.
Both the cost (for configuring and loading the truck) and the number of travels needed to deliver all the 
orders must be minimized, the cost being the most important criterion. 

------------------------------------------------------------ */

using System;
using System.IO;
using ILOG.CP;
using ILOG.Concert;


namespace Truckfleet {
  public class Truckfleet {
    static public int Max(int[] x) {
        int m = x[0];
        for (int i = 1; i < x.Length; i++) {
            if (m<x[i])
                m=x[i];
        }
        return m;
    }

    static public void Main(string[] args) {
        CP cp = new CP();
        int nbTruckConfigs = 7; // number of possible configurations for the truck 
        int nbOrders       = 21;
        int nbCustomers    = 3; 
        int nbTrucks       = 15; //max number of travels of the truck
        int[]   maxTruckConfigLoad = { //Capacity of the truck depends on its config 
            11, 11, 11, 11,10,10,10}; 
        int     maxLoad        = Max(maxTruckConfigLoad) ; 
        int[]   customerOfOrder = {
            0, 0, 0, 0, 0, 0, 0, 1, 1, 1,
            1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 
            2};
        int[]   volumes = {
            3, 4, 3, 2, 5, 4, 11, 4, 5, 2,
            4, 7, 3, 5, 2, 5, 6, 11, 1, 6, 
            3};
        int[]   colors = {
            1, 2, 0, 1, 1, 1, 0, 0, 0, 0, 
            2, 2, 2 ,0, 2, 1, 0, 2, 0, 0, 
            0};
        int[]   truckCost = { //cost for loading a truck of a given config
            2, 2, 2, 3, 3, 3, 4}; 
        
        //Decision variables
        IIntVar[] truckConfigs = cp.IntVarArray(nbTrucks,0,nbTruckConfigs-1); //configuration of the truck
        IIntVar[] where = cp.IntVarArray(nbOrders, 0, nbTrucks - 1); //In which truck is an order
        IIntVar[] load = cp.IntVarArray(nbTrucks, 0, maxLoad); //load of a truck
        IIntVar numUsed = cp.IntVar(0, nbTrucks); // number of trucks used
        IIntVar[] customerOfTruck = cp.IntVarArray(nbTrucks, 0, nbCustomers);
        
        // transition costs between trucks
        IIntTupleSet costTuples = cp.IntTable(3);  
        cp.AddTuple(costTuples, new int[] {0,0,0});
        cp.AddTuple(costTuples, new int[] {0,1,0});
        cp.AddTuple(costTuples, new int[] {0,2,0});
        cp.AddTuple(costTuples, new int[] {0,3,10});
        cp.AddTuple(costTuples, new int[] {0,4,10});
        cp.AddTuple(costTuples, new int[] {0,5,10});
        cp.AddTuple(costTuples, new int[] {0,6,15});
        cp.AddTuple(costTuples, new int[] {1,0,0});
        cp.AddTuple(costTuples, new int[] {1,1,0});
        cp.AddTuple(costTuples, new int[] {1,2,0});
        cp.AddTuple(costTuples, new int[] {1,3,10});
        cp.AddTuple(costTuples, new int[] {1,4,10});
        cp.AddTuple(costTuples, new int[] {1,5,10});
        cp.AddTuple(costTuples, new int[] {1,6,15});
        cp.AddTuple(costTuples, new int[] {2,0,0});
        cp.AddTuple(costTuples, new int[] {2,1,0});
        cp.AddTuple(costTuples, new int[] {2,2,0});
        cp.AddTuple(costTuples, new int[] {2,3,10});
        cp.AddTuple(costTuples, new int[] {2,4,10});
        cp.AddTuple(costTuples, new int[] {2,5,10});
        cp.AddTuple(costTuples, new int[] {2,6,15});
        cp.AddTuple(costTuples, new int[] {3,0,3});
        cp.AddTuple(costTuples, new int[] {3,1,3});
        cp.AddTuple(costTuples, new int[] {3,2,3});
        cp.AddTuple(costTuples, new int[] {3,3,0});
        cp.AddTuple(costTuples, new int[] {3,4,10});
        cp.AddTuple(costTuples, new int[] {3,5,10});
        cp.AddTuple(costTuples, new int[] {3,6,15});
        cp.AddTuple(costTuples, new int[] {4,0,3});
        cp.AddTuple(costTuples, new int[] {4,1,3});
        cp.AddTuple(costTuples, new int[] {4,2,3});
        cp.AddTuple(costTuples, new int[] {4,3,10});
        cp.AddTuple(costTuples, new int[] {4,4,0});
        cp.AddTuple(costTuples, new int[] {4,5,10});
        cp.AddTuple(costTuples, new int[] {4,6,15});
        cp.AddTuple(costTuples, new int[] {5,0,3});
        cp.AddTuple(costTuples, new int[] {5,1,3});
        cp.AddTuple(costTuples, new int[] {5,2,3});
        cp.AddTuple(costTuples, new int[] {5,3,10});
        cp.AddTuple(costTuples, new int[] {5,4,10});
        cp.AddTuple(costTuples, new int[] {5,5,0});
        cp.AddTuple(costTuples, new int[] {5,6,15});
        cp.AddTuple(costTuples, new int[] {6,0,3});
        cp.AddTuple(costTuples, new int[] {6,1,3});
        cp.AddTuple(costTuples, new int[] {6,2,3});
        cp.AddTuple(costTuples, new int[] {6,3,10});
        cp.AddTuple(costTuples, new int[] {6,4,10});
        cp.AddTuple(costTuples, new int[] {6,5,10});
        cp.AddTuple(costTuples, new int[] {6,6,0});
        
        IIntVar[] transitionCost = cp.IntVarArray(nbTrucks-1, 0, 1000);
        for (int i = 1; i < nbTrucks; i++) {
            IIntVar[] auxVars = new IIntVar[3];
            auxVars[0]= truckConfigs[i-1];
            auxVars[1]= truckConfigs[i];
            auxVars[2]= transitionCost[i-1];
            cp.Add(cp.AllowedAssignments(auxVars, costTuples));
        }
        
        // constrain the volume of the orders in each truck 
        cp.Add(cp.Pack(load, where, volumes, numUsed));
        for (int i = 0; i < nbTrucks; i++) {
            cp.Add(cp.Le(load[i], cp.Element(maxTruckConfigLoad, truckConfigs[i])));
        }
        
        // compatibility between the colors of an order and the configuration of its truck 
        int[][] allowedContainerConfigs = new int[3][];
        allowedContainerConfigs[0] = new int[] {0, 3, 4, 6};
        allowedContainerConfigs[1] = new int[] {1, 3, 5, 6};
        allowedContainerConfigs[2] = new int[] {2, 4, 5, 6};
        for (int j = 0; j < nbOrders; j++) {
            IIntVar configOfContainer = cp.IntVar(allowedContainerConfigs[colors[j]]);
            cp.Add(cp.Eq(configOfContainer, cp.Element(truckConfigs,where[j])));
        }
        
        // only one customer per truck 
        for (int j = 0; j < nbOrders; j++) {
            cp.Add(cp.Eq( cp.Element(customerOfTruck,where[j]), customerOfOrder[j]));
        }
        
        // non used trucks are at the end
        for (int j = 1; j < nbTrucks; j++) {
            cp.Add(cp.Or( cp.Gt(load[j-1], 0) , cp.Eq(load[j], 0)));
        }
        
        // Dominance: the non used trucks keep the last used configuration
        cp.Add(cp.Gt(load[0],0));
        for (int i = 1; i < nbTrucks; i++) {
            cp.Add(cp.Or(cp.Gt(load[i], 0), cp.Eq(truckConfigs[i], truckConfigs[i-1])));
        }
        
        //Dominance:  regroup deliveries with same configuration
        for (int i = nbTrucks-2; i >0; i--) {
            IConstraint Ct = cp.TrueConstraint();
            for (int p = i+1; p < nbTrucks; p++) 
                Ct = cp.And( cp.Neq(truckConfigs[p], truckConfigs[i-1]) , Ct);
            cp.Add( cp.Or(cp.Eq(truckConfigs[i], truckConfigs[i-1]), Ct));
        }
        
        // Objective: first criterion for minimizing the cost for configuring and loading trucks 
        //            second criterion for minimizing the number of trucks
        
        IIntExpr obj1 = cp.Constant(0);
        for(int i = 0; i < nbTrucks; i++){
            obj1 = cp.Sum(obj1, cp.Prod( cp.Element(truckCost,truckConfigs[i]),cp.Neq(load[i],0)));
        }
        obj1 = cp.Sum(obj1, cp.Sum(transitionCost));
        
        IIntExpr obj2 = numUsed;
        
        // Search strategy: first assign order to truck
        ISearchPhase phase = cp.SearchPhase(where);
        
        // Multicriteria lexicographic optimization
        cp.Add(cp.Minimize(cp.StaticLex(obj1,obj2)));
        cp.SetParameter(CP.DoubleParam.TimeLimit, 20); 
        cp.SetParameter(CP.IntParam.LogPeriod, 50000); 
        cp.Solve(phase);
        double[] obj = cp.GetObjValues();
        Console.WriteLine("Configuration cost: "+ (int)obj[0] +
                           " Number of Trucks: " + (int)obj[1]);
        for(int i = 0; i < nbTrucks; i++) {  
            if (cp.GetValue(load[i]) > 0) {
                Console.Write("Truck " +  i +
                              ": Config=" +  cp.GetIntValue(truckConfigs[i])
                              + " Items= ");
                for (int j = 0; j < nbOrders; j++) {
                    if (cp.GetValue(where[j]) == i) {
                         Console.Write("<" + j + "," + colors[j] + "," + volumes[j] + "> ");
                    }
                }
                Console.WriteLine();
            }
        } 
    }
  }
}




