REM ------------------------------------------------ -*- Visual Basic .Net -*-
REM File: Mulprod.vb
REM --------------------------------------------------------------------------
REM Licensed Materials - Property of IBM
REM 
REM 5725-A06 5725-A29 5724-Y48 5724-Y49 5724-Y54 5724-Y55
REM Copyright IBM Corporation 1998, 2013. All Rights Reserved.
REM
REM Note to U.S. Government Users Restricted Rights:
REM Use, duplication or disclosure restricted by GSA ADP Schedule
REM Contract with IBM Corp.
REM --------------------------------------------------------------------------


REM --------------------------------------------------------------------------
REM Visual Basic version of mulprod.cpp of OPL distrib
REM --------------------------------------------------------------------------

Imports ILOG.OPL
Imports ILOG.Concert
Imports ILOG.CPLEX

Module Mulprod
  Const DATADIR As String = "../.."

  Sub Main()
    Dim status As Integer = 127
    Try
      OplFactory.DebugMode = True

      Dim oplF As OplFactory = New OplFactory
 
      Dim errorHandler As OplErrorHandler = oplF.CreateOplErrorHandler()
    
      Dim modelSource As OplModelSource = oplF.CreateOplModelSource(DATADIR + "/mulprod.mod")
      Dim settings As OplSettings = oplF.CreateOplSettings(errorHandler)
     
      Dim def As OplModelDefinition = oplF.CreateOplModelDefinition(modelSource, settings)
   
      Dim cplex As Cplex = oplF.CreateCplex()
      cplex.SetOut(Nothing)
      Dim opl As OplModel = oplF.CreateOplModel(def, cplex)
      Dim dataSource As OplDataSource = oplF.CreateOplDataSource(DATADIR + "/mulprod.dat")
      opl.AddDataSource(dataSource)
    
      opl.Generate()
  
      If (cplex.Solve()) Then
        
      
        Console.Out.WriteLine("OBJECTIVE: " + Str(opl.Cplex.ObjValue))
        opl.PostProcess()
        opl.PrintSolution(Console.Out)
        status = 0
      Else
        Console.Out.WriteLine("No solution!")
        status = 1
      End If
      oplF.End()
    Catch ex As ILOG.OPL.OplException
        Console.Out.WriteLine(ex.Message)
	Status = 2
    Catch ex As ILOG.Concert.Exception
        Console.Out.WriteLine(ex.Message)
	Status = 3
    Catch ex As System.Exception
        Console.Out.WriteLine(ex.Message)
	Status = 4
    End Try
    Environment.ExitCode = status
    Console.WriteLine("--Press <Enter> to exit--")
    Console.ReadLine()
  End Sub

End Module
