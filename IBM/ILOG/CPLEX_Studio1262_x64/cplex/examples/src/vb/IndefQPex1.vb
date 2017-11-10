' --------------------------------------------------------------------------
' File: IndefQPex1.vb
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
' IndefQPex1.vb - Entering and optimizing an indefinite QP problem
' 


Imports ILOG.Concert
Imports ILOG.CPLEX


Public Class IndefQPex1
   Public Shared Sub Main(args As String())
      Try
         Dim cplex As New Cplex()
         Dim lp As ILPMatrix = PopulateByRow(cplex)

         Dim ind As Integer() = {0}
         Dim val As Double() = {1.0}

         ' When a non-convex objective function is present, CPLEX
         ' will raise an exception unless the parameter
         ' Cplex.Param.OptimalityTarget is set to accept
         ' first-order optimal solutions
         cplex.SetParam(Cplex.Param.OptimalityTarget, 2)

         ' CPLEX may converge to either local optimum
         SolveAndDisplay(cplex, lp)

         ' Add a constraint that cuts off the solution at (-1, 1)
         lp.AddRow(0.0, System.[Double].MaxValue, ind, val)
         SolveAndDisplay(cplex, lp)

         ' Remove the newly added constraint and add a new constraint
         ' with the opposite sense to cut off the solution at (1, 1)
         lp.RemoveRow(lp.Nrows - 1)
         lp.AddRow(-System.[Double].MaxValue, 0.0, ind, val)
         SolveAndDisplay(cplex, lp)

         cplex.ExportModel("indefqpex1.lp")
         cplex.[End]()
      Catch e As ILOG.Concert.Exception
         System.Console.WriteLine(("Concert exception caught: " + e.ToString))
      End Try
   End Sub

   Friend Shared Function PopulateByRow(model As IMPModeler) As ILPMatrix
      Dim lp As ILPMatrix = model.AddLPMatrix()

      Dim lb As Double() = {-1.0, 0.0}
      Dim ub As Double() = {1.0, 1.0}
      Dim x As INumVar() = model.NumVarArray(model.ColumnArray(lp, 2), lb, ub)

      ' - x0 +   x1 + x2 <= 20
      '   x0 - 3*x1 + x2 <= 30
      Dim lhs As Double() = {0.0, 0.0}
      Dim rhs As Double() = {System.[Double].MaxValue, System.[Double].MaxValue}
      Dim val As Double()() = {New Double() {-1.0, 1.0}, New Double() {1.0, 1.0}}
      Dim ind As Integer()() = {New Integer() {0, 1}, New Integer() {0, 1}}
      lp.AddRows(lhs, rhs, ind, val)

      ' Q = 0.5 ( 33*x0*x0 + 22*x1*x1 + 11*x2*x2 - 12*x0*x1 - 23*x1*x2 )
      Dim x00 As INumExpr = model.Prod(-3.0, x(0), x(0))
      Dim x11 As INumExpr = model.Prod(-3.0, x(1), x(1))
      Dim x01 As INumExpr = model.Prod(-1.0, x(0), x(1))
      Dim Q As INumExpr = model.Prod(0.5, model.Sum(x00, x11, x01))

      ' minimize Q
      model.Add(model.Minimize(Q))

      Return (lp)
   End Function

   Friend Shared Sub SolveAndDisplay(cplex As Cplex, lp As ILPMatrix)

      If cplex.Solve() Then
         Dim x As Double() = cplex.GetValues(lp)
         Dim dj As Double() = cplex.GetReducedCosts(lp)
         Dim pi As Double() = cplex.GetDuals(lp)
         Dim slack As Double() = cplex.GetSlacks(lp)

         System.Console.WriteLine(("Solution status = " + cplex.GetStatus().ToString))
         System.Console.WriteLine(("Solution value  = " + cplex.ObjValue.ToString))

         Dim nvars As Integer = x.Length
         For j As Integer = 0 To nvars - 1
            System.Console.WriteLine("Variable " & j & ": Value = " & x(j) & " Reduced cost = " & dj(j))
         Next

         Dim ncons As Integer = slack.Length
         For i As Integer = 0 To ncons - 1
            System.Console.WriteLine("Constraint " & i & ": Slack = " & slack(i) & " Pi = " & pi(i))
         Next
      End If
   End Sub
End Class



