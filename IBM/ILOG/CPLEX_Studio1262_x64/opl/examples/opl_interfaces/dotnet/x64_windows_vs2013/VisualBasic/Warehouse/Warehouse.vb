REM ------------------------------------------------ -*- Visual Basic .Net -*-
REM File: Warehouse.vb
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
REM Visual Basic version of steelmill.cpp of OPL distrib
REM --------------------------------------------------------------------------

Imports System
Imports ILOG.Concert
Imports ILOG.CPLEX
Imports ILOG.OPL

Module Warehouse

  Sub Main(ByVal args() As String)

    Dim nbWarehouses As Integer = 5
    Dim nbStores As Integer = 10
    Dim fixedP As Integer = 30
    Dim disaggregate As Integer = 1
    For i As Integer = 0 To args.Length - 1
      If ("-h".Equals(args(i))) Then
        usage()
      ElseIf ("fixed".Equals(args(i))) Then
        i = i + 1
        If (i = args.Length) Then
          usage()
        End If
        fixedP = System.Int32.Parse(args(i))

      ElseIf ("nbWarehouses".Equals(args(i))) Then
        i = i + 1
        If (i = args.Length) Then
          usage()
        End If
        nbWarehouses = System.Int32.Parse(args(i))
      ElseIf ("nbStores".Equals(args(i))) Then
        i = i + 1
        If (i = args.Length) Then
          usage()
        End If
        nbStores = System.Int32.Parse(args(i))
      ElseIf ("disaggregate".Equals(args(i))) Then
        i = i + 1
        If (i = args.Length) Then
          usage()
        End If
        disaggregate = System.Int32.Parse(args(i))
      Else
        Exit For
      End If
    Next
    If (fixedP = -1 Or nbWarehouses = -1 Or nbStores = -1 Or disaggregate = -1) Then
      usage()
    End If
    Console.Out.WriteLine("Using parameters: ")
    Console.Out.WriteLine("    nbWarehouses " + nbWarehouses.ToString)
    Console.Out.WriteLine("    nbStores     " + nbStores.ToString)
    Console.Out.WriteLine("    fixed        " + fixedP.ToString)
    Console.Out.WriteLine("    disaggregate " + disaggregate.ToString)
    Console.Out.WriteLine()
    Dim status As Integer = 127
    Try
      OplFactory.DebugMode = True
      Dim oplF As OplFactory = New OplFactory()
      Dim errHandler As OplErrorHandler = oplF.CreateOplErrorHandler(Console.Out)
      Dim modelSource As OplModelSource = oplF.CreateOplModelSourceFromString(GetModelText(), "warehouse")
      Dim settings As OplSettings = oplF.CreateOplSettings(errHandler)
      Dim def As OplModelDefinition = oplF.CreateOplModelDefinition(modelSource, settings)
      Dim cplex As Cplex = oplF.CreateCplex()
      Dim opl As OplModel = oplF.CreateOplModel(def, cplex)
      Dim dataSource As OplDataSource = New MyParams(oplF, nbWarehouses, nbStores, fixedP, disaggregate)
      opl.AddDataSource(dataSource)
      opl.Generate()
      If (cplex.Solve()) Then
        Console.Out.WriteLine("OBJECTIVE: " + opl.Cplex.ObjValue.ToString())
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

  Function GetModelText() As String

    Dim model As String
    model = "int   fixed        = ...;" & vbCrLf & _
            "int   nbWarehouses = ...;" & vbCrLf & _
            "int   nbStores     = ...;" & vbCrLf & _
            "int   disaggregate = ...;" & vbCrLf & _
            "assert nbStores > nbWarehouses;" & vbCrLf & _
            "range Warehouses = 1..nbWarehouses;" & vbCrLf & _
            "range Stores     = 1..nbStores;" & vbCrLf & _
            "int capacity[w in Warehouses] = nbStores div nbWarehouses + w mod (nbStores div nbWarehouses);" & vbCrLf & _
            "int supplyCost[s in Stores][w in Warehouses] = 1+((s+10*w) mod 100);" & vbCrLf & _
            "dvar boolean open[Warehouses];" & vbCrLf & _
            "dvar boolean supply[Stores][Warehouses];" & vbCrLf & _
            "minimize " & vbCrLf & _
            "sum(w in Warehouses) fixed * open[w] +" & vbCrLf & _
            "sum(w in Warehouses, s in Stores) supplyCost[s][w] * supply[s][w];" & vbCrLf & _
            "constraints {" & vbCrLf & _
            "  forall(s in Stores)" & vbCrLf & _
            "    sum(w in Warehouses) supply[s][w] == 1;" & vbCrLf & _
            "  forall(w in Warehouses)" & vbCrLf & _
            "    sum(s in Stores) supply[s][w] <= open[w]*capacity[w];" & vbCrLf & _
            "  if (disaggregate == 1) {" & vbCrLf & _
            "   forall(w in Warehouses, s in Stores)" & vbCrLf & _
            "      supply[s][w] <= open[w];" & vbCrLf & _
            "  }" & vbCrLf & _
            "}"
    Return model
  End Function

  Sub usage()
    Console.Error.WriteLine()
    Console.Error.WriteLine("Usage: warehouse [-h] parameters")
    Console.Error.WriteLine("  -h  this help message")
    Console.Error.WriteLine("  parameters ")
    Console.Error.WriteLine("    nbWarehouses <value> ")
    Console.Error.WriteLine("    nbStores <value> ")
    Console.Error.WriteLine("    fixed <value> ")
    Console.Error.WriteLine("    disaggregate <value> ")
    Console.Error.WriteLine()
    Environment.Exit(0)
  End Sub


  Friend Class MyParams
    Inherits CustomOplDataSource

    Dim _nbWarehouses As Integer
    Dim _nbStores As Integer
    Dim _fixed As Integer
    Dim _disaggregate As Integer

    Friend Sub New(ByVal oplF As OplFactory, ByVal nbWarehouses As Integer, ByVal nbStores As Integer, ByVal fixedP As Integer, ByVal disaggregate As Integer)
      MyBase.New(oplF)

      _nbWarehouses = nbWarehouses
      _nbStores = nbStores
      _fixed = fixedP
      _disaggregate = disaggregate
    End Sub
    Public Overrides Sub CustomRead()

      Dim handler As OplDataHandler = DataHandler

      handler.StartElement("nbWarehouses")
      handler.AddIntItem(_nbWarehouses)
      handler.EndElement()

      handler.StartElement("nbStores")
      handler.AddIntItem(_nbStores)
      handler.EndElement()

      handler.StartElement("fixed")
      handler.AddIntItem(_fixed)
      handler.EndElement()

      handler.StartElement("disaggregate")
      handler.AddIntItem(_disaggregate)
      handler.EndElement()
    End Sub
  End Class
End Module
