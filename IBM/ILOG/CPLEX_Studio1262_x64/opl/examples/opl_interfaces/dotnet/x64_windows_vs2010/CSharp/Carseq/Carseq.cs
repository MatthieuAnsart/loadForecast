// --------------------------------------------------------------- -*- C# -*-
// File: Carseq.cs
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
// C# version of carseq.cpp of OPL distrib
//--------------------------------------------------------------------------
using System;
using ILOG.Concert;
using ILOG.CP;
using ILOG.OPL;

namespace Carseq
{
    class Carseq
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
                OplModelSource modelSource = oplF.CreateOplModelSourceFromString(GetModelText(), "carseq");
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
            model += "int nbConfs   = ...;";
            model += "int nbOptions = ...;";
            model += "range Confs = 1..nbConfs;";
            model += "range Options = 1..nbOptions;";
            model += "int demand[Confs] = ...;";
            model += "tuple CapacitatedWindow {";
            model += "  int l;";
            model += "  int u;";
            model += "};";
            model += "CapacitatedWindow capacity[Options] = ...; ";
            model += "range AllConfs = 0..nbConfs;";
            model += "int nbCars = sum (c in Confs) demand[c];";
            model += "int nbSlots = ftoi(floor(nbCars * 1.1 + 5)); ";
            model += "int nbBlanks = nbSlots - nbCars;";
            model += "range Slots = 1..nbSlots;";
            model += "int option[Options,Confs] = ...; ";
            model += "int allOptions[o in Options, c in AllConfs] = (c == 0) ? 0 : option[o][c];";
            model += "dvar int slot[Slots] in AllConfs;";
            model += "dvar int lastSlot in nbCars..nbSlots;";

            model += "minimize lastSlot - nbCars; ";
            model += "subject to {";
            model += "  count(slot, 0) == nbBlanks;";
            model += "  forall (c in Confs)";
            model += "    count(slot, c) == demand[c];";
            model += "  forall(o in Options, s in Slots : s + capacity[o].u - 1 <= nbSlots)";
            model += "    sum(j in s .. s + capacity[o].u - 1) allOptions[o][slot[j]] <= capacity[o].l;";
            model += "  forall (s in nbCars + 1 .. nbSlots)";
            model += "    (s > lastSlot) => slot[s] == 0;";
            model += "}";      
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

            int _nbConfs = 7;
            int _nbOptions = 5;

            OplDataHandler handler = getDataHandler();
            handler.StartElement("nbConfs");
            handler.AddIntItem(_nbConfs);
            handler.EndElement();
            handler.StartElement("nbOptions");
            handler.AddIntItem(_nbOptions);
            handler.EndElement();

            int[] _demand = {5, 5, 10, 10, 10, 10, 5};
            handler.StartElement("demand");
            handler.StartArray();
            for (int i = 0; i < _nbConfs; i++)
                handler.AddIntItem(_demand[i]);
            handler.EndArray();
            handler.EndElement();

            int[,] _option = {{1, 0, 0, 0, 1, 1, 0}, 
                               {0, 0, 1, 1, 0, 1, 0}, 
                               {1, 0, 0, 0, 1, 0, 0}, 
                               {1, 1, 0, 1, 0, 0, 0}, 
                               {0, 0, 1, 0, 0, 0, 0}};
            handler.StartElement("option");
            handler.StartArray();
            for (int i = 0 ; i< _nbOptions ; i++) {
                handler.StartArray();
                for (int j = 0; j < _nbConfs; j++)
                    handler.AddIntItem(_option[i, j]);
                handler.EndArray();
            }
            handler.EndArray();
            handler.EndElement();

            int[,] _capacity= {{1, 2}, {2, 3}, {1, 3}, {2, 5}, {1, 5}};
            handler.StartElement("capacity");
            handler.StartArray();
            for (int i = 0; i<_nbOptions;i++) {
                handler.StartTuple();
                for (int j= 0; j<=1;j++)
                    handler.AddIntItem(_capacity[i,j]);
                handler.EndTuple();
            }
            handler.EndArray();
            handler.EndElement();
        }
    }
}
