' --------------------------------------------------------------------------
' File: QPex3.vb
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
' QPex3.vb - Entering and modifying a QP problem
'
' Example QPex3.vb illustrates how to enter and modify a QP problem 
' by using linear quadratic expressions.

Imports ILOG.Concert
Imports ILOG.CPLEX
Imports System.Collections


Public Class QPex3
   Public Shared Sub Main(ByVal args As String())
      Try
         ' create a QP problem
         Dim cplex As New Cplex()
         CreateQPModel(cplex)
         cplex.ExportModel("qp1ex3.lp")

         ' solve the QP problem
         SolveAndPrint(cplex, "Solving the QP problem ...")

         ' Modify the quadratic objective function
         ModifyQuadObjective(cplex)
         cplex.ExportModel("qp2ex3.lp")

         ' solve the modified QP problem
         SolveAndPrint(cplex, "Solving the modified QP problem ...")

         cplex.[End]()
      Catch e As ILOG.Concert.Exception
         System.Console.WriteLine("Concert exception '" + e.ToString + "' caught")
      End Try
   End Sub


   ' Creating a simple QP problem
   Friend Shared Function CreateQPModel(ByVal model As IMPModeler) As ILPMatrix
      Dim lp As ILPMatrix = model.AddLPMatrix()

      Dim lb As Double() = {0.0, 0.0, 0.0}
      Dim ub As Double() = {40.0, System.[Double].MaxValue, System.[Double].MaxValue}
      Dim x As INumVar() = model.NumVarArray(model.ColumnArray(lp, 3), lb, ub)
      Dim nvars As Integer = x.Length
      For j As Integer = 0 To nvars - 1
         x(j).Name = "x" & j
      Next

      ' - x0 +   x1 + x2 <= 20
      '   x0 - 3*x1 + x2 <= 30
      Dim lhs As Double() = {-System.[Double].MaxValue, -System.[Double].MaxValue}
      Dim rhs As Double() = {20.0, 30.0}
      Dim val As Double()() = {New Double() {-1.0, 1.0, 1.0}, New Double() {1.0, -3.0, 1.0}}
      Dim ind As Integer()() = {New Integer() {0, 1, 2}, New Integer() {0, 1, 2}}
      lp.AddRows(lhs, rhs, ind, val)

      ' minimize - x0 - x1 - x2 + x0*x0 + x1*x1 + x0*x1 + x1*x0 
      Dim objExpr As ILQNumExpr = model.LqNumExpr()
      For i As Integer = 0 To nvars - 1
         objExpr.AddTerm(-1.0, x(i))
         For j As Integer = 0 To nvars - 1
            objExpr.AddTerm(1.0, x(i), x(j))
         Next
      Next
      Dim obj As IObjective = model.Minimize(objExpr)
      model.Add(obj)

      ' Print out the objective function
      PrintObjective(obj)

      Return lp
   End Function


   ' Modifying all quadratic terms x[i]*x[j] 
   ' in the objective function.
   Friend Shared Sub ModifyQuadObjective(ByVal model As CplexModeler)
      Dim matrixEnum As IEnumerator = model.GetLPMatrixEnumerator()
      matrixEnum.MoveNext()
      Dim lp As ILPMatrix = DirectCast(matrixEnum.Current, ILPMatrix)
      Dim x As INumVar() = lp.NumVars
      Dim ncols As Integer = x.Length
      Dim obj As IObjective = model.GetObjective()

      ' Note that the quadratic expression in the objective
      ' is normalized: i.e., for all i != j, terms 
      ' c(i,j)*x[i]*x[j] + c(j,i)*x[j]*x[i] are normalized as
      ' (c(i,j) + c(j,i)) * x[i]*x[j], or 
      ' (c(i,j) + c(j,i)) * x[j]*x[i].
      ' Therefore you can only modify one of the terms 
      ' x[i]*x[j] or x[j]*x[i]. 
      ' If you modify both x[i]*x[j] and x[j]*x[i], then 
      ' the second modification will overwrite the first one.
      For i As Integer = 0 To ncols - 1
         model.SetQuadCoef(obj, x(i), x(i), i * i)
         For j As Integer = 0 To i - 1
            model.SetQuadCoef(obj, x(i), x(j), -2.0 * (i * j))
         Next
      Next

      ' Print out the objective function
      PrintObjective(obj)
   End Sub


   ' Print out the objective function.
   ' Note that the quadratic expression in the objective
   ' is normalized: i.E., for all i != j, terms 
   ' c(i,j)*x[i]*x[j] + c(j,i)*x[j]*x[i] is normalized as
   ' (c(i,j) + c(j,i)) * x[i]*x[j], or 
   ' (c(i,j) + c(j,i)) * x[j]*x[i].
   Friend Shared Sub PrintObjective(ByVal obj As IObjective)
      System.Console.WriteLine("obj: " & Convert.ToString(obj))

      ' Count the number of linear terms 
      ' in the objective function.
      Dim nlinterms As Integer = 0
      Dim len As ILinearNumExprEnumerator = DirectCast(obj.Expr, ILQNumExpr).GetLinearEnumerator()
      While len.MoveNext()
         nlinterms += 1
      End While

      ' Count the number of quadratic terms 
      ' in the objective function.
      Dim nquadterms As Integer = 0
      Dim nquaddiag As Integer = 0
      Dim qen As IQuadNumExprEnumerator = DirectCast(obj.Expr, ILQNumExpr).GetQuadEnumerator()

      While qen.MoveNext()
         nquadterms += 1
         Dim var1 As INumVar = qen.GetNumVar1()
         Dim var2 As INumVar = qen.GetNumVar2()
         If var1.Equals(var2) Then
            nquaddiag += 1
         End If
      End While

      System.Console.WriteLine("number of linear terms in the objective             : " & nlinterms)
      System.Console.WriteLine("number of quadratic terms in the objective          : " & nquadterms)
      System.Console.WriteLine("number of diagonal quadratic terms in the objective : " & nquaddiag)
      System.Console.WriteLine()
   End Sub


   ' Solve the current model and print results
   Friend Shared Sub SolveAndPrint(ByVal cplex As Cplex, ByVal msg As String)
      System.Console.WriteLine(msg)
      If cplex.Solve() Then
         System.Console.WriteLine()
         System.Console.WriteLine(("Solution status = " + cplex.GetStatus().ToString))
         System.Console.WriteLine(("Solution value  = " & cplex.ObjValue))

         Dim matrixEnum As IEnumerator = cplex.GetLPMatrixEnumerator()
         matrixEnum.MoveNext()
         Dim lp As ILPMatrix = DirectCast(matrixEnum.Current, ILPMatrix)
         Dim x As Double() = cplex.GetValues(lp)
         Dim nvars As Integer = x.Length
         For j As Integer = 0 To nvars - 1
            System.Console.WriteLine("Variable " & j & ": Value = " & x(j))
         Next
      End If
      System.Console.WriteLine()
   End Sub

End Class
