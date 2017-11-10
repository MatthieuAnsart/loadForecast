// --------------------------------------------------------------- -*- C# -*-
// File: Mulprod_main.cs
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
// C# version of mulprod_main.cpp of OPL distrib
//--------------------------------------------------------------------------

using System;
using ILOG.Concert;
using ILOG.CPLEX;
using ILOG.OPL;

namespace Mulprod_main
{
  class Mulprod_main
  {
    static int Main(string[] args)
    {
      int status = 127;
      const string DATADIR = "../..";
      OplFactory oplF = new OplFactory();
      try {
        int capFlour = 20;
        double best;
        double curr = double.MaxValue;

        OplRunConfiguration rc0 = oplF.CreateOplRunConfiguration(DATADIR+"/mulprod.mod",DATADIR+"/mulprod.dat");
        OplDataElements dataElements = rc0.GetOplModel().MakeDataElements();

        Cplex cplex = oplF.CreateCplex();
        do {
          best = curr;

          OplRunConfiguration rc = oplF.CreateOplRunConfiguration(rc0.GetOplModel().ModelDefinition,dataElements);

          rc.Cplex.SetOut(null);
          rc.GetOplModel().Generate();

          Console.Out.WriteLine("Solve with capFlour = " + capFlour);
          if ( rc.Cplex.Solve() ) {
            curr = rc.Cplex.ObjValue;
            Console.Out.WriteLine();
            Console.Out.WriteLine( "OBJECTIVE: " +curr);
            status = 0;
          } else {
            Console.Out.WriteLine("No solution!");
            status = 1;
          }

          capFlour++;
          // Change the value of Capacity["flour"] in dataElements
          dataElements.GetElement("Capacity").AsNumMap().Set("flour", capFlour);

          rc.End();
        } while ( best != curr && status == 0);
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
    oplF.End();
    Console.WriteLine("--Press <Enter> to exit--");
    Console.ReadLine();
    return status;
    }
  }
}
