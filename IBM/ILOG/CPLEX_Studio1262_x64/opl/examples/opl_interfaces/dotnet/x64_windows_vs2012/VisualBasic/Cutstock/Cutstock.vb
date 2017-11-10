REM ------------------------------------------------ -*- Visual Basic .Net -*-
REM File: Cutstock.vb
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
REM Visual Basic version of Cutstock.cpp of OPL distrib
REM --------------------------------------------------------------------------

Imports ILOG.OPL
Imports ILOG.Concert
Imports ILOG.CPLEX

Module Cutstock
  Const DATADIR As String = "../.."
  Const RC_EPS As Double = 0.000001

  Sub Main()
        Dim status As Integer = 127

        Try
            OplFactory.DebugMode = True
            Dim oplF As OplFactory = New OplFactory
            Dim errHandler As OplErrorHandler = oplF.CreateOplErrorHandler()
            Dim settings As OplSettings = oplF.CreateOplSettings(errHandler)


            REM Make master model 
            Dim masterCplex As Cplex = oplF.CreateCplex()
            masterCplex.SetOut(Nothing)

            Dim masterRC0 As OplRunConfiguration = oplF.CreateOplRunConfiguration(DATADIR + "/cutstock.mod", DATADIR + "/cutstock.dat")
            masterRC0.Cplex = masterCplex
            Dim masterDataElements As OplDataElements = masterRC0.OplModel.MakeDataElements()

            REM Prepare sub model
            Dim subSource As OplModelSource = oplF.CreateOplModelSource(DATADIR + "/cutstock-sub.mod")
            Dim subDef As OplModelDefinition = oplF.CreateOplModelDefinition(subSource, settings)
            Dim subCplex As Cplex = oplF.CreateCplex()

            Const nbItems As Integer = 5
            Dim items As IIntRange = masterCplex.IntRange(1, 5)
            Dim best As Double
            Dim curr As Double = Double.MaxValue
            Do
                best = curr

                masterCplex.ClearModel()

                Dim masterRC As OplRunConfiguration = oplF.CreateOplRunConfiguration(masterRC0.OplModel.ModelDefinition, masterDataElements)
                masterRC.Cplex = masterCplex
                masterRC.OplModel.Generate()

                Console.Out.WriteLine("Solve master.")
                If (masterCplex.Solve()) Then
                    curr = masterCplex.ObjValue
                    Console.Out.WriteLine("OBJECTIVE: " + curr.ToString("F"))
                    status = 0
                Else
                    Console.Out.WriteLine("No solution!")
                    status = 1
                End If

                REM prepare sub model data
                Dim subDataElements As OplDataElements = oplF.CreateOplDataElements()
                subDataElements.AddElement(masterDataElements.GetElement("RollWidth"))
                subDataElements.AddElement(masterDataElements.GetElement("Size"))
                subDataElements.AddElement(masterDataElements.GetElement("Duals"))
                REM get reduced costs and set them in sub problem
                Dim duals As INumMap = subDataElements.GetElement("Duals").AsNumMap()
                Dim i As Integer
                For i = 1 To nbItems
                    Dim forAll As IForAllRange = masterRC.OplModel.GetElement("ctFill").AsConstraintMap().Get(i)
                    duals.Set(i, masterCplex.GetDual(forAll))
                Next
                REM make sub model
                Dim subOpl As OplModel = oplF.CreateOplModel(subDef, subCplex)
                subOpl.AddDataSource(subDataElements)
                subOpl.Generate()

                Console.Out.WriteLine("Solve sub.")
                If (subCplex.Solve()) Then
                    Console.Out.WriteLine("OBJECTIVE: " + subCplex.ObjValue.ToString("F"))
                    status = 0
                Else
                    Console.Out.WriteLine("No solution!")
                    status = 1
                End If

                If (subCplex.ObjValue > -RC_EPS) Then Exit Do

                REM Add variable in master model
                Dim newFill As IIntMap = masterCplex.IntMap(items)
                For i = 1 To nbItems
                    Dim coef As Integer = subCplex.GetValue(subOpl.GetElement("Use").AsIntVarMap().Get(i))
                    newFill.Set(i, coef)
                Next
                Dim buf As ITupleBuffer = masterDataElements.GetElement("Patterns").AsTupleSet().MakeTupleBuffer(-1)
                buf.SetIntValue("id", masterDataElements.GetElement("Patterns").AsTupleSet().Size)
                buf.SetIntValue("cost", 1)
                buf.SetIntMapValue("fill", newFill)
                buf.Commit()

                subOpl.End()
                masterRC.End()
            Loop While (best <> curr And status = 0)
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
