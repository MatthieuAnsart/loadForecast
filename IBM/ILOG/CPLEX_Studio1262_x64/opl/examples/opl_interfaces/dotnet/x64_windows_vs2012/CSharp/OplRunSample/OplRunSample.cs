// --------------------------------------------------------------- -*- C# -*-
// File: OplRunSample.cs
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
// C# version of oplrunsample.cpp of OPL distrib
//--------------------------------------------------------------------------

using System;
using System.IO;
using ILOG.Concert;
using ILOG.CP;
using ILOG.CPLEX;
using ILOG.OPL;
using ILOG.Util;

namespace OplRunSample
{
    public class CommandLine
    {
        public string ModelFileName;
        public string[] DataFileNames;
        public string ExportName;
        public string CompileName;
        public string ExternalDataName;
        public string InternalDataName;
        public Boolean IsVerbose = false;
        public Boolean IsForceElementUsage = false;
        public Boolean IsRelaxation = false;
        public Boolean IsConflict = false;
        public Boolean IsProject = false;

        public string getProjectPath()
        {
            if (IsProject)
                return ModelFileName;
            else
                return null;
        }
        public string getRunConfigurationName()
        {
            if (IsProject && DataFileNames.Length == 1)
                return DataFileNames[0];
            else
                return null;
        }

        void usage()
        {
            Console.Out.WriteLine();
            Console.Out.WriteLine("Usage: ");
            Console.Out.WriteLine("OplRunSample [options] model-file [data-file ...]");
            Console.Out.WriteLine("OplRunSample [options] -p project-path [run-configuration]");
            Console.Out.WriteLine("  options ");
            Console.Out.WriteLine("    -h               this help message");
            Console.Out.WriteLine("    -v               verbose");
            Console.Out.WriteLine("    -e [export-file] export model");
            Console.Out.WriteLine("    -de dat-file     write external data");
            Console.Out.WriteLine("    -di dat-file     write internal data");
            Console.Out.WriteLine("    -o output-file   compile model");
            Console.Out.WriteLine("    -f               force element usage");
            Console.Out.WriteLine("    -relax           calculate relaxations needed for feasible model");
            Console.Out.WriteLine("    -conflict        calculate a conflict for infeasible model");
            Console.Out.WriteLine();
            Environment.Exit(0);
        }
        public CommandLine(string[] args)
        {
            if (args.Length < 2)
            {
                ModelFileName = "..\\..\\mulprod.mod";
                DataFileNames = new String[1] { "..\\..\\mulprod.dat" };
            }
            int i = 0;
            for (i = 1; i < args.Length; i++)
            {
                if (args[i].Equals("-h"))
                    usage();
                else if (args[i].Equals("-p"))
                    IsProject = true;
                else if (args[i].Equals("-v"))
                    IsVerbose = true;
                else if (args[i].Equals("-e"))
                {
                    i++;
                    if (i < args.Length && !args[i].StartsWith("-"))
                        ExportName = args[i];
                    else
                    {
                        ExportName = "OplRunSample.lp";
                        i--;
                    }
                }
                else if (args[i].Equals("-o"))
                {
                    i++;
                    if (i < args.Length && !args[i].StartsWith("-"))
                        CompileName = args[i];
                    else
                        usage();
                }
                else if (args[i].Equals("-de"))
                {
                    i++;
                    if (i < args.Length && !args[i].StartsWith("-"))
                        ExternalDataName = args[i];
                    else
                        usage();
                }
                else if (args[i].Equals("-di"))
                {
                    i++;
                    if (i < args.Length && !args[i].StartsWith("-"))
                        InternalDataName = args[i];
                    else
                        usage();
                }
                else if (args[i].Equals("-f"))
                    IsForceElementUsage = true;
                else if (args[i].Equals("-relax"))
                    IsRelaxation = true;
                else if (args[i].Equals("-conflict"))
                    IsConflict = true;
                else if (args[i].StartsWith("-"))
                {
                    Console.Error.WriteLine("Unknown option: " + args[i]);
                    usage();
                }
                else
                    break;
            }
            if (i >= args.Length && i > 1)
                usage();
            if (i < args.Length)
            {
                ModelFileName = args[i];
                i++;
                int j;
                int nbDataFiles = args.Length - i;
                DataFileNames = new String[nbDataFiles];
                int k = 0;
                for (j = i; j < args.Length; j++)
                {
                    DataFileNames[k] = args[j];
                    k++;
                }
            }
            if ((IsProject && DataFileNames.Length > 1) || (IsProject && args.Length < 3))
                usage();
        }
    }
    class Timer
    {
        public int _time = System.Environment.TickCount;
        int _startTime = System.Environment.TickCount;
        public Timer()
        {
        }
        public void restart()
        {
            _time = System.Environment.TickCount;
        }
        public double getTime()
        {
            return (System.Environment.TickCount - _time) / 1000;
        }
        public double getAbsoluteTime()
        {
            return (System.Environment.TickCount - _startTime) / 1000;
        }
    }

