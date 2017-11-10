' --------------------------------------------------------------------------
' File: SocpEx1.vb
' Version 12.6.2
' --------------------------------------------------------------------------
' Licensed Materials - Property of IBM
' 5725-A06 5725-A29 5724-Y48 5724-Y49 5724-Y54 5724-Y55 5655-Y21
' Copyright IBM Corporation 2003, 2015. All Rights Reserved.
'
' US Government Users Restricted Rights - Use, duplication or
' disclosure restricted by GSA ADP Schedule Contract with
' IBM Corp.
' --------------------------------------------------------------------------

' SocpEx1.vb -- Solve a second order cone program to optimality, fetch
'               the dual values and test that the primal and dual solution
'               vectors returned by CPLEX satisfy the KKT conditions.
'               The problems that this code can handle are second order
'               cone programs in standard form. A second order cone
'               program in standard form is a problem of the following
'               type (c' is the transpose of c):
'                 min c1'x1 + ... + cr'xr
'                   A1 x1 + ... + Ar xr = b
'                   xi in second order cone (SOC)
'               where xi is a vector of length ni. Note that the
'               different xi are orthogonal. The constraint "xi in SOC"
'               for xi=(xi[1], ..., xi[ni]) is
'                   xi[1] >= |(xi[2],...,xi[ni])|
'               where |.| denotes the Euclidean norm. In CPLEX such a
'               constraint is formulated as
'                   -xi[1]^2 + xi[2]^2 + ... + xi[ni]^2 <= 0
'                    xi[1]                              >= 0
'                              xi[2], ..., xi[ni] free
'               Note that if ni = 1 then the second order cone constraint
'               reduces to xi[1] >= 0.

Imports ILOG.CPLEX
Imports ILOG.Concert

Imports System.Collections.Generic

