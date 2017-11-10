// --------------------------------------------------------------- -*- C# -*-
// File: Mulprod.cs
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
// C# version of mulprod.cpp of OPL distrib
//--------------------------------------------------------------------------

using System;
using System.Collections;
using ILOG.Concert;
using ILOG.CPLEX;
using ILOG.OPL;

namespace Mulprod
{
  class Mulprod
  {
    static int Main(string[] args)
    {
      int status = 127;
      const string DATADIR = "../..";

      try
      {
          OplFactory oplF = new OplFactory();
          OplErrorHandler handler = oplF.CreateOplErrorHandler(Console.Out);
          OplModelSource modelSource = oplF.CreateOplModelSource(DATADIR + "/mulprod.mod");
          OplSettings settings = oplF.CreateOplSettings(handler);
          OplModelDefinition def = oplF.CreateOplModelDefinition(modelSource, settings);
          Cplex cplex = oplF.CreateCplex();
          OplModel opl = oplF.CreateOplModel(def, cplex);
          OplDataSource dataSource = oplF.CreateOplDataSource(DATADIR + "/mulprod.dat");
          opl.AddDataSource(dataSource);
          opl.Generate();
          if (cplex.Solve())
          {
              Console.Out.WriteLine();
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

    Console.WriteLine("--Press <Enter> to exit--");
    Console.ReadLine();
    
    return status;
    }
  }
}
