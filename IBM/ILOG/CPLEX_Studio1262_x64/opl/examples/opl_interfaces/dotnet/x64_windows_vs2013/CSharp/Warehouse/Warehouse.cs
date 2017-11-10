// --------------------------------------------------------------- -*- C# -*-
// File: Warehouse.cs
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
// C# version of warehouse.cpp of OPL distrib
//--------------------------------------------------------------------------
using System;
using ILOG.Concert;
using ILOG.CPLEX;
using ILOG.OPL;

namespace Warehouse
{
    class Warehouse
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            int nbWarehouses = -1;
            int nbStores = -1;
            int fixedP = -1;
            int disaggregate = -1;
            for (int i = 0; i < args.Length; i++)
            {
                if ("-h".Equals(args[i]))
                {
                    usage();
                }
                else if ("fixed".Equals(args[i]))
                {
                    if (i == args.Length)
                    {
                        usage();
                    }
                    fixedP = System.Int32.Parse(args[++i]);
                }
                else if ("nbWarehouses".Equals(args[i]))
                {
                    if (i == args.Length)
                    {
                        usage();
                    }
                    nbWarehouses = System.Int32.Parse(args[++i]);
                }
                else if ("nbStores".Equals(args[i]))
                {
                    if (i == args.Length)
                    {
                        usage();
                    }
                    nbStores = System.Int32.Parse(args[++i]);
                }
                else if ("disaggregate".Equals(args[i]))
                {
                    if (i == args.Length)
                    {
                        usage();
                    }
                    disaggregate = System.Int32.Parse(args[++i]);
                }
                else
                {
                    break;
                }
            }

            if (fixedP == -1 || nbWarehouses == -1 || nbStores == -1 || disaggregate == -1)
            {
                usage();
            }

            int status = 127;
            try
            {
                OplFactory.DebugMode = true;
                OplFactory oplF = new OplFactory();
                OplErrorHandler errHandler = oplF.CreateOplErrorHandler(Console.Out);
                OplModelSource modelSource = oplF.CreateOplModelSourceFromString(GetModelText(), "warehouse");
                OplSettings settings = oplF.CreateOplSettings(errHandler);
                OplModelDefinition def = oplF.CreateOplModelDefinition(modelSource, settings);
                Cplex cplex = oplF.CreateCplex();
                OplModel opl = oplF.CreateOplModel(def, cplex);
                OplDataSource dataSource = new MyParams(oplF, nbWarehouses, nbStores, fixedP, disaggregate);
                opl.AddDataSource(dataSource);
                opl.Generate();
                if (cplex.Solve())
                {
                    Console.Out.WriteLine("OBJECTIVE: " + opl.Cplex.ObjValue);
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

            Environment.ExitCode = status;

            Console.WriteLine("--Press <Enter> to exit--");
            Console.ReadLine();
        }

        static String GetModelText()
        {
            String model = "";
            model += "int   fixed        = ...;";
            model += "int   nbWarehouses = ...;";
            model += "int   nbStores     = ...;";
            model += "int   disaggregate = ...;";
            model += "assert nbStores > nbWarehouses;";

            model += "range Warehouses = 1..nbWarehouses;";
            model += "range Stores     = 1..nbStores;";

            model += "int capacity[w in Warehouses] = nbStores div nbWarehouses + w mod (nbStores div nbWarehouses);";
            model += "int supplyCost[s in Stores][w in Warehouses] = 1+((s+10*w) mod 100);";

            model += "dvar boolean open[Warehouses];";
            model += "dvar boolean supply[Stores][Warehouses];";

            model += "minimize ";
            model += "sum(w in Warehouses) fixed * open[w] +";
            model += "sum(w in Warehouses, s in Stores) supplyCost[s][w] * supply[s][w];";

            model += "constraints {";
            model += "  forall(s in Stores)";
            model += "    sum(w in Warehouses) supply[s][w] == 1;";
            model += "  forall(w in Warehouses)";
            model += "    sum(s in Stores) supply[s][w] <= open[w]*capacity[w];";
            model += "  if (disaggregate == 1) {";
            model += "   forall(w in Warehouses, s in Stores)";
            model += "      supply[s][w] <= open[w];";
            model += "  }";
            model += "}";
            return model;
        }

        static void usage()
        {
            Console.Error.WriteLine();
            Console.Error.WriteLine("Usage: warehouse [-h] parameters");
            Console.Error.WriteLine("  -h  this help message");
            Console.Error.WriteLine("  parameters ");
            Console.Error.WriteLine("    nbWarehouses <value> ");
            Console.Error.WriteLine("    nbStores <value> ");
            Console.Error.WriteLine("    fixed <value> ");
            Console.Error.WriteLine("    disaggregate <value> ");
            Console.Error.WriteLine();
            Environment.Exit(0);
        }
    }

    internal class MyParams : CustomOplDataSource
    {
        int _nbWarehouses;
        int _nbStores;
        int _fixed;
        int _disaggregate;

        internal MyParams(OplFactory oplF, int nbWarehouses, int nbStores, int fixedP, int disaggregate)
            : base(oplF)
        {
            _nbWarehouses = nbWarehouses;
            _nbStores = nbStores;
            _fixed = fixedP;
            _disaggregate = disaggregate;
        }
        public override void CustomRead()
        {
            OplDataHandler handler = DataHandler;

            handler.StartElement("nbWarehouses");
            handler.AddIntItem(_nbWarehouses);
            handler.EndElement();

            handler.StartElement("nbStores");
            handler.AddIntItem(_nbStores);
            handler.EndElement();

            handler.StartElement("fixed");
            handler.AddIntItem(_fixed);
            handler.EndElement();

            handler.StartElement("disaggregate");
            handler.AddIntItem(_disaggregate);
            handler.EndElement();
        }
    }
}
