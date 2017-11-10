REM ------------------------------------------------ -*- Visual Basic .Net -*-
REM File: Mulprod_main.vb
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
REM Visual Basic version of Mulprod_main.cpp of OPL distrib
REM --------------------------------------------------------------------------

Imports ILOG.OPL
Imports ILOG.Concert
Imports ILOG.CPLEX

Module Mulprod_main
  Const DATADIR As String = "../.."

  Sub Main()
    Dim status As Integer = 127
    Dim capFlour As Integer = 20
    Dim best As Double
    Dim curr As Double = Double.MaxValue
    Try
      OplFactory.DebugMode = True
      Dim oplF As OplFactory = New OplFactory
      Dim rc0 As OplRunConfiguration = oplF.CreateOplRunConfiguration(DATADIR + "/mulprod.mod", DATADIR + "/mulprod.dat")
      Dim dataElements As OplDataElements = rc0.GetOplModel().MakeDataElements()
      Do
        best = curr
        Dim rc As OplRunConfiguration = oplF.CreateOplRunConfiguration(rc0.GetOplModel().ModelDefinition, dataElements)
        rc.Cplex.SetOut(Nothing)
        rc.GetOplModel().Generate()

        Console.Out.WriteLine("Solve with capFlour = " + capFlour.ToString)
        If (rc.Cplex.Solve()) Then
          curr = rc.Cplex.ObjValue
          Console.Out.WriteLine("OBJECTIVE: " + curr.ToString("F"))
          status = 0
        Else
          Console.Out.WriteLine("No solution!")
          status = 1
        End If
        capFlour = capFlour + 1
        dataElements.GetElement("Capacity").AsNumMap().Set("flour", capFlour)
        rc.End()
      Loop While best <> curr And status = 0
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
