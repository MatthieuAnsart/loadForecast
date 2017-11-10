' --------------------------------------------------------------------------
' File: BendersATSP.vb
' Version 12.6.2  
' --------------------------------------------------------------------------
' Licensed Materials - Property of IBM
' 5725-A06 5725-A29 5724-Y48 5724-Y49 5724-Y54 5724-Y55 5655-Y21
' Copyright IBM Corporation 2001, 2015. All Rights Reserved.
' 
' US Government Users Restricted Rights - Use, duplication or
' disclosure restricted by GSA ADP Schedule Contract with
' IBM Corp.
' --------------------------------------------------------------------------
' 
' Example BendersATSP.vb solves a flow MILP model for an
' Asymmetric Traveling Salesman Problem (ATSP) instance
' through Benders decomposition.
' 
' The arc costs of an ATSP instance are read from an input file.
' The flow MILP model is decomposed into a master ILP and a worker LP.
' 
' The master ILP is then solved by adding Benders' cuts during
' the branch-and-cut process via the cut callback classes 
' Cplex.LazyConstraintCallback and Cplex.UserCutCallback.
' The cut callbacks add to the master ILP violated Benders' cuts
' that are found by solving the worker LP.
' 
' The example allows the user to decide if Benders' cuts have to be separated:
' 
' a) Only to separate integer infeasible solutions.
' In this case, Benders' cuts are treated as lazy constraints through the
' class Cplex.LazyConstraintCallback.
' 
' b) Also to separate fractional infeasible solutions.
' In this case, Benders' cuts are treated as lazy constraints through the
' class Cplex.LazyConstraintCallback.
' In addition, Benders' cuts are also treated as user cuts through the
' class Cplex.UserCutCallback.
' 
' 
' To run this example, command line arguments are required:
'     BendersATSP {0|1} [filename]
' where
'     0         Indicates that Benders' cuts are only used as lazy constraints,
'               to separate integer infeasible solutions.
'     1         Indicates that Benders' cuts are also used as user cuts,
'               to separate fractional infeasible solutions.
' 
'     filename  Is the name of the file containing the ATSP instance (arc costs).
'               If filename is not specified, the instance
'               ../../../../examples/data/atsp.dat is read
' 
' 
' ATSP instance defined on a directed graph G = (V, A)
' - V = {0, ..., n-1}, V0 = V \ {0}
' - A = {(i,j) : i in V, j in V, i != j }
' - forall i in V: delta+(i) = {(i,j) in A : j in V}
' - forall i in V: delta-(i) = {(j,i) in A : j in V}
' - c(i,j) = traveling cost associated with (i,j) in A
' 
' Flow MILP model
' 
' Modeling variables:
' forall (i,j) in A:
'    x(i,j) = 1, if arc (i,j) is selected
'           = 0, otherwise
' forall k in V0, forall (i,j) in A:
'    y(k,i,j) = flow of the commodity k through arc (i,j)
' 
' Objective:
' minimize sum((i,j) in A) c(i,j) * x(i,j)
' 
' Degree constraints:
' forall i in V: sum((i,j) in delta+(i)) x(i,j) = 1
' forall i in V: sum((j,i) in delta-(i)) x(j,i) = 1
' 
' Binary constraints on arc variables:
' forall (i,j) in A: x(i,j) in {0, 1}
' 
' Flow constraints:
' forall k in V0, forall i in V:
'    sum((i,j) in delta+(i)) y(k,i,j) - sum((j,i) in delta-(i)) y(k,j,i) = q(k,i)
'    where q(k,i) =  1, if i = 0
'                 = -1, if k == i
'                 =  0, otherwise
' 
' Capacity constraints:
' forall k in V0, for all (i,j) in A: y(k,i,j) <= x(i,j)
' 
' Nonnegativity of flow variables:
' forall k in V0, for all (i,j) in A: y(k,i,j) >= 0
' 

Imports ILOG.Concert
Imports ILOG.CPLEX


