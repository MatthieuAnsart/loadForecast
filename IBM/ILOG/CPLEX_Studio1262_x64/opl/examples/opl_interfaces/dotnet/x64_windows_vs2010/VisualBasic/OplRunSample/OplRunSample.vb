REM ------------------------------------------------ -*- Visual Basic .Net -*-
REM File: OplRunSample.vb
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
REM Visual Basic version of OplRunSample.cpp of OPL distrib
REM --------------------------------------------------------------------------

Imports ILOG.OPL
Imports ILOG.Concert
Imports ILOG.CPLEX
Imports System.Collections
Imports System.IO



Module OplRunSample
    Dim _cl As CommandLine
    Dim _timer As Timer

    Sub Main()
        Try
            _cl = New CommandLine(Environment.GetCommandLineArgs)
            _timer = New Timer
            Run()
        Catch ex As System.Exception
            Console.Error.WriteLine(ex)
            Console.Error.WriteLine(ex.StackTrace)
        End Try
        Console.WriteLine("--Press <Enter> to exit--")
        Console.ReadLine()
    End Sub

    Function Run() As Integer
	Dim Status As Integer = 1
        Try
            OplFactory.DebugMode = True
            Dim oplF As OplFactory = New OplFactory

            If (Not _cl.CompileName Is Nothing) Then
                Dim modelSource As OplModelSource = oplF.CreateOplModelSource(_cl.ModelFileName)
                Dim compiler As OplCompiler = oplF.CreateOplCompiler()
                Dim ofs As StreamWriter = New StreamWriter(_cl.CompileName, FileMode.Create)
                compiler.Compile(modelSource, ofs)
                ofs.Close()
                Trace("compile")
                Return 0
            End If

            If (_cl.ModelFileName = "" & Not _cl.IsProject) Then
                Return 0
            End If

            Trace("initial")

            Dim errHandler As OplErrorHandler = oplF.CreateOplErrorHandler()
            Dim rc As OplRunConfiguration
            If (_cl.IsProject) Then
                Dim prj As OplProject = oplF.CreateOplProject(_cl.ProjectPath)
                rc = prj.MakeRunConfiguration(_cl.RunConfigurationName)
            Else
                If (_cl.DataFileNames.Count = 0) Then
                    rc = oplF.CreateOplRunConfiguration(_cl.ModelFileName)
                Else
                    Dim dataFiles As String()
                    dataFiles = CType(_cl.DataFileNames.ToArray(GetType(String)), String())
                    rc = oplF.CreateOplRunConfiguration(_cl.ModelFileName, dataFiles)
                End If
            End If
            rc.ErrorHandler = errHandler

            Dim opl As OplModel = rc.OplModel

            Dim settings As OplSettings = opl.Settings
            settings.IsWithLocations = True
            settings.IsWithNames = True
            settings.IsForceElementUsage = _cl.IsForceElementUsage

            If (opl.ModelDefinition.hasMain) Then
                opl.Main()
                Trace("main")
            Else
                opl.Generate()
                Trace("generate model")
                If (opl.hasCplex()) Then
                    If (Not _cl.ExportName Is Nothing) Then
                        opl.Cplex.ExportModel(_cl.ExportName)
                        Trace("export model", _cl.ExportName)
                    End If

                    If _cl.IsRelaxation Then
                        Console.Out.WriteLine("RELAXATIONS to obtain a feasable problem: ")
                        Dim count As Integer = opl.PrintRelaxation(Console.Out)
                        Console.Out.WriteLine("RELAXATIONS done.")
                        If (count > 0) Then
                            opl.PostProcess()
                            Trace("post process")
                            If (_cl.IsVerbose) Then
                                opl.PrintSolution(Console.Out)
                            End If
                        End If
                    ElseIf _cl.IsConflict Then
                        Console.Out.WriteLine("CONFLICTS to obtain a feasable problem: ")
                        opl.PrintConflict(Console.Out)
                        Console.Out.WriteLine("CONFLICTS done.")
                    Else
                        Dim result As Boolean = opl.Cplex.Solve()
                        If (result) Then
                            Trace("solve")
                            Console.Out.WriteLine()
                            Console.Out.WriteLine()
                            Console.Out.WriteLine("OBJECTIVE: " + Str(opl.Cplex.ObjValue))

                            opl.PostProcess()
                            Trace("post process")

                            If (_cl.IsVerbose) Then
                                opl.PrintSolution(Console.Out)
                            End If
                        Else
                            Trace("no solution")
                        End If
                    End If
                Else 'opl.hasCP()
                    Dim result As Boolean = opl.CP.Solve()
                    If (result) Then
                        Trace("solve")
                        Console.Out.WriteLine()
                        Console.Out.WriteLine()
                        If (opl.CP.hasObjective()) Then
                            Console.Out.WriteLine("OBJECTIVE: " + Str(opl.CP.ObjValue))
                        Else
                            Console.Out.WriteLine("OBJECTIVE: no objective")
                        End If
                        opl.PostProcess()
                        If (_cl.IsVerbose) Then
                            opl.PrintSolution(Console.Out)
                        Else
                            Trace("no solution")
                        End If

                    End If
                End If
                Trace("done")
            End If


            If (Not _cl.ExternalDataName Is Nothing) Then
                Dim ofs As StreamWriter = New StreamWriter(_cl.ExternalDataName, FileMode.Create)
                opl.PrintExternalData(ofs)
                ofs.Close()
                Trace("write external data", _cl.ExternalDataName)
            End If

            If (Not _cl.InternalDataName Is Nothing) Then
                Dim ofs As StreamWriter = New StreamWriter(_cl.InternalDataName, FileMode.Create)
                opl.PrintInternalData(ofs)
                ofs.Close()
                Trace("write internal data", _cl.InternalDataName)
            End If

            rc.End()
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
    End Function

    Sub Trace(ByVal title As String, ByVal info As String)
        Console.Out.WriteLine()
        Console.Out.Write("<<< " + title)
        If (Not info Is Nothing) Then
            Console.Out.Write(": " + info)
        End If
        If (_cl.IsVerbose) Then
            Console.Out.Write(", at " + Str(_timer.GetAbsoluteTime()) + "s" + ", took " + Str(_timer.GetTime()) + "s")
            _timer.Restart()
        End If
        Console.Out.WriteLine()
        Console.Out.WriteLine()
    End Sub

    Sub Trace(ByVal title As String)
        Trace(title, Nothing)
    End Sub

    Class CommandLine
        Public IsVerbose As Boolean = False
        Public IsForceElementUsage As Boolean = False
        Public IsRelaxation As Boolean = False
        Public IsConflict As Boolean = False
        Public ModelFileName As String
        Public DataFileNames As ArrayList = New ArrayList
        Public IsProject As Boolean = False
        Public ExportName As String
        Public CompileName As String
        Public ExternalDataName As String
        Public InternalDataName As String


        Public ReadOnly Property ProjectPath() As String
            Get
                If (IsProject) Then
                    Return ModelFileName
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Public ReadOnly Property RunConfigurationName() As String
            Get
                If (IsProject And DataFileNames.Count = 1) Then
                    Return DataFileNames.Item(0)
                Else
                    Return Nothing
                End If
            End Get
        End Property


        Sub New(ByVal args() As String)
            If (args.Length < 2) Then
                ModelFileName = "..\..\mulprod.mod"
                DataFileNames.Add("..\..\mulprod.dat")
            End If

            Dim i As Integer = 0
            For i = 1 To args.Length - 1
                If ("-h".Equals(args(i))) Then
                    Usage()
                ElseIf ("-p".Equals(args(i))) Then
                    IsProject = True
                ElseIf ("-v".Equals(args(i))) Then
                    IsVerbose = True
                ElseIf ("-e".Equals(args(i))) Then
                    i = i + 1
                    If (i < args.Length And args(i).Chars(0) <> "-") Then
                        ExportName = args(i)
                    Else
                        ExportName = "oplRunSample.lp"
                        i = i - 1
                    End If
                ElseIf ("-o".Equals(args(i))) Then
                    i = i + 1
                    If (i < args.Length And args(i).Chars(0) <> "-") Then
                        CompileName = args(i)
                    Else
                        Usage()
                    End If
                ElseIf ("-de".Equals(args(i))) Then
                    i = i + 1
                    If (i < args.Length And args(i).Chars(0) <> "-") Then
                        ExternalDataName = args(i)
                    Else
                        Usage()
                    End If
                ElseIf ("-di".Equals(args(i))) Then
                    i = i + 1
                    If (i < args.Length And args(i).Chars(0) <> "-") Then
                        InternalDataName = args(i)
                    Else
                        Usage()
                    End If
                ElseIf ("-f".Equals(args(i))) Then
                    IsForceElementUsage = True
                ElseIf ("-relax".Equals(args(i))) Then
                    IsRelaxation = True
                ElseIf ("-conflict".Equals(args(i))) Then
                    IsConflict = True
                ElseIf (args(i).Chars(0) = "-") Then
                    Console.Error.WriteLine("Unknown option: " + args(i))
                    Usage()
                Else
                    Exit For
                End If
            Next

            If (i >= args.Length & i > 1) Then
                Usage()
            End If
            If (i < args.Length) Then
                ModelFileName = args(i)
                Dim j As Integer
                For j = i + 1 To args.Length - 1
                    DataFileNames.Add(args(j))
                Next
            End If

            If ((IsProject And DataFileNames.Count > 1) Or (IsProject And args.Length < 3)) Then
                Usage()
            End If
        End Sub

        Sub Usage()
            Console.Error.WriteLine()
            Console.Error.WriteLine("Usage:")
            Console.Error.WriteLine("OplRunSample [options] model-file [data-file ...]")
            Console.Error.WriteLine("OplRunSample [options] -p project-path [run-configuration]")
            Console.Error.WriteLine("  options ")
            Console.Error.WriteLine("    -h               this help message")
            Console.Error.WriteLine("    -v               verbose")
            Console.Error.WriteLine("    -e [export-file] export model")
            Console.Error.WriteLine("    -de dat-file     write external data")
            Console.Error.WriteLine("    -di dat-file     write internal data")
            Console.Error.WriteLine("    -o output-file   compile model")
            Console.Error.WriteLine("    -f               force element usage")
            Console.Error.WriteLine("    -relax           calculate relaxations needed for feasable model")
            Console.Error.WriteLine("    -conflict        calculate a conflict for an infeasable model")

            Console.Error.WriteLine()
            End
        End Sub
    End Class

    Class Timer
        Dim _time As Integer = System.Environment.TickCount
        Dim _startTime As Integer = System.Environment.TickCount
        Sub Restart()
            _time = System.Environment.TickCount
        End Sub
        Function GetTime() As Double
            Return (System.Environment.TickCount - _time) / 1000
        End Function
        Function GetAbsoluteTime() As Double
            Return (System.Environment.TickCount - _startTime) / 1000
        End Function
    End Class

End Module
