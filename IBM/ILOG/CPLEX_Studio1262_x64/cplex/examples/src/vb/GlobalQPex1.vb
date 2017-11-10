' ---------------------------------------------------------------------------
' File: GlobalQPex1.vb
' Version 12.6.2  
' ---------------------------------------------------------------------------
' Licensed Materials - Property of IBM
' 5725-A06 5725-A29 5724-Y48 5724-Y49 5724-Y54 5724-Y55 5655-Y21
' Copyright IBM Corporation 2001, 2015. All Rights Reserved.
' 
' US Government Users Restricted Rights - Use, duplication or
' disclosure restricted by GSA ADP Schedule Contract with
' IBM Corp.
' ---------------------------------------------------------------------------
'
' GlobalQPex1.vb - Reading in and optimizing a convex or nonconvex (mixed-integer) QP
' with convex, first order or global optimizer.
' QP problem
'
' To run this example, command line arguments are required.
' That is:   GlobalQPex1 filename optimalitytarget
' where 
'     filename is the name of the file, with .mps, .lp, or .sav extension
'     optimalitytarget   is the optimality target
'                 c          for a convex qp or miqp
'                 f          for a first order solution (only qp)
'                 g          for the global optimum
'
' Example:
'     GlobalQPex1  nonconvexqp.lp g
'
Imports ILOG.Concert
Imports ILOG.CPLEX
Imports System.Collections


Public Class GlobalQPex1
   
   Friend Shared Sub Usage()
      System.Console.WriteLine("usage:  GlobalQPex1 <filename> <optimality target>")
      System.Console.WriteLine("          c       convex QP or MIQP")
      System.Console.WriteLine("          f       first order solution (only QP)")
      System.Console.WriteLine("          g       global optimum")
   End Sub 'Usage
   
   Public Overloads Shared Sub Main(ByVal args() As String)
      If args.Length <> 2 Then
         Usage()
         Return
      End If
      Dim ismip as Boolean = False
      Try
         Dim cplex As New Cplex()

         ' Evaluate command line option and set optimization method accordingly.
         Select Case args(1).ToCharArray()(0)
            Case "c"c
               cplex.SetParam(Cplex.Param.OptimalityTarget, cplex.OptimalityTarget.OptimalConvex)
            Case "f"c
               cplex.SetParam(Cplex.Param.OptimalityTarget, cplex.OptimalityTarget.FirstOrder)
            Case "g"c
               cplex.SetParam(Cplex.Param.OptimalityTarget, cplex.OptimalityTarget.OptimalGlobal)
            Case Else
               Usage()
               Return
         End Select

         cplex.ImportModel(args(0))

         ismip = cplex.IsMIP

         If cplex.Solve() Then
            System.Console.WriteLine(("Solution status = " + cplex.GetStatus().ToString))
            System.Console.WriteLine(("Solution value  = " & cplex.ObjValue))

            Dim matrixEnum As IEnumerator = cplex.GetLPMatrixEnumerator()
            matrixEnum.MoveNext()

            Dim lp As ILPMatrix = CType(matrixEnum.Current, ILPMatrix)

            Dim x As Double() = cplex.GetValues(lp)
            Dim j As Integer

            For j = 0 To x.Length - 1
               System.Console.WriteLine(("Variable " & j & ": Value = " & x(j)))
            Next j
         End If
         cplex.End()
      Catch e As ILOG.CPLEX.CplexModeler.Exception 
         If args(1).ToCharArray()(0) = "c" And e.getStatus() = 5002 Then
'            Status 5002 is CPXERR_Q_NOT_POS_DEF
           If ismip Then
              System.Console.WriteLine("Problem is not convex. Use argument g to get global optimum.")
           Else 
              System.Console.Write("Problem is not convex. Use argument f to get local optimum ")
              System.Console.WriteLine("or g to get global optimum.")
           End If         
         Else If args(1).ToCharArray()(0) = "f" And e.getStatus() = 1017 And ismip Then
'            Status 1017 is CPXERR_NOT_FOR_MIP
            System.Console.Write("Problem is a MIP, cannot compute local optima satifying ")
            System.Console.WriteLine("the first order KKT.")
            System.Console.WriteLine("Use argument g to get the global optimum.")
         Else 
            System.Console.WriteLine("Cplex exception '" + e.ToString + "' caught")
         End If
      Catch e As ILOG.Concert.Exception
         System.Console.WriteLine(("Concert exception caught: " + e.ToString))
      End Try
   End Sub 'Main
End Class 'GlobalQPex1.vb