Public Class BendersATSP

   ' The class BendersLazyConsCallback 
   ' allows to add Benders' cuts as lazy constraints.
   '
   Public Class BendersLazyConsCallback
      Inherits Cplex.LazyConstraintCallback
      Friend x As IIntVar()()
      Friend workerLP As WorkerLP
      Friend numNodes As Integer

      Friend Sub New(ByVal x As IIntVar()(), ByVal workerLP As WorkerLP)
         Me.x = x
         Me.workerLP = workerLP
         numNodes = x.Length
      End Sub 'New

      Public Overrides Sub Main()
         ' Get the current x solution

         Dim sol As Double()() = New Double(numNodes - 1)() {}
         For i As Integer = 0 To numNodes - 1
            sol(i) = GetValues(x(i))
         Next

         ' Benders' cut separation

         Dim cut As IRange = workerLP.Separate(sol, x)
         If cut IsNot Nothing Then
            Add(cut)
         End If
      End Sub 'Main

   End Class 'BendersLazyConsCallback 


   ' The class BendersUserCutCallback 
   ' allows to add Benders' cuts as user cuts.
   '
   Public Class BendersUserCutCallback
      Inherits Cplex.UserCutCallback
      Friend x As IIntVar()()
      Friend workerLP As WorkerLP
      Friend numNodes As Integer

      Friend Sub New(ByVal x As IIntVar()(), ByVal workerLP As WorkerLP)
         Me.x = x
         Me.workerLP = workerLP
         numNodes = x.Length
      End Sub 'New

      Public Overrides Sub Main()
         ' Skip the separation if not at the end of the cut loop

         If Not IsAfterCutLoop() Then
            Return
         End If

         ' Get the current x solution

         Dim sol As Double()() = New Double(numNodes - 1)() {}
         For i As Integer = 0 To numNodes - 1
            sol(i) = GetValues(x(i))
         Next

         ' Benders' cut separation

         Dim cut As IRange = workerLP.Separate(sol, x)
         If cut IsNot Nothing Then
            Add(cut)
         End If
      End Sub 'Main

   End Class 'BendersUserCutCallback


   ' Data class to read an ATSP instance from an input file
   '
   Friend Class Data
      Friend numNodes As Integer
      Friend arcCost As Double()()

      Friend Sub New(ByVal fileName As String)
         Dim reader As New InputDataReader(fileName)
         arcCost = reader.ReadDoubleArrayArray()
         numNodes = arcCost.Length
         For i As Integer = 0 To numNodes - 1
            If arcCost(i).Length <> numNodes Then
               Throw New ILOG.Concert.Exception("inconsistent data in file " & fileName)
            End If
            arcCost(i)(i) = 0.0
         Next
      End Sub 'New
   End Class 'Data


   ' This class builds the worker LP (i.e., the dual of flow constraints and
   ' capacity constraints of the flow MILP) and allows to separate violated
   ' Benders' cuts.
   '
   Friend Class WorkerLP
      Friend cplex As Cplex
      Friend numNodes As Integer
      Friend v As INumVar()()()
      Friend u As INumVar()()

      ' The constructor sets up the Cplex instance to solve the worker LP, 
      ' and creates the worker LP (i.e., the dual of flow constraints and
      ' capacity constraints of the flow MILP)
      '
      ' Modeling variables:
      ' forall k in V0, i in V:
      '    u(k,i) = dual variable associated with flow constraint (k,i)
      '
      ' forall k in V0, forall (i,j) in A:
      '    v(k,i,j) = dual variable associated with capacity constraint (k,i,j)
      '
      ' Objective:
      ' minimize sum(k in V0) sum((i,j) in A) x(i,j) * v(k,i,j)
      '          - sum(k in V0) u(k,0) + sum(k in V0) u(k,k)
      '
      ' Constraints:
      ' forall k in V0, forall (i,j) in A: u(k,i) - u(k,j) <= v(k,i,j)
      '
      ' Nonnegativity on variables v(k,i,j)
      ' forall k in V0, forall (i,j) in A: v(k,i,j) >= 0
      '
      Friend Sub New(ByVal numNodes As Integer)
         Me.numNodes = numNodes
         Dim i As Integer, j As Integer, k As Integer

         ' Set up Cplex instance to solve the worker LP

         cplex = New Cplex()
         cplex.SetOut(Nothing)

         ' Turn off the presolve reductions and set the CPLEX optimizer
         ' to solve the worker LP with primal simplex method.

         cplex.SetParam(Cplex.Param.Preprocessing.Reduce, 0)
         cplex.SetParam(Cplex.Param.RootAlgorithm, Cplex.Algorithm.Primal)

         ' Create variables v(k,i,j) forall k in V0, (i,j) in A
         ' For simplicity, also dummy variables v(k,i,i) are created.
         ' Those variables are fixed to 0 and do not partecipate to 
         ' the constraints.

         v = New INumVar(numNodes - 2)()() {}
         For k = 1 To numNodes - 1
            v(k - 1) = New INumVar(numNodes - 1)() {}
            For i = 0 To numNodes - 1
               v(k - 1)(i) = New INumVar(numNodes - 1) {}
               For j = 0 To numNodes - 1
                  v(k - 1)(i)(j) = cplex.NumVar(0.0, System.[Double].MaxValue, "v." & k & "." & i & "." & j)
                  cplex.Add(v(k - 1)(i)(j))
               Next
               v(k - 1)(i)(i).UB = 0.0
            Next
         Next

         ' Create variables u(k,i) forall k in V0, i in V

         u = New INumVar(numNodes - 2)() {}
         For k = 1 To numNodes - 1
            u(k - 1) = New INumVar(numNodes - 1) {}
            For i = 0 To numNodes - 1
               u(k - 1)(i) = cplex.NumVar(-System.[Double].MaxValue, System.[Double].MaxValue, "u." & k & "." & i)
               cplex.Add(u(k - 1)(i))
            Next
         Next

         ' Initial objective function is empty

         cplex.AddMinimize()

         ' Add constraints:
         ' forall k in V0, forall (i,j) in A: u(k,i) - u(k,j) <= v(k,i,j)

         For k = 1 To numNodes - 1
            For i = 0 To numNodes - 1
               For j = 0 To numNodes - 1
                  If i <> j Then
                     Dim expr As ILinearNumExpr = cplex.LinearNumExpr()
                     expr.AddTerm(v(k - 1)(i)(j), -1.0)
                     expr.AddTerm(u(k - 1)(i), 1.0)
                     expr.AddTerm(u(k - 1)(j), -1.0)
                     cplex.AddLe(expr, 0.0)
                  End If
               Next
            Next
         Next

      End Sub 'New

      Public Sub [End]()
         cplex.[End]()
      End Sub '[End]

      ' This method separates Benders' cuts violated by the current x solution.
      ' Violated cuts are found by solving the worker LP
      '
      Public Function Separate(ByVal xSol As Double()(), ByVal x As IIntVar()()) As IRange

         Dim i As Integer, j As Integer, k As Integer

         Dim cut As IRange = Nothing

         ' Update the objective function in the worker LP:
         ' minimize sum(k in V0) sum((i,j) in A) x(i,j) * v(k,i,j)
         '          - sum(k in V0) u(k,0) + sum(k in V0) u(k,k)

         Dim objExpr As ILinearNumExpr = cplex.LinearNumExpr()
         For k = 1 To numNodes - 1
            For i = 0 To numNodes - 1
               For j = 0 To numNodes - 1
                  objExpr.AddTerm(v(k - 1)(i)(j), xSol(i)(j))
               Next
            Next
         Next
         For k = 1 To numNodes - 1
            objExpr.AddTerm(u(k - 1)(k), 1.0)
            objExpr.AddTerm(u(k - 1)(0), -1.0)
         Next
         cplex.GetObjective().Expr = objExpr

         ' Solve the worker LP

         cplex.Solve()

         ' A violated cut is available iff the solution status is Unbounded 

         If cplex.GetStatus().Equals(Cplex.Status.Unbounded) Then

            ' Get the violated cut as an unbounded ray of the worker LP

            Dim rayExpr As ILinearNumExpr = cplex.Ray

            ' Compute the cut from the unbounded ray. The cut is:
            ' sum((i,j) in A) (sum(k in V0) v(k,i,j)) * x(i,j) >=
            ' sum(k in V0) u(k,0) - u(k,k)

            Dim cutLhs As ILinearNumExpr = cplex.LinearNumExpr()
            Dim cutRhs As Double = 0.0
            Dim rayEnum As ILinearNumExprEnumerator = rayExpr.GetLinearEnumerator()

            While rayEnum.MoveNext()
               Dim var As INumVar = rayEnum.NumVar
               Dim varFound As Boolean = False
               k = 1
               While k < numNodes AndAlso Not varFound
                  i = 0
                  While i < numNodes AndAlso Not varFound
                     j = 0
                     While j < numNodes AndAlso Not varFound
                        If var.Equals(v(k - 1)(i)(j)) Then
                           cutLhs.AddTerm(x(i)(j), rayEnum.Value)
                           varFound = True
                        End If
                        j += 1
                     End While
                     i += 1
                  End While
                  k += 1
               End While
               k = 1
               While k < numNodes AndAlso Not varFound
                  i = 0
                  While i < numNodes AndAlso Not varFound
                     If var.Equals(u(k - 1)(i)) Then
                        If i = 0 Then
                           cutRhs += rayEnum.Value
                        ElseIf i = k Then
                           cutRhs -= rayEnum.Value
                        End If
                        varFound = True
                     End If
                     i += 1
                  End While
                  k += 1
               End While
            End While

            cut = cplex.Ge(cutLhs, cutRhs)
         End If

         Return cut
      End Function 'Separate

   End Class 'WorkerLP


   ' This method creates the master ILP (arc variables x and degree constraints).
   '
   ' Modeling variables:
   ' forall (i,j) in A:
   '    x(i,j) = 1, if arc (i,j) is selected
   '           = 0, otherwise
   '
   ' Objective:
   ' minimize sum((i,j) in A) c(i,j) * x(i,j)
   '
   ' Degree constraints:
   ' forall i in V: sum((i,j) in delta+(i)) x(i,j) = 1
   ' forall i in V: sum((j,i) in delta-(i)) x(j,i) = 1
   '
   ' Binary constraints on arc variables:
   ' forall (i,j) in A: x(i,j) in {0, 1}
   '
   Friend Shared Sub CreateMasterILP(ByVal model As IModeler, ByVal data As Data, ByVal x As IIntVar()())
      Dim i As Integer, j As Integer
      Dim numNodes As Integer = data.numNodes

      ' Create variables x(i,j) for (i,j) in A 
      ' For simplicity, also dummy variables x(i,i) are created.
      ' Those variables are fixed to 0 and do not partecipate to 
      ' the constraints.

      For i = 0 To numNodes - 1
         x(i) = New IIntVar(numNodes - 1) {}
         For j = 0 To numNodes - 1
            x(i)(j) = model.BoolVar("x." & i & "." & j)
            model.Add(x(i)(j))
         Next
         x(i)(i).UB = 0
      Next

      ' Create objective function: minimize sum((i,j) in A ) c(i,j) * x(i,j)

      Dim objExpr As ILinearNumExpr = model.LinearNumExpr()
      For i = 0 To numNodes - 1
         objExpr.Add(model.ScalProd(x(i), data.arcCost(i)))
      Next
      model.AddMinimize(objExpr)

      ' Add the out degree constraints.
      ' forall i in V: sum((i,j) in delta+(i)) x(i,j) = 1

      For i = 0 To numNodes - 1
         Dim expr As ILinearNumExpr = model.LinearNumExpr()
         For j = 0 To i - 1
            expr.AddTerm(x(i)(j), 1.0)
         Next
         For j = i + 1 To numNodes - 1
            expr.AddTerm(x(i)(j), 1.0)
         Next
         model.AddEq(expr, 1.0)
      Next

      ' Add the in degree constraints.
      ' forall i in V: sum((j,i) in delta-(i)) x(j,i) = 1

      For i = 0 To numNodes - 1
         Dim expr As ILinearNumExpr = model.LinearNumExpr()
         For j = 0 To i - 1
            expr.AddTerm(x(j)(i), 1.0)
         Next
         For j = i + 1 To numNodes - 1
            expr.AddTerm(x(j)(i), 1.0)
         Next
         model.AddEq(expr, 1.0)
      Next

   End Sub 'CreateMasterILP

   Public Shared Sub Main(ByVal args As String())
      Try
         Dim fileName As String = "../../../../examples/data/atsp.dat"

         ' Check the command line arguments 

         If args.Length <> 1 AndAlso args.Length <> 2 Then
            Usage()
            Return
         End If

         If Not (args(0).Equals("0") OrElse args(0).Equals("1")) Then
            Usage()
            Return
         End If

         Dim sepFracSols As Boolean = (If(args(0).ToCharArray()(0) = "0"c, False, True))

         If sepFracSols Then
            System.Console.WriteLine("Benders' cuts separated to cut off: Integer and fractional infeasible solutions.")
         Else
            System.Console.WriteLine("Benders' cuts separated to cut off: Only integer infeasible solutions.")
         End If

         If args.Length = 2 Then
            fileName = args(1)
         End If

         ' Read arc costs from data file (17 city problem)

         Dim data As New Data(fileName)

         ' create master ILP

         Dim numNodes As Integer = data.numNodes
         Dim cplex As New Cplex()
         Dim x As IIntVar()() = New IIntVar(numNodes - 1)() {}
         CreateMasterILP(cplex, data, x)

         ' Create workerLP for Benders' cuts separation

         Dim workerLP As New WorkerLP(numNodes)

         ' Set up the cut callback to be used for separating Benders' cuts

         cplex.SetParam(Cplex.Param.Preprocessing.Presolve, False)

         ' Set the maximum number of threads to 1. 
         ' This instruction is redundant: If MIP control callbacks are registered, 
         ' then by default CPLEX uses 1 (one) thread only.
         ' Note that the current example may not work properly if more than 1 threads 
         ' are used, because the callback functions modify shared global data.
         ' We refer the user to the documentation to see how to deal with multi-thread 
         ' runs in presence of MIP control callbacks. 

         cplex.SetParam(Cplex.Param.Threads, 1)

         ' Turn on traditional search for use with control callbacks

         cplex.SetParam(Cplex.Param.MIP.Strategy.Search, cplex.MIPSearch.Traditional)

         cplex.Use(New BendersLazyConsCallback(x, workerLP))
         If sepFracSols Then
            cplex.Use(New BendersUserCutCallback(x, workerLP))
         End If

         ' Solve the model and write out the solution

         If cplex.Solve() Then
            System.Console.WriteLine()
            System.Console.WriteLine("Solution status: " + cplex.GetStatus().ToString)
            System.Console.WriteLine("Objective value: " & cplex.ObjValue)

            If cplex.GetStatus().Equals(cplex.Status.Optimal) Then
               ' Write out the optimal tour

               Dim i As Integer, j As Integer
               Dim sol As Double()() = New Double(numNodes - 1)() {}
               Dim succ As Integer() = New Integer(numNodes - 1) {}
               For j = 0 To numNodes - 1
                  succ(j) = -1
               Next

               For i = 0 To numNodes - 1
                  sol(i) = cplex.GetValues(x(i))
                  For j = 0 To numNodes - 1
                     If sol(i)(j) > 0.001 Then
                        succ(i) = j
                     End If
                  Next
               Next

               System.Console.WriteLine("Optimal tour:")
               i = 0
               While succ(i) <> 0
                  System.Console.Write(i & ", ")
                  i = succ(i)
               End While
               System.Console.WriteLine(i)
            Else
               System.Console.WriteLine("Solution status is not Optimal")
            End If
         Else
            System.Console.WriteLine("No solution available")
         End If

         workerLP.[End]()
         cplex.[End]()

      Catch ex As ILOG.Concert.Exception
         System.Console.WriteLine("Concert Error: " + ex.ToString)
      Catch ex As InputDataReader.InputDataReaderException
         System.Console.WriteLine("Data Error: " + ex.ToString)
      Catch ex As System.IO.IOException
         System.Console.WriteLine("IO Error: " + ex.ToString)
      End Try

   End Sub 'Main

   Private Shared Sub Usage()
      System.Console.WriteLine("Usage:     BendersATSP {0|1} [filename]")
      System.Console.WriteLine(" 0:        Benders' cuts only used as lazy constraints,")
      System.Console.WriteLine("           to separate integer infeasible solutions.")
      System.Console.WriteLine(" 1:        Benders' cuts also used as user cuts,")
      System.Console.WriteLine("           to separate fractional infeasible solutions.")
      System.Console.WriteLine(" filename: ATSP instance file name.")
      System.Console.WriteLine("           File ../../../../examples/data/atsp.dat used " & "if no name is provided.")
   End Sub 'Usage 

End Class 'BendersATSP
