REM ------------------------------------------------ -*- Visual Basic .Net -*-
REM File: Carseq.vb
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
REM Visual Basic version of carseq.cpp of OPL distrib
REM --------------------------------------------------------------------------

Imports System
Imports ILOG.Concert
Imports ILOG.CP
Imports ILOG.OPL

Module Carseq

    Friend Class MyData
        Inherits CustomOplDataSource
        Sub New(ByVal oplF As OplFactory)
            MyBase.New(oplF)
        End Sub
        Overrides Sub CustomRead()
            Dim _nbConfs As Integer = 7
            Dim _nbOptions As Integer = 5

            Dim handler As OplDataHandler = DataHandler
            handler.StartElement("nbConfs")
            handler.AddIntItem(_nbConfs)
            handler.EndElement()
            handler.StartElement("nbOptions")
            handler.AddIntItem(_nbOptions)
            handler.EndElement()

            Dim _demand() As Integer = {5, 5, 10, 10, 10, 10, 5}
            handler.StartElement("demand")
            handler.StartArray()
            For i As Integer = 0 To _nbConfs - 1
                handler.AddIntItem(_demand(i))
            Next i
            handler.EndArray()
            handler.EndElement()

            Dim _option(,) As Integer = {{1, 0, 0, 0, 1, 1, 0}, _
                                         {0, 0, 1, 1, 0, 1, 0}, _
                                         {1, 0, 0, 0, 1, 0, 0}, _
                                         {1, 1, 0, 1, 0, 0, 0}, _
                                         {0, 0, 1, 0, 0, 0, 0}}
            handler.StartElement("option")
            handler.StartArray()
            For i As Integer = 0 To _nbOptions - 1
                handler.StartArray()
                For j As Integer = 0 To _nbConfs - 1
                    handler.AddIntItem(_option(i, j))
                Next
                handler.EndArray()
            Next
            handler.EndArray()
            handler.EndElement()

            Dim _capacity(,) As Integer = {{1, 2}, {2, 3}, {1, 3}, {2, 5}, {1, 5}}
            handler.StartElement("capacity")
            handler.StartArray()
            For i As Integer = 0 To _nbOptions - 1
                handler.StartTuple()
                For j As Integer = 0 To 1
                    handler.AddIntItem(_capacity(i, j))
                Next
                handler.EndTuple()
            Next
            handler.EndArray()
            handler.EndElement()
        End Sub
    End Class

    Function GetModelText() As String
        Dim model As String
        model = "using CP;" & vbCrLf & _
        "int nbConfs   = ...; " & vbCrLf & _
        "int nbOptions = ...;" & vbCrLf & _
        "range Confs = 1..nbConfs;" & vbCrLf & _
        "range Options = 1..nbOptions;" & vbCrLf & _
        "int demand[Confs] = ...;" & vbCrLf & _
        "tuple CapacitatedWindow {" & vbCrLf & _
        "  int l;" & vbCrLf & _
        "  int u;" & vbCrLf & _
        "};" & vbCrLf & _
        "CapacitatedWindow capacity[Options] = ...; " & vbCrLf & _
        "range AllConfs = 0..nbConfs;" & vbCrLf & _
        "int nbCars = sum (c in Confs) demand[c];" & vbCrLf & _
        "int nbSlots = ftoi(floor(nbCars * 1.1 + 5));" & vbCrLf & _
        "int nbBlanks = nbSlots - nbCars;" & vbCrLf & _
        "range Slots = 1..nbSlots;" & vbCrLf & _
        "int option[Options,Confs] = ...; " & vbCrLf & _
        "int allOptions[o in Options, c in AllConfs] = (c == 0) ? 0 : option[o][c];" & vbCrLf & _
        "dvar int slot[Slots] in AllConfs;" & vbCrLf & _
        "dvar int lastSlot in nbCars..nbSlots;" & vbCrLf & _
        "minimize lastSlot - nbCars; " & vbCrLf & _
        "subject to {" & vbCrLf & _
        "  count(slot, 0) == nbBlanks;" & vbCrLf & _
        "  forall (c in Confs)" & vbCrLf & _
        "    count(slot, c) == demand[c];" & vbCrLf & _
        "  forall(o in Options, s in Slots : s + capacity[o].u - 1 <= nbSlots)" & vbCrLf & _
        "    sum(j in s .. s + capacity[o].u - 1) allOptions[o][slot[j]] <= capacity[o].l;" & vbCrLf & _
        "  forall (s in nbCars + 1 .. nbSlots)" & vbCrLf & _
        "    (s > lastSlot) => slot[s] == 0;" & vbCrLf & _
        "};"

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
