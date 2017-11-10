REM ------------------------------------------------ -*- Visual Basic .Net -*-
REM File: SteelMill.vb
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
Imports ILOG.CP
Imports ILOG.OPL

Module SteelMill

  Friend Class MyData
    Inherits CustomOplDataSource
    Sub New(ByVal oplF As OplFactory)
      MyBase.New(oplF)
    End Sub
    Overrides Sub CustomRead()
      Dim _nbOrders As Integer = 12
      Dim _nbSlabs As Integer = 12
      Dim _nbColors As Integer = 8
      Dim _nbCap As Integer = 20

      Dim handler As OplDataHandler = DataHandler
      handler.StartElement("nbOrders")
      handler.AddIntItem(_nbOrders)
      handler.EndElement()
      handler.StartElement("nbSlabs")
      handler.AddIntItem(_nbSlabs)
      handler.EndElement()
      handler.StartElement("nbColors")
      handler.AddIntItem(_nbColors)
      handler.EndElement()
      handler.StartElement("nbCap")
      handler.AddIntItem(_nbCap)
      handler.EndElement()

      Dim _capacity() As Integer = New Integer() {0, 11, 13, 16, 17, 19, 20, 23, 24, 25, 26, 27, 28, 29, 30, 33, 34, 40, 43, 45}
      handler.StartElement("capacities")
      handler.StartArray()
      For i As Integer = 0 To _nbCap - 1
        handler.AddIntItem(_capacity(i))
      Next i

      handler.EndArray()
      handler.EndElement()

      Dim _weight() As Integer = New Integer() {22, 9, 9, 8, 8, 6, 5, 3, 3, 3, 2, 2}
      handler.StartElement("weight")
      handler.StartArray()
      For i As Integer = 0 To _nbOrders - 1
        handler.AddIntItem(_weight(i))
      Next
      handler.EndArray()
      handler.EndElement()

      Dim _colors() As Integer = New Integer() {5, 3, 4, 5, 7, 3, 6, 0, 2, 3, 1, 5}
      handler.StartElement("colors")
      handler.StartArray()
      For i As Integer = 0 To _nbOrders - 1
        handler.AddIntItem(_colors(i))
      Next
      handler.EndArray()
      handler.EndElement()
    End Sub
  End Class

  Function GetModelText() As String
    Dim model As String
    model = "using CP;" & vbCrLf & _
            "int nbOrders   = ...;" & vbCrLf & _
            "int nbSlabs = ...;" & vbCrLf & _
            "int nbColors   = ...;" & vbCrLf & _
            "int nbCap      = ...;" & vbCrLf & _
            "int capacities[1..nbCap] = ...;" & vbCrLf & _
            "int weight[1..nbOrders] = ...;" & vbCrLf & _
            "int colors[1..nbOrders] = ...;" & vbCrLf & _
            "int maxLoad = sum(i in 1..nbOrders) weight[i];" & vbCrLf & _
            "int maxCap  = max(i in 1..nbCap) capacities[i];" & vbCrLf & _
            "int loss[c in 0..maxCap] = min(i in 1..nbCap : capacities[i] >= c) capacities[i] - c; " & vbCrLf & _
            "execute {" & vbCrLf & _
            "writeln(""loss = "", loss);" & vbCrLf & _
            "writeln(""maxLoad = "", maxLoad);" & vbCrLf & _
            "writeln(""maxCap = "", maxCap);" & vbCrLf & _
            "};" & vbCrLf & _
            "dvar int where[1..nbOrders] in 1..nbSlabs;" & vbCrLf & _
            "dvar int load[1..nbSlabs] in 0..maxLoad;" & vbCrLf & _
            "execute {" & vbCrLf & _
            "  cp.param.LogPeriod = 50;" & vbCrLf & _
            "  var f = cp.factory;" & vbCrLf & _
            "  cp.setSearchPhases(f.searchPhase(where));" & vbCrLf & _
            "}" & vbCrLf & _
            "dexpr int totalLoss = sum(s in 1..nbSlabs) loss[load[s]];" & vbCrLf & _
            "minimize totalLoss;" & vbCrLf & _
            "subject to {  " & vbCrLf & _
            "  packCt: pack(load, where, weight);" & vbCrLf & _
            "  forall(s in 1..nbSlabs)" & vbCrLf & _
            "    colorCt: sum (c in 1..nbColors) (or(o in 1..nbOrders : colors[o] == c) (where[o] == s)) <= 2; " & vbCrLf & _
            "}"
    Return model
  End Function

  Sub Main()
    Dim status As Integer = 127
    Try
      OplFactory.DebugMode = True
      Dim oplF As OplFactory = New OplFactory()
      Dim errHandler As OplErrorHandler = oplF.CreateOplErrorHandler(Console.Out)
      Dim modelSource As OplModelSource = oplF.CreateOplModelSourceFromString(GetModelText(), "steelmill")
      Dim settings As OplSettings = oplF.CreateOplSettings(errHandler)
      Dim def As OplModelDefinition = oplF.CreateOplModelDefinition(modelSource, settings)
      Dim CP As CP = oplF.CreateCP()
      Dim opl As OplModel = oplF.CreateOplModel(def, CP)
      Dim dataSource As OplDataSource = New MyData(oplF)
      opl.AddDataSource(dataSource)
      opl.Generate()
      If CP.Solve() Then
        Console.Out.WriteLine("OBJECTIVE: " + opl.CP.ObjValue.ToString)
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
