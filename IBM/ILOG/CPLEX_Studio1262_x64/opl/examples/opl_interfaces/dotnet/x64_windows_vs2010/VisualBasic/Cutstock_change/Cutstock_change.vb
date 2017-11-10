REM ------------------------------------------------ -*- Visual Basic .Net -*-
REM File: cutstock.vb
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
REM Visual Basic version of Cutstock_change.cpp of OPL distrib
REM --------------------------------------------------------------------------

Imports ILOG.OPL
Imports ILOG.Concert
Imports ILOG.CPLEX
Imports System.Collections

Module Cutstock_change
  Const DATADIR As String = "../.."
  Const RC_EPS As Double = 0.000001
  Dim status As Integer = 127

  Sub Main()
        Try
            OplFactory.DebugMode = True
            Dim oplF As OplFactory = New OplFactory
            Dim errHandler As OplErrorHandler = oplF.CreateOplErrorHandler()
            Dim settings As OplSettings = oplF.CreateOplSettings(errHandler)

            Dim masterCplex As Cplex = oplF.CreateCplex()
            masterCplex.SetOut(Nothing)

            Dim errorHandler As OplErrorHandler = oplF.CreateOplErrorHandler()
            Dim masterRC As OplRunConfiguration = oplF.CreateOplRunConfiguration(DATADIR + "/cutstock_change.mod", DATADIR + "/cutstock_change.dat")
            masterRC.ErrorHandler = errorHandler
            masterRC.Cplex = masterCplex
            Dim masterOpl As OplModel = masterRC.OplModel
            masterOpl.Generate()
            Dim masterDataElements As OplDataElements = masterOpl.MakeDataElements()

            Dim subSource As OplModelSource = oplF.CreateOplModelSource(DATADIR + "/cutstock-sub.mod")
            Dim subDef As OplModelDefinition = oplF.CreateOplModelDefinition(subSource, settings)
            Dim subCplex As Cplex = oplF.CreateCplex
            subCplex.SetOut(Nothing)

            Dim nWdth As Integer = masterDataElements.GetElement("Amount").AsIntMap().Size
            Dim masterVars As ArrayList = New ArrayList
            Dim cuts As INumVarMap = masterOpl.GetElement("Cut").AsNumVarMap()
            Dim i As Integer
            For i = 1 To nWdth
                masterVars.Add(cuts.Get(i))
            Next


            Dim best As Double
            Dim curr As Double = Double.MaxValue
            Do
                best = curr

                REM Make master model 
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
                For i = 1 To nWdth
                    Dim forAll As IForAllRange = masterOpl.GetElement("ctFill").AsConstraintMap().Get(i)
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
                Dim newVar As INumVar = masterCplex.NumVar(0, Double.MaxValue)
                Dim masterObj As IObjective = masterOpl.Objective
                masterCplex.SetLinearCoef(masterObj, newVar, 1)
                For i = 1 To nWdth
                    Dim coef As Double = subCplex.GetValue(subOpl.GetElement("Use").AsIntVarMap().Get(i))
                    Dim forAll As IForAllRange = masterOpl.GetElement("ctFill").AsConstraintMap().Get(i)
                    masterCplex.SetLinearCoef(forAll, newVar, coef)
                Next
                masterVars.Add(newVar)

                subOpl.End()
            Loop While (best <> curr And status = 0)

            Dim masterVarsA() As INumVar
            masterVarsA = masterVars.ToArray(GetType(INumVar))

            masterCplex.Add(masterCplex.Conversion(masterVarsA, NumVarType.Int))
            If (masterCplex.Solve()) Then
                Console.Out.WriteLine("OBJECTIVE: " + masterCplex.ObjValue.ToString("F"))
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