Public NotInheritable Class SocpEx1
    ' Tolerance for testing KKT conditions.
    Private Shared ReadOnly TESTTOL As Double = 0.000000001
    ' Tolerance for barrier convergence.
    Private Shared ReadOnly CONVTOL As Double = 0.000000001

    ' A comparator to compare two instances of IRange by name.
    Private Class RangeCompare : Implements IComparer(Of IRange)
        Public Function Compare(ByVal a1 As IRange, ByVal a2 As IRange) As Integer Implements IComparer(Of IRange).Compare
            Return a1.Name.CompareTo(a2.Name)
        End Function
    End Class
    Private Shared RangeComparator As New RangeCompare()

    ' A comparator to compare two instances of INumVar by name.
    Private Class NumVarCompare : Implements IComparer(Of INumVar)
        Public Function Compare(ByVal a1 As INumVar, ByVal a2 As INumVar) As Integer Implements IComparer(Of INumVar).Compare
            Return a1.Name.CompareTo(a2.Name)
        End Function
    End Class
    Private Shared NumVarComparator As New NumVarCompare()


    ' Marks variables that are in cone constraints but are not the cone
    ' constraint's cone head. 
    Private Shared NOT_CONE_HEAD As IRange
    ' Marks variables that are not in any cone constraint.
    Private Shared NOT_IN_CONE As IRange

    ' NOTE: CPLEX does not provide a function to directly get the dual
    '       multipliers for second order cone constraint. However, we can
    '
    '      This involves checking for cone top issues and carefule numerics
    '      for general QCPs.
    '      However, for SOCP we can do something simpler: we can read those
    '      multipliers directly from the dual slacks for the
    '      cone head variables. For a second order cone constraint
    '         x[1] >= |(x[2], ..., x[n])|
    '      the dual multiplier is the dual slack value for x[1].
    Private Shared Function Getsocpconstrmultipliers(ByRef cplex As Cplex, ByRef vars As ICollection(Of INumVar), ByRef rngs As ICollection(Of IRange), ByRef dslack As IDictionary(Of INumVar, System.Double)) As IDictionary(Of IRange, System.Double)
        ' Compute the full dual slack.
        Dim dense As IDictionary(Of INumVar, System.Double) = New SortedDictionary(Of INumVar, System.Double)(NumVarComparator)
        For Each v As INumVar In vars
            dense(v) = cplex.GetReducedCost(v)
        Next v
        For Each r As IRange In rngs
            Dim e As INumExpr = r.Expr
            If (TypeOf e Is IQuadNumExpr) AndAlso DirectCast(e, IQuadNumExpr).GetQuadEnumerator().MoveNext() Then
                ' Quadratic constraint: pick up dual slack vector
                Dim l As ILinearNumExprEnumerator = DirectCast(cplex.GetQCDSlack(r), ILinearNumExpr).GetLinearEnumerator()
                While l.MoveNext
                    dense(l.NumVar) = dense(l.NumVar) + l.Value
                End While
            End If
        Next r

        ' Find the cone head variables and pick up the dual slack for them.
        Dim socppi As IDictionary(Of IRange, System.Double) = New SortedDictionary(Of IRange, System.Double)(RangeComparator)
        For Each r As IRange In rngs
            Dim e As INumExpr = r.Expr
            If (TypeOf e Is IQuadNumExpr) AndAlso DirectCast(e, IQuadNumExpr).GetQuadEnumerator().MoveNext() Then
                ' Quadratic constraint: pick up dual slack vector
                Dim q As IQuadNumExprEnumerator = DirectCast(e, IQuadNumExpr).GetQuadEnumerator()
                While q.MoveNext
                    If q.Value < 0 Then
                        socppi(r) = dense(q.NumVar1)
                        Exit While
                    End If
                End While
            End If
        Next r

        ' Fill in the dual slack if the caller asked for it
        If dslack IsNot Nothing Then
            dslack.Clear()
            For Each v As INumVar In dense.Keys
                dslack(v) = dense(v)
            Next v
        End If

        Return socppi
    End Function

    ' Test KKT conditions on the solution.
    ' The function returns true if the tested KKT conditions are satisfied
    ' and false otherwise.
    Private Shared Function Checkkkt(ByRef cplex As Cplex, ByRef obj As IObjective, ByRef vars As ICollection(Of INumVar), ByRef rngs As ICollection(Of IRange), ByRef cone As IDictionary(Of INumVar, IRange), ByVal tol As Double) As Boolean
        Dim err As System.IO.TextWriter = cplex.Output()
        Dim output As System.IO.TextWriter = cplex.Output()

        Dim dslack As IDictionary(Of INumVar, System.Double) = New SortedDictionary(Of INumVar, System.Double)(NumVarComparator)
        Dim x As IDictionary(Of INumVar, System.Double) = New SortedDictionary(Of INumVar, System.Double)(NumVarComparator)
        Dim pi As IDictionary(Of IRange, System.Double) = Nothing
        Dim slack As IDictionary(Of IRange, System.Double) = New SortedDictionary(Of IRange, System.Double)(RangeComparator)

        ' Read primal and dual solution information.
        For Each v As INumVar In vars
            x(v) = cplex.GetValue(v)
        Next v
        pi = Getsocpconstrmultipliers(cplex, vars, rngs, dslack)
        For Each r As IRange In rngs
            slack(r) = cplex.GetSlack(r)
            Dim e As INumExpr = r.Expr
            If Not ((TypeOf e Is IQuadNumExpr) AndAlso DirectCast(e, IQuadNumExpr).GetQuadEnumerator().MoveNext()) Then
                ' Linear constraint: get the dual value
                pi(r) = cplex.GetDual(r)
            End If
        Next r
        ' Pick up dual values for quadratic constraints.
        For Each v As INumVar In vars
            If cone(v) IsNot NOT_CONE_HEAD AndAlso cone(v) IsNot NOT_IN_CONE Then
                pi(cone(v)) = dslack(v)
            End If
        Next v

        ' Print out the data we just fetched.
        output.Write("x      = [")
        For Each v As INumVar In vars
            output.Write(" {0,7:0.000}", x(v))
        Next v
        output.WriteLine(" ]")
        output.Write("dslack = [")
        For Each v As INumVar In vars
            output.Write(" {0,7:0.000}", dslack(v))
        Next v
        output.WriteLine(" ]")
        output.Write("pi     = [")
        For Each r As IRange In rngs
            output.Write(" {0,7:0.000}", pi(r))
        Next r
        output.WriteLine(" ]")
        output.Write("slack  = [")
        For Each r As IRange In rngs
            output.Write(" {0,7:0.000}", slack(r))
        Next r
        output.WriteLine(" ]")

        ' Test primal feasibility.
        ' This example illustrates the use of dual vectors returned by CPLEX
        ' to verify dual feasibility, so we do not test primal feasibility
        ' here.

        ' Test dual feasibility.
        ' We must have
        ' - for all <= constraints the respective pi value is non-negative,
        ' - for all >= constraints the respective pi value is non-positive,
        ' - the dslack value for all non-cone variables must be non-negative.
        ' Note that we do not support ranged constraints here.
        For Each v As INumVar In vars
            If cone(v) Is NOT_IN_CONE AndAlso dslack(v) < -tol Then
                err.WriteLine("Dual multiplier for " & v.Name & " is not feasible: " & dslack(v))
                Return False
            End If
        Next v
        For Each r As IRange In rngs
            If System.Math.Abs(r.LB - r.UB) <= tol Then
                ' Nothing to check for equality constraints.
            ElseIf r.LB > -System.[Double].MaxValue AndAlso pi(r) > tol Then
                err.WriteLine("Dual multiplier " & pi(r) & " for >= constraint")
                err.WriteLine(" " & r.Name)
                err.WriteLine("not feasible.")
                Return False
            ElseIf r.UB < System.[Double].MaxValue AndAlso pi(r) < -tol Then
                err.WriteLine("Dual multiplier " & pi(r) & " for <= constraint")
                err.WriteLine(" " & r.Name)
                err.WriteLine("not feasible.")
                Return False
            End If
        Next r

        ' Test complementary slackness.
        ' For each constraint either the constraint must have zero slack or
        ' the dual multiplier for the constraint must be 0. We must also
        ' consider the special case in which a variable is not explicitly
        '  contained in a second order cone constraint.
        For Each v As INumVar In vars
            If cone(v) Is NOT_IN_CONE Then
                If System.Math.Abs(x(v)) > tol AndAlso dslack(v) > tol Then
                    err.WriteLine("Invalid complementary slackness for " & v.Name & ":")
                    err.WriteLine(" " & x(v) & " and " & dslack(v))
                    Return False
                End If
            End If
        Next v
        For Each r As IRange In rngs
            If System.Math.Abs(slack(r)) > tol AndAlso System.Math.Abs(pi(r)) > tol Then
                err.WriteLine("Invalid complementary slackness for")
                err.WriteLine(" " & r.Name)
                err.WriteLine(" " & slack(r) & " and " & pi(r))
                Return False
            End If
        Next r

        ' Test stationarity.
        ' We must have
        '  c - g[i]'(X)*pi[i] = 0
        ' where c is the objective function, g[i] is the i-th constraint of the
        ' problem, g[i]'(x) is the derivate of g[i] with respect to x and X is the
        ' optimal solution.
        ' We need to distinguish the following cases:
        ' - linear constraints g(x) = ax - b. The derivative of such a
        '   constraint is g'(x) = a.
        ' - second order constraints g(x[1],...,x[n]) = -x[1] + |(x[2],...,x[n])|
        '   the derivative of such a constraint is
        '     g'(x) = (-1, x[2]/|(x[2],...,x[n])|, ..., x[n]/|(x[2],...,x[n])|
        '   (here |.| denotes the Euclidean norm).
        ' - bound constraints g(x) = -x for variables that are not explicitly
        '   contained in any second order cone constraint. The derivative for
        '   such a constraint is g'(x) = -1.
        ' Note that it may happen that the derivative of a second order cone
        ' constraint is not defined at the optimal solution X (this happens if
        ' X=0). In this case we just skip the stationarity test.
        Dim sum As IDictionary(Of INumVar, System.Double) = New SortedDictionary(Of INumVar, System.Double)(NumVarComparator)
        For Each v As INumVar In vars
            sum(v) = 0.0
        Next v
        Dim it As ILinearNumExprEnumerator = DirectCast(cplex.GetObjective().Expr, ILinearNumExpr).GetLinearEnumerator()
        While it.MoveNext
            sum(it.NumVar) = it.Value
        End While

        For Each v As INumVar In vars
            If cone(v) Is NOT_IN_CONE Then
                sum(v) = sum(v) - dslack(v)
            End If
        Next v

        For Each r As IRange In rngs
            Dim e As INumExpr = r.Expr
            If (TypeOf e Is IQuadNumExpr) AndAlso DirectCast(e, IQuadNumExpr).GetQuadEnumerator().MoveNext() Then
                Dim norm As Double = 0.0
                Dim q As IQuadNumExprEnumerator = DirectCast(e, IQuadNumExpr).GetQuadEnumerator()
                While q.MoveNext
                    If q.Value > 0 Then
                        norm += x(q.NumVar1) * x(q.NumVar1)
                    End If
                End While
                norm = System.Math.Sqrt(norm)
                If System.Math.Abs(norm) <= tol Then
                    cplex.Warning().WriteLine("Cannot check stationarity at non-differentiable point")
                    Return True
                End If
                q = DirectCast(e, IQuadNumExpr).GetQuadEnumerator()
                While q.MoveNext
                    Dim v As INumVar = q.NumVar1
                    If q.Value < 0 Then
                        sum(v) = sum(v) - pi(r)
                    ElseIf q.Value > 0 Then
                        sum(v) = sum(v) + pi(r) * x(v) / norm
                    End If
                End While
            ElseIf TypeOf e Is ILinearNumExpr Then
                it = DirectCast(e, ILinearNumExpr).GetLinearEnumerator()
                While it.MoveNext
                    Dim v As INumVar = it.NumVar
                    sum(v) = sum(v) - pi(r) * it.Value
                End While
            End If
        Next r

        ' Now test that all elements in sum[] are 0.
        For Each v As INumVar In vars
            If System.Math.Abs(sum(v)) > tol Then
                err.WriteLine("Invalid stationarity " & sum(v) & " for " & v.Name)
                Return False
            End If
        Next v

        Return True
    End Function 'Checkkkt

    ' This function creates the following model:
    '   Minimize
    '    obj: x1 + x2 + x3 + x4 + x5 + x6
    '   Subject To
    '    c1: x1 + x2      + x5      = 8
    '    c2:           x3 + x5 + x6 = 10
    '    q1: [ -x1^2 + x2^2 + x3^2 ] <= 0
    '    q2: [ -x4^2 + x5^2 ] <= 0
    '   Bounds
    '    x2 Free
    '    x3 Free
    '    x5 Free
    '   End
    ' which is a second order cone program in standard form.
    Private Shared Function Createmodel(ByRef cplex As Cplex, ByRef vars As ICollection(Of INumVar), ByRef rngs As ICollection(Of IRange), ByRef cone As IDictionary(Of INumVar, IRange)) As IObjective
        Dim pinf As System.Double = System.[Double].PositiveInfinity
        Dim ninf As System.Double = System.[Double].NegativeInfinity
        Dim x1 As INumVar = cplex.NumVar(0, pinf, "x1")
        Dim x2 As INumVar = cplex.NumVar(ninf, pinf, "x2")
        Dim x3 As INumVar = cplex.NumVar(ninf, pinf, "x3")
        Dim x4 As INumVar = cplex.NumVar(0, pinf, "x4")
        Dim x5 As INumVar = cplex.NumVar(ninf, pinf, "x5")
        Dim x6 As INumVar = cplex.NumVar(0, pinf, "x6")

        Dim obj As IObjective = cplex.AddMinimize(cplex.Sum(cplex.Sum(cplex.Sum(x1, x2), cplex.Sum(x3, x4)), cplex.Sum(x5, x6)), "obj")

        Dim c1 As IRange = cplex.AddEq(cplex.Sum(x1, cplex.Sum(x2, x5)), 8, "c1")
        Dim c2 As IRange = cplex.AddEq(cplex.Sum(x3, cplex.Sum(x5, x6)), 10, "c2")

        Dim q1 As IRange = cplex.AddLe(cplex.Sum(cplex.Prod(-1, cplex.Prod(x1, x1)), cplex.Sum(cplex.Prod(x2, x2), cplex.Prod(x3, x3))), 0, "q1")
        cone(x1) = q1
        cone(x2) = NOT_CONE_HEAD
        cone(x3) = NOT_CONE_HEAD
        Dim q2 As IRange = cplex.AddLe(cplex.Sum(cplex.Prod(-1, cplex.Prod(x4, x4)), cplex.Prod(x5, x5)), 0, "q2")
        cone(x4) = q2
        cone(x5) = NOT_CONE_HEAD

        cone(x6) = NOT_IN_CONE

        vars.Add(x1)
        vars.Add(x2)
        vars.Add(x3)
        vars.Add(x4)
        vars.Add(x5)
        vars.Add(x6)

        rngs.Add(c1)
        rngs.Add(c2)
        rngs.Add(q1)
        rngs.Add(q2)

        Return obj
    End Function 'Createmodel

    Public Overloads Shared Sub Main(ByVal args() As String)
        Dim cone As IDictionary(Of INumVar, IRange) = New SortedDictionary(Of INumVar, IRange)(NumVarComparator)
        Dim retval As Integer = -1
        Dim cplex As Cplex = Nothing
        Try
            cplex = New Cplex()

            ' Initialize the two special (empty) marker ranges.
            NOT_CONE_HEAD = cplex.Range(0, 0)
            NOT_IN_CONE = cplex.Range(0, 0)

            ' Create the model.
            Dim vars As ICollection(Of INumVar) = New List(Of INumVar)()
            Dim rngs As ICollection(Of IRange) = New List(Of IRange)()
            Dim obj As IObjective = Createmodel(cplex, vars, rngs, cone)

            ' Apply parameter settings.
            cplex.SetParam(Cplex.Param.Barrier.QCPConvergeTol, CONVTOL)

            ' Solve the problem. If we cannot find an _optimal_ solution then
            ' there is no point in checking the KKT conditions and we throw an
            ' exception.
            If Not cplex.Solve() OrElse cplex.GetStatus() IsNot cplex.Status.Optimal Then
                Throw New System.SystemException("Failed to solve problem to optimality")
            End If

            ' Test the KKT conditions on the solution.
            If Not Checkkkt(cplex, obj, vars, rngs, cone, TESTTOL) Then
                cplex.Output().WriteLine("Testing of KKT conditions failed.")
            Else
                cplex.Output().WriteLine("KKT conditions are satisfied.")
                retval = 0
            End If
        Catch e As ILOG.Concert.Exception
            If cplex IsNot Nothing Then
                cplex.Output().WriteLine("IloException: " & e.Message)
                cplex.Output().WriteLine(e.StackTrace)
            Else
                System.Console.WriteLine("IloException: " & e.Message)
                System.Console.WriteLine(e.StackTrace)
            End If
            retval = -1
        Finally
            If cplex IsNot Nothing Then
                cplex.[End]()
            End If
        End Try
        System.Environment.[Exit](retval)
    End Sub 'Main
End Class 'SocpEx1

