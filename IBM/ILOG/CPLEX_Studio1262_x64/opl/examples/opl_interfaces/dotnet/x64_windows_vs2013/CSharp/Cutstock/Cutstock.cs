// --------------------------------------------------------------- -*- C# -*-
// File: Cutstock.cs
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
// C# version of cutstock.cpp of OPL distrib
//--------------------------------------------------------------------------

using System;
using ILOG.Concert;
using ILOG.OPL;
using ILOG.CPLEX;

namespace Cutstock
{
    class Cutstock
    {
        static int Main(string[] args)
        {
            int status = 127;
            const string DATADIR = "../..";
            const double RC_EPS = 0.000001;
            try
            {
                OplFactory.DebugMode = true;
                OplFactory oplF = new OplFactory();
                OplErrorHandler errHandler = oplF.CreateOplErrorHandler();
                OplSettings settings = oplF.CreateOplSettings(errHandler);
                // Make master model 
                Cplex masterCplex = oplF.CreateCplex();
                masterCplex.SetOut(null);

                OplRunConfiguration masterRC0 = oplF.CreateOplRunConfiguration(DATADIR + "/cutstock.mod", DATADIR + "/cutstock.dat");
                masterRC0.Cplex = masterCplex;
                OplDataElements masterDataElements = masterRC0.OplModel.MakeDataElements();

                // prepare sub model source, definition and engine
                OplModelSource subSource = oplF.CreateOplModelSource(DATADIR + "/cutstock-sub.mod");
                OplModelDefinition subDef = oplF.CreateOplModelDefinition(subSource, settings);
                Cplex subCplex = oplF.CreateCplex();
                subCplex.SetOut(null);

                const int nbItems = 5;
                IIntRange items = masterCplex.IntRange(1, 5);
                double best;
                double curr = double.MaxValue;
                do
                {
                    best = curr;

                    masterCplex.ClearModel();

                    OplRunConfiguration masterRC = oplF.CreateOplRunConfiguration(masterRC0.OplModel.ModelDefinition, masterDataElements);
                    masterRC.Cplex = masterCplex;
                    masterRC.OplModel.Generate();

                    Console.Out.WriteLine("Solve master.");
                    if (masterCplex.Solve())
                    {
                        curr = masterCplex.ObjValue;
                        Console.Out.WriteLine("OBJECTIVE: " + curr);
                        status = 0;
                    }
                    else
                    {
                        Console.Out.WriteLine("No solution!");
                        status = 1;
                    }

                    // prepare sub model data
                    OplDataElements subDataElements = oplF.CreateOplDataElements();
                    subDataElements.AddElement(masterDataElements.GetElement("RollWidth"));
                    subDataElements.AddElement(masterDataElements.GetElement("Size"));
                    subDataElements.AddElement(masterDataElements.GetElement("Duals"));
                    // get reduced costs and set them in sub problem
                    INumMap duals = subDataElements.GetElement("Duals").AsNumMap();
                    for (int i = 1; i <= nbItems; i++)
                    {
                        IForAllRange forAll = (IForAllRange)masterRC.OplModel.GetElement("ctFill").AsConstraintMap().Get(i);
                        duals.Set(i, masterCplex.GetDual(forAll));
                    }
                    //make sub model
                    OplModel subOpl = oplF.CreateOplModel(subDef, subCplex);
                    subOpl.AddDataSource(subDataElements);
                    subOpl.Generate();

                    Console.Out.WriteLine("Solve sub.");
                    if (subCplex.Solve())
                    {
                        Console.Out.WriteLine("OBJECTIVE: " + subCplex.ObjValue);
                        status = 0;
                    }
                    else
                    {
                        Console.Out.WriteLine("No solution!");
                        status = 1;
                    }

                    if (subCplex.ObjValue > -RC_EPS)
                    {
                        break;
                    }

                    // Add variable in master model
                    IIntMap newFill = masterCplex.IntMap(items);
                    for (int i = 1; i <= nbItems; i++)
                    {
                        int coef = (int)subCplex.GetValue(subOpl.GetElement("Use").AsIntVarMap().Get(i));
                        newFill.Set(i, coef);
                    }
                    ITupleBuffer buf = masterDataElements.GetElement("Patterns").AsTupleSet().MakeTupleBuffer(-1);
                    buf.SetIntValue("id", masterDataElements.GetElement("Patterns").AsTupleSet().Size);
                    buf.SetIntValue("cost", 1);
                    buf.SetIntMapValue("fill", newFill);
                    buf.Commit();

                    subOpl.End();
                    masterRC.End();
                } while (best != curr && status == 0);
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

            Console.WriteLine("--Press <Enter> to exit--");
            Console.ReadLine();
            return status;
        }
    }
}
