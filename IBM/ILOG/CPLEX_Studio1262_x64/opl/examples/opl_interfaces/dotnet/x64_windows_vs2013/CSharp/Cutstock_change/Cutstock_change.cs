// --------------------------------------------------------------- -*- C# -*-
// File: Cutstock_change.cs
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
// C# version of cutstock_change.cpp of OPL distrib
//--------------------------------------------------------------------------
using System;
using System.Collections;
using ILOG.Concert;
using ILOG.CPLEX;
using ILOG.OPL;


namespace Cutstock_change
{
    class Cutstock_change
    {
        const string DATADIR = "../..";
        const double RC_EPS = 0.000001;
        static int Main(string[] args)
        {
            int status = 127;
            try
            {
                OplFactory.DebugMode = true;
                OplFactory oplF = new OplFactory();
                OplErrorHandler errHandler = oplF.CreateOplErrorHandler();
                OplSettings settings = oplF.CreateOplSettings(errHandler);

                Cplex masterCplex = oplF.CreateCplex();
                masterCplex.SetOut(null);

                OplErrorHandler errorHandler = oplF.CreateOplErrorHandler();
                OplRunConfiguration masterRC = oplF.CreateOplRunConfiguration(DATADIR + "/cutstock_change.mod", DATADIR + "/cutstock_change.dat");
                masterRC.ErrorHandler = errorHandler;
                masterRC.Cplex = masterCplex;
                OplModel masterOpl = masterRC.OplModel;
                masterOpl.Generate();
                OplDataElements masterDataElements = masterOpl.MakeDataElements();

                OplModelSource subSource = oplF.CreateOplModelSource(DATADIR + "/cutstock-sub.mod");
                OplModelDefinition subDef = oplF.CreateOplModelDefinition(subSource, settings);
                Cplex subCplex = oplF.CreateCplex();
                subCplex.SetOut(null);

                int nWdth = masterDataElements.GetElement("Amount").AsIntMap().Size;
                ArrayList masterVars = new ArrayList();
                INumVarMap cuts = masterOpl.GetElement("Cut").AsNumVarMap();
                for (int i = 1; i <= nWdth; i++)
                {
                    masterVars.Add(cuts.Get(i));
                }

                double best;
                double curr = double.MaxValue;
                do
                {
                    best = curr;

                    // Make master model 
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

                    // set sub model data
                    OplDataElements subDataElements = oplF.CreateOplDataElements();
                    subDataElements.AddElement(masterDataElements.GetElement("RollWidth"));
                    subDataElements.AddElement(masterDataElements.GetElement("Size"));
                    subDataElements.AddElement(masterDataElements.GetElement("Duals"));

                    // get reduced costs and set them in sub problem
                    INumMap duals = subDataElements.GetElement("Duals").AsNumMap();
                    for (int i = 1; i <= nWdth; i++)
                    {
                        IForAllRange forAll = (IForAllRange)masterOpl.GetElement("ctFill").AsConstraintMap().Get(i);
                        duals.Set(i, masterCplex.GetDual(forAll));
                    }
                    // make sub model
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
                    };

                    // Add variable in master model
                    INumVar newVar = masterCplex.NumVar(0, double.MaxValue);
                    IObjective masterObj = masterOpl.Objective;
                    masterCplex.SetLinearCoef(masterObj, newVar, 1);
                    for (int i = 1; i <= nWdth; i++)
                    {
                        double coef = subCplex.GetValue(subOpl.GetElement("Use").AsIntVarMap().Get(i));
                        IForAllRange forAll = (IForAllRange)masterOpl.GetElement("ctFill").AsConstraintMap().Get(i);
                        masterCplex.SetLinearCoef(forAll, newVar, coef);
                    }
                    masterVars.Add(newVar);

                    subOpl.End();
                } while (best != curr && status == 0);

                INumVar[] masterVarsA = (INumVar[])masterVars.ToArray(typeof(INumVar));

                masterCplex.Add(masterCplex.Conversion(masterVarsA, NumVarType.Int));
                if (masterCplex.Solve())
                {
                    Console.Out.WriteLine("OBJECTIVE: " + masterCplex.ObjValue);
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


            Console.WriteLine("--Press <Enter> to exit--");
            Console.ReadLine();
            return status;
        }
    }
}