    /**********************/
    /* Class OplRunSample */
    /**********************/
    class OplRunSample
    {
        public CommandLine _cl;
        Timer _timer;
        public static void Main(string[] args)
        {
            int status = 127;
            try
            {
                OplRunSample oplrun = new OplRunSample();
                oplrun._cl = new CommandLine(Environment.GetCommandLineArgs());
                oplrun._timer = new Timer();
                status = oplrun.Run();
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
        int Run()
        {
            int status = 127;
            OplFactory oplF = new OplFactory();
            if (_cl.CompileName != null)
            {
                OplCompiler compiler = oplF.CreateOplCompiler();
                StreamWriter ofs = new StreamWriter(_cl.CompileName, false);
                OplModelSource modelSource = oplF.CreateOplModelSource(_cl.ModelFileName);
                compiler.Compile(modelSource, ofs);
                ofs.Close();
                Trace("Compile");
                return 0;
            }
            if (_cl.ModelFileName == null && !_cl.IsProject)
                return 0;

            Trace("initial");
            OplRunConfiguration rc;
            OplErrorHandler errHandler = oplF.CreateOplErrorHandler();
            if (_cl.IsProject)
            {
                OplProject prj = oplF.CreateOplProject(_cl.getProjectPath());
                rc = prj.MakeRunConfiguration(_cl.getRunConfigurationName());
            }
            else
            {
                if (_cl.DataFileNames.Length == 0)
                    rc = oplF.CreateOplRunConfiguration(_cl.ModelFileName);
                else
                    rc = oplF.CreateOplRunConfiguration(_cl.ModelFileName, _cl.DataFileNames);
            }
            rc.ErrorHandler = errHandler;
            OplModel opl = rc.OplModel;
            OplSettings settings = opl.Settings;
            settings.IsWithLocations = true;
            settings.IsWithNames = true;
            settings.IsForceElementUsage = _cl.IsForceElementUsage;

            status = 9;
            if (opl.ModelDefinition.hasMain())
            {
                status = opl.Main();
                Console.Out.WriteLine("main returns " + status);
                Trace("main");
            }
            else if (errHandler.Ok)
            {
                opl.Generate();
                Trace("generate model");
                if (opl.HasCplex)
                {
                    if (_cl.ExportName != null)
                    {
                        opl.Cplex.ExportModel(_cl.ExportName);
                        Trace("export model " + _cl.ExportName);
                    }
                    if (_cl.IsRelaxation)
                    {
                        Console.Out.WriteLine("RELAXATIONS to obtain a feasible problem: ");
                        opl.PrintRelaxation(Console.Out);
                        Console.Out.WriteLine("RELAXATIONS done.");
                    }
                    if (_cl.IsConflict)
                    {
                        Console.Out.WriteLine("CONFLICT in the infeasible problem: ");
                        opl.PrintConflict(Console.Out);
                        Console.Out.WriteLine("CONFLICT done.");
                    }
                    if (!_cl.IsRelaxation && !_cl.IsConflict)
                    {
                        bool result = false;
                        try
                        {
                            result = opl.Cplex.Solve();
                        }
                        catch (IloException ex)
                        {
                            Console.Out.WriteLine("### ENGINE exception: " + ex.Message);
                        }
                        if (result)
                        {
                            Trace("solve");
                            Console.Out.WriteLine();
                            Console.Out.WriteLine();
                            Console.Out.WriteLine("OBJECTIVE: " + opl.Cplex.ObjValue.ToString("F"));
                            opl.PostProcess();
                            Trace("post process");
                            if (_cl.IsVerbose)
                                opl.PrintSolution(Console.Out);
                            status = 0;
                        }
                        else
                        {
                            Trace("no solution");
                            status = 1;
                        }
                    }
                }
                else
                {//opl.hasCP()
                    bool result = false;
                    try
                    {
                        result = opl.CP.Solve();
                    }
                    catch (IloException ex)
                    {
                        Console.Out.WriteLine("### Engine exception: " + ex.Message);
                    }
                    if (result)
                    {
                        Trace("solve");
                        if (opl.CP.HasObjective())
                        {
                            Console.Out.WriteLine();
                            Console.Out.WriteLine();
                            Console.Out.WriteLine("OBJECTIVE: " + opl.CP.ObjValue.ToString("F"));
                        }
                        else
                        {
                            Console.Out.WriteLine();
                            Console.Out.WriteLine();
                            Console.Out.WriteLine("OBJECTIVE: no objective");
                        }
                        opl.PostProcess();
                        Trace("post process");
                        if (_cl.IsVerbose)
                            opl.PrintSolution(Console.Out);
                        status = 0;
                    }
                    else
                    {
                        Trace("no solution");
                        status = 1;
                    }
                }
            }
            if (_cl.ExternalDataName != null)
            {
                StreamWriter ofs = new StreamWriter(_cl.ExternalDataName, false);
                opl.PrintExternalData(ofs);
                ofs.Close();
                Trace("write external data " + _cl.ExternalDataName);
            }
            if (_cl.InternalDataName != null)
            {
                StreamWriter ofs = new StreamWriter(_cl.InternalDataName, false);
                opl.PrintInternalData(ofs);
                ofs.Close();
                Trace("write internal data " + _cl.InternalDataName);
            }

            Trace("done");
            return status;
        }

        void Trace(string title, string info)
        {
            Console.Out.WriteLine();
            Console.Out.Write("<<< " + title);
            if (info != null)
                Console.Out.Write(": " + info);
            if (_cl.IsVerbose)
            {
                Console.Out.Write(", at " + _timer.getAbsoluteTime() + "s, took " + _timer.getTime() + "s");
                _timer.restart();
            } Console.Out.WriteLine();
        }
        void Trace(string title)
        {
            Trace(title, null);
        }
    }
}
