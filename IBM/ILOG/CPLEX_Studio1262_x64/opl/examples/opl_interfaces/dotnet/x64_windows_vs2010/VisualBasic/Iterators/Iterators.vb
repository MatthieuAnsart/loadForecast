REM ------------------------------------------------ -*- Visual Basic .Net -*-
REM File: Iterators.vb
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
REM Visual Basic version of iterators.cpp of OPL distrib
REM --------------------------------------------------------------------------

Imports ILOG.OPL
Imports ILOG.Concert
Imports ILOG.CPLEX

Module Iterators
  Const DATADIR As String = "../.."

REM The purpose of this sample is to check the result of filtering by iterating on the generated data element.
REM
REM The data element is an array of strings that is indexed by a set of strings.
REM It is filled as the result of an iteration on a set of tuples by filtering out the duplicates.
REM It is based on the model used in "Sparsity" run configuration of the "transp" example.
REM
REM
REM The simplified model is:
REM
REM {string} Products = ...;
REM tuple Route { string p; string o; string d; }
REM {Route} Routes = ...;
REM {string} orig[p in Products] = { o | <p,o,d> in Routes };
REM
  Function Sample1() As Integer
    Dim status As Integer = 127
    Try
      OplFactory.DebugMode = True
      Dim oplF As OplFactory = New OplFactory

      Dim rc As OplRunConfiguration = oplF.CreateOplRunConfiguration(DATADIR + "/transp2.mod", DATADIR + "/transp2.dat")
      Dim opl As OplModel = rc.OplModel
      rc.Cplex.SetOut(Nothing)

      Console.Out.WriteLine("Verification of the computation of orig:")

      REM Get the orig, Routes, Product elements from the OplModel.
      Dim orig As ISymbolSetMap = opl.GetElement("Orig").AsSymbolSetMap()
      Dim Routes As ITupleSet = opl.GetElement("Routes").AsTupleSet()
      Dim Products As ISymbolSet = opl.GetElement("Products").AsSymbolSet()

      REM Iterate through the orig to see the result of the data element filtering.
      Dim it1 As IEnumerator = Products.GetEnumerator
      While it1.MoveNext
        Dim p As String = it1.Current
        REM We are in the last dimension of the array (as it is a 1 dimensional array), so we can use the get method directly.
        Dim mSet As ISymbolSet = orig.Get(p)
        Console.Out.Write("for p = " + p)
        Console.Out.Write(" we have [")
        Dim i As Integer
        For i = 0 To mSet.Size - 1
          Console.Out.Write(mSet.GetValue(i) + " ")
        Next
        Console.WriteLine("]")
      End While

      Console.WriteLine("---------------------")

      REM Iterate through the TupleSet.
      Dim it2 As IEnumerator = Routes.GetEnumerator
      While it2.MoveNext
        Dim t As ITuple = it2.Current
        REM Get the string "p" from the tuple.
        Dim p As String = t.GetStringValue("p")
        REM if "p" is in the indexer, we will try to add the "o" string to the array.
        If (Products.Contains(p)) Then
          Console.Out.Write("for p = " + p + " we will have " + t.GetStringValue("o") + " from ")
          Console.Out.WriteLine(t)
        End If
      End While
      Console.Out.WriteLine("---------------------")
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
    Return status
  End Function


