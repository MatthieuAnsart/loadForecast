// --------------------------------------------------------------- -*- C# -*-
// File: SteelMill.cs
// --------------------------------------------------------------------------
// Licensed Materials - Property of IBM
// 
// 5725-A06 5725-A29 5724-Y48 5724-Y49 5724-Y54 5724-Y55 
// Copyright IBM Corporation 1998, 2013. All Rights Reserved.
//
// Note to U.S. Government Users Restricted Rights:
// Use, duplication or disclosure restricted by GSA ADP Schedule
// Contract with IBM Corp.
// --------------------------------------------------------------------------

//-------------------------------------------------------------- -*- C# -*-
// C# version of steelmill.cpp of OPL distrib
//--------------------------------------------------------------------------
using System;
using ILOG.Concert;
using ILOG.CP;
using ILOG.OPL;

namespace SteelMill
{
    class SteelMill
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            int status = 127;
            try
            {
                OplFactory.DebugMode = true;
                OplFactory oplF = new OplFactory();
                OplErrorHandler errHandler = oplF.CreateOplErrorHandler(Console.Out);
                OplModelSource modelSource = oplF.CreateOplModelSourceFromString(GetModelText(), "steelmill");
                OplSettings settings = oplF.CreateOplSettings(errHandler);
                OplModelDefinition def = oplF.CreateOplModelDefinition(modelSource, settings);
                CP cp = oplF.CreateCP();
                OplModel opl = oplF.CreateOplModel(def, cp);
                OplDataSource dataSource = new MyData(oplF);
                opl.AddDataSource(dataSource);
                opl.Generate();
                if (cp.Solve())
                {
                    Console.Out.WriteLine("OBJECTIVE: " + opl.CP.ObjValue);
                    opl.PostProcess();
                    opl.PrintSolution(Console.Out);
                    status = 0;
                }
                else
                {
                    Console.Out.WriteLine("No solution!");
                    status = 1;
                }

                oplF.End();
            }
            catch (ILOG.OPL.OplException ex)
            {
                Console.WriteLine(ex.Message);
                status = 2;
            }
            catch (ILOG.Concert.Exception ex)
            {
                Console.WriteLine(ex.Message);
                status = 3;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                status = 4;
            }
            Environment.ExitCode=status;

            Console.WriteLine("--Press <Enter> to exit--");
            Console.ReadLine();
        }

        static String GetModelText() 
        {
            String model=""; 
        model+="using CP;";
            model+="int nbOrders   = ...;";
            model+="int nbSlabs = ...;";
            model+="int nbColors   = ...;";
            model+="int nbCap      = ...;";
            model+="int capacities[1..nbCap] = ...;";
            model+="int weight[1..nbOrders] = ...;";
            model+="int colors[1..nbOrders] = ...;";
            model+="int maxLoad = sum(i in 1..nbOrders) weight[i];";
            model+="int maxCap  = max(i in 1..nbCap) capacities[i];";
            model+="int loss[c in 0..maxCap] = min(i in 1..nbCap : capacities[i] >= c) capacities[i] - c; ";
            model+="execute {";
            model+="writeln(\"loss = \", loss);";
            model+="writeln(\"maxLoad = \", maxLoad);";
            model+="writeln(\"maxCap = \", maxCap);";
            model+="};";
            model+="dvar int where[1..nbOrders] in 1..nbSlabs;";
            model+="dvar int load[1..nbSlabs] in 0..maxLoad;";
            model+="execute {";
            model+="  cp.param.LogPeriod = 50;";
            model+="  var f = cp.factory;";
            model+="  cp.setSearchPhases(f.searchPhase(where));";
            model+="}";
            model+="dexpr int totalLoss = sum(s in 1..nbSlabs) loss[load[s]];";
            model+="minimize totalLoss;";
            model+="subject to {  ";
            model+="  packCt: pack(load, where, weight);";
            model+="  forall(s in 1..nbSlabs)";
            model+="    colorCt: sum (c in 1..nbColors) (or(o in 1..nbOrders : colors[o] == c) (where[o] == s)) <= 2; ";
            model+="}";      
            return model;
        }

    }

    internal class MyData : CustomOplDataSource 
    {
    
        internal MyData(OplFactory oplF) : base(oplF)
        {
        }
        public override void CustomRead()
        {
            int _nbOrders = 12;
            int _nbSlabs = 12;
            int _nbColors = 8;
            int _nbCap = 20;

            OplDataHandler handler = getDataHandler();

            handler.StartElement("nbOrders");
            handler.AddIntItem(_nbOrders);
            handler.EndElement();
            handler.StartElement("nbSlabs");
            handler.AddIntItem(_nbSlabs);
            handler.EndElement();
            handler.StartElement("nbColors");
            handler.AddIntItem(_nbColors);
            handler.EndElement();
            handler.StartElement("nbCap");
            handler.AddIntItem(_nbCap);
            handler.EndElement();

            int[] _capacity = {0, 11, 13, 16, 17, 19, 20, 23, 24, 25, 26, 27, 28, 29, 30, 33, 34, 40, 43, 45};
            handler.StartElement("capacities");
            handler.StartArray();
            for (int i=0; i<_nbCap; i++)
                handler.AddIntItem(_capacity[i]);
            handler.EndArray();
            handler.EndElement();

            int[] _weight = {22, 9, 9, 8, 8, 6, 5, 3, 3, 3, 2, 2};
            handler.StartElement("weight");
            handler.StartArray();
            for (int i=0; i<_nbOrders; i++)
                handler.AddIntItem(_weight[i]);
            handler.EndArray();
            handler.EndElement();

            int[] _colors = {5, 3, 4, 5, 7, 3, 6, 0, 2, 3, 1, 5};
            handler.StartElement("colors");
            handler.StartArray();
            for (int i=0; i<_nbOrders; i++)
                handler.AddIntItem(_colors[i]);
            handler.EndArray();
            handler.EndElement();
        }
    }
}
