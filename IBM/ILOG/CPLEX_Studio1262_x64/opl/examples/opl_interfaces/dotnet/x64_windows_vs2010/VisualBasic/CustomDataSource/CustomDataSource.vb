REM ------------------------------------------------ -*- Visual Basic .Net -*-
REM File: CustomDataSource.vb
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
REM Visual Basic version of customdatasource.cpp of OPL distrib
REM --------------------------------------------------------------------------

Imports System
Imports ILOG.Concert
Imports ILOG.OPL
Imports ILOG.CPLEX

Module CustomDataSource
  Const DATADIR As String = "../.."
  Sub Main()
    Dim status As Integer = 127
    Try
      OplFactory.DebugMode = True
      Dim oplF As OplFactory = New OplFactory()
      Dim errHandler As OplErrorHandler = oplF.CreateOplErrorHandler(Console.Out)
      Dim modelSource As OplModelSource = oplF.CreateOplModelSource(DATADIR + "/customDataSource.mod")
      Dim settings As OplSettings = oplF.CreateOplSettings(errHandler)
      Dim def As OplModelDefinition = oplF.CreateOplModelDefinition(modelSource, settings)
      Dim cplex As Cplex = oplF.CreateCplex()
      Dim opl As OplModel = oplF.CreateOplModel(def, cplex)
      Dim dataSource As OplDataSource = New MyCustomDataSource(oplF)
      opl.AddDataSource(dataSource)
      opl.Generate()
      oplF.End()
      status = 0
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


  Friend Class MyCustomDataSource
    Inherits CustomOplDataSource
    Friend Sub New(ByVal oplF As OplFactory)
      MyBase.New(oplF)
    End Sub
    Public Overrides Sub CustomRead()
      Dim handler As OplDataHandler = DataHandler

      ' initialize the int "simpleInt"
      handler.StartElement("anInt")
      handler.AddIntItem(3)
      handler.EndElement()

      ' initialize the int array "simpleIntArray"
      handler.StartElement("anIntArray")
      handler.StartArray()
      handler.AddIntItem(1)
      handler.AddIntItem(2)
      handler.AddIntItem(3)
      handler.EndArray()
      handler.EndElement()

      ' initialize int array indexed by float "anArrayIndexedByFloat"
      handler.StartElement("anArrayIndexedByFloat")
      handler.StartIndexedArray()
      handler.SetItemNumIndex(2.0)
      handler.AddIntItem(1)
      handler.SetItemNumIndex(2.5)
      handler.AddIntItem(2)
      handler.SetItemNumIndex(1.0)
      handler.AddIntItem(3)
      handler.SetItemNumIndex(1.5)
      handler.AddIntItem(4)
      handler.EndIndexedArray()
      handler.EndElement()

      ' initialize int array indexed by string "anArrayIndexedByString"
      handler.StartElement("anArrayIndexedByString")
      handler.StartIndexedArray()
      handler.SetItemStringIndex("idx1")
      handler.AddIntItem(1)
      handler.SetItemStringIndex("idx2")
      handler.AddIntItem(2)
      handler.EndIndexedArray()
      handler.EndElement()

      ' initialize a tuple in the order the components are declared
      handler.StartElement("aTuple")
      handler.StartTuple()
      handler.AddIntItem(1)
      handler.AddNumItem(2.3)
      handler.AddStringItem("not named tuple")
      handler.EndTuple()
      handler.EndElement()

      ' initialize a tuple using tuple component names.
      handler.StartElement("aNamedTuple")
      handler.StartNamedTuple()
      handler.SetItemName("f")
      handler.AddNumItem(3.45)
      handler.SetItemName("s")
      handler.AddStringItem("named tuple")
      handler.SetItemName("i")
      handler.AddIntItem(99)
      handler.EndNamedTuple()
      handler.EndElement()

      ' initialize the tuple set "simpleTupleSet"
      handler.StartElement("aTupleSet")
      handler.StartSet()
      ' first tuple
      handler.StartTuple()
      handler.AddIntItem(1)
      handler.AddNumItem(2.5)
      handler.AddStringItem("a")
      handler.EndTuple()
      ' second element
      handler.StartTuple()
      handler.AddIntItem(3)
      handler.AddNumItem(4.1)
      handler.AddStringItem("b")
      handler.EndTuple()
      handler.EndSet()
      handler.EndElement()

      ' initialize element 3 and 2 of the tuple array "simpleTupleArray" in that particular order
      handler.StartElement("aTupleArray")
      handler.StartIndexedArray()
      ' initialize 3rd cell
      handler.SetItemIntIndex(3)
      handler.StartTuple()
      handler.AddIntItem(1)
      handler.AddNumItem(2.5)
      handler.AddStringItem("a")
      handler.EndTuple()
      ' initialize second cell
      handler.SetItemIntIndex(2)
      handler.StartTuple()
      handler.AddIntItem(3)
      handler.AddNumItem(4.1)
      handler.AddStringItem("b")
      handler.EndTuple()
      handler.EndIndexedArray()
      handler.EndElement()

      ' initialize int array indexed by tuple set "anArrayIndexedByTuple"
      handler.StartElement("anArrayIndexedByTuple")
      handler.StartIndexedArray()
      handler.StartItemTupleIndex()
      handler.AddIntItem(3)
      handler.AddNumItem(4.1)
      handler.AddStringItem("b")
      handler.EndItemTupleIndex()
      handler.AddIntItem(1)
      handler.EndIndexedArray()
      handler.EndElement()

      ' initialize a 2-dimension int array 'a2DIntArray'
      handler.StartElement("a2DIntArray")
      handler.StartArray()
      For i As Integer = 1 To 2
        handler.StartArray()
        For j As Integer = 1 To 3
          handler.AddIntItem(i * 10 + j)
        Next
        handler.EndArray()
      Next
      handler.EndArray()
      handler.EndElement()

    End Sub
  End Class
End Module