REM The purpose of this sample is to output a multidimensional array x[i][j] to illustrate how arrays and sub-arrays are managed.
REM To access the elements of an array, you must first access the sub-arrays until the last dimension, then you can get the values.
REM  Here, as there are two dimensions, you have to get one sub-array from which you can directly get the values. 
REM
REM The array of integers is indexed by two sets of strings..
REM
REM The simplified model is:
REM
REM {string} s1 = ...;
REM {string} s2 = ...;
REM {int} x[s1][s2] = ...;
REM
  Function Sample2() As Integer
    Dim status As Integer = 127
    Try
      OplFactory.DebugMode = True
      Dim oplF As OplFactory = New OplFactory

      Dim rc As OplRunConfiguration = oplF.CreateOplRunConfiguration(DATADIR + "/iterators.mod")
      rc.Cplex.SetOut(Nothing)
      Dim opl As OplModel = rc.OplModel

      REM Get the x, s1 and s2 elements from the OplModel.
      Dim x As IIntMap = opl.GetElement("x").AsIntMap()
      Dim s1 As ISymbolSet = opl.GetElement("s1").AsSymbolSet()
      Dim s2 As ISymbolSet = opl.GetElement("s2").AsSymbolSet()

      REM Iterate on the first indexer.
      Dim it1 As IEnumerator = s1.GetEnumerator
      While it1.MoveNext
        Dim sub1 As String = it1.Current
        REM Get the 2nd dimension array from the 1st dimension.
        Dim subM As IIntMap = x.GetSub(sub1)
        REM Iterate on the second indexer of x (that is the indexer of the sub array).
        Dim it2 As IEnumerator = s2.GetEnumerator
        While it2.MoveNext
          Dim sub2 As String = it2.Current
          Console.Out.Write(sub1 + " " + sub2 + " ")
          REM We are in the last dimension of the array, so we can directly use the get method.
          Console.Out.WriteLine(subM.Get(sub2))
        End While
      End While
      Console.Out.WriteLine("---------------------")
      status = 0
    Catch ex As ILOG.OPL.OplException
        Console.Out.WriteLine(ex.Message)
	Status = 2
    Catch ex As ILOG.Concert.Exception
      Console.WriteLine(ex.Message)
      status = 3
    Catch ex As System.Exception
        Console.Out.WriteLine(ex.Message)
	Status = 4
    End Try
    Return status
  End Function


    REM The purpose of this sample is to output an array of tuples arrayT[i], 
    REM to illustrate how tuple elements can be accessed.
    REM The simplified model is:
    REM tuple t
    REM {
    REM   int a;
    REM   int b;
    REM }
    REM {string} ids={"id1","id2","id3"};
    REM t arrayT[ids]=[<1,2>,<2,3>,<1,3>];

    Function getModelTextSample3() As String
        Dim model As String
        model = "tuple t{int a;int b;}" & vbCrLf & _
                "{string} ids = {""id1"",""id2"", ""id3""};" & vbCrLf & _
                "t arrayT[ids] = [<1,2>,<2,3>,<1,3>];"
        Return model
    End Function

    Function sample3() As Integer
        Dim status As Integer = 0
        OplFactory.DebugMode = True
        Dim oplF As OplFactory = New OplFactory
        Try
            Dim errHandler As OplErrorHandler = oplF.CreateOplErrorHandler(Console.Out)
            Dim settings As OplSettings = oplF.CreateOplSettings(errHandler)
            Dim src As OplModelSource = oplF.CreateOplModelSourceFromString(getModelTextSample3(), "tuple array iterator")
            Dim def As OplModelDefinition = oplF.CreateOplModelDefinition(src, settings)
            Dim cplex As Cplex = oplF.CreateCplex()
            Dim opl As OplModel = oplF.CreateOplModel(def, cplex)
            opl.Generate()

            REM get the string set used to index the array of tuples
            Dim arrayT As ITupleMap = opl.GetElement("arrayT").AsTupleMap()
            Dim ids As ISymbolSet = opl.GetElement("ids").AsSymbolSet()
            REM iterate on the index set to retrieve the tuples stored in the array
            Dim it As IEnumerator = ids.GetEnumerator()
            While it.MoveNext()
                Console.Out.Write("arrayT[" + it.Current + "] = ")
                Dim id As IMapIndexArray = oplF.MapIndexArray(0)
                id.Add(it.Current.ToString)
                Dim t As ITuple = arrayT.MakeTuple()
                arrayT.GetAt(id, t)
                Console.Out.WriteLine(t)
            End While
    	Catch ex As ILOG.OPL.OplException
           Console.Out.WriteLine(ex.Message)
	   Status = 2
    	Catch ex As ILOG.Concert.Exception
           Console.WriteLine(ex.Message)
           status = 3
    	Catch ex As System.Exception
           Console.Out.WriteLine(ex.Message)
	   Status = 4        
	End Try
        oplF.End()
        Return status
    End Function

    Sub Main()
        Dim res1 As Integer = Sample1()
        Dim res2 As Integer = Sample2()
        Dim res3 As Integer = Sample3()
        Environment.ExitCode = res1 + res2 + res3
        Console.WriteLine("--Press <Enter> to exit--")
        Console.ReadLine()
    End Sub
End Module
