Attribute VB_Name = "cplexvba"
' --------------------------------------------------------------------------
' File: cplexvba.bas
' Version 12.6.2
' --------------------------------------------------------------------------
' Licensed Materials - Property of IBM
' 5725-A06 5725-A29 5724-Y48 5724-Y49 5724-Y54 5724-Y55 5655-Y21
' Copyright IBM Corporation 2008, 2015. All Rights Reserved.
'
' US Government Users Restricted Rights - Use, duplication or
' disclosure restricted by GSA ADP Schedule Contract with
' IBM Corp.
' --------------------------------------------------------------------------

Option Explicit


' Clear the model from the active sheet.
' All information relating to the CPLEX model is removed from the active sheet.
Sub CPXclear()
    Application.Run ("CPLEX.CLEAR")
End Sub


' Add a variable to the model on the active sheet.
' All arguments but Range are optional and leaving them out is the same as leaving them
' empty in the GUI.
' Upon success the function returns the 1-based index of the new variable in the model's
' variable list. This index can be used as input for CPXupdateVariable or CPXremoveVariable.
' Parameters
' Range    -- The cells that are considered to be variables. This must not be an empty range.
' Lb       -- The lower bound(s) for variables. Leaving this out means "no lower bound".
' Ub       -- The upper bound(s) for variables. Leaving this out means "no upper bound".
' Integral -- True if the variables in Range are restricted to integer values. The default
'             is false.
' Binary   -- True if the variables in Range are restricted to values 0 and 1. The default
'             is false.
' Returns
'    The 1-based index of the newly created variable(s) on success or 0 if the
'    variable(s) could not be added.
Function CPXaddVariable(Variable As Variant, Optional Lb As Variant, Optional Ub As Variant, Optional Integral As Boolean, Optional Binary As Boolean) As Long
    CPXaddVariable = Application.Run("CPLEX.ADDVARIABLE", Variable, Lb, Ub, Integral, Binary)
End Function


' Update variable(s) at 1-based index Idx in the model's variable list.
' All arguments but Range are optional and leaving them out is the same as leaving them
' empty in the GUI.
' Note: The function will change ALL fields of the variable definition -- even those
'       for which you leave out parameters.
' Parameters
' Idx      -- The 1-based index of the variable(s) to be changed (as returned by CPXaddVariable).
' Range    -- The cells that are considered to be variables. This must not be an empty range.
' Lb       -- The lower bound(s) for variables. Leaving this out means "no lower bound".
' Ub       -- The upper bound(s) for variables. Leaving this out means "no upper bound".
' Integral -- True if the variables in Range are restricted to integer values. The default
'             is false.
' Binary   -- True if the variables in Range are restricted to values 0 and 1. The default
'             is false.
' Returns
'     True on success and false otherwise.
Function CPXupdateVariable(Idx As Variant, Variable As Variant, Optional Lb As Variant, Optional Ub As Variant, Optional Integral As Boolean, Optional Binary As Boolean) As Boolean
    CPXupdateVariable = Application.Run("CPLEX.UPDATEVARIABLE", Idx, Variable, Lb, Ub, Integral, Binary)
End Function


' Remove variable(s) at 1-based index Idx from the model's variable list.
' Parameters
'     Idx  -- The 1-based index of the variable(s) to be removed (as returned by CPXaddVariable).
'             If the argument is omitted all variables are removed from the model.
' Returns
'     True on success and false otherwise.
Function CPXremoveVariable(Optional Idx As Variant) As Boolean
    CPXremoveVariable = Application.Run("CPLEX.REMOVEVARIABLE", Idx)
End Function


' Add a constraint to the model on the active sheet.
' All arguments but Range are optional and leaving them out is the same as leaving them
' empty in the GUI.
' Upon success the function returns the 1-based index of the new constraint in the model's
' constraint list. This index can be used as input for CPXupdateConstraint or
' CPXremoveConstraint.
' Parameters
' Range    -- The cells that are to be constrained. This must not be an empty range.
' Lb       -- The lower bound(s) for the constrained cells. Leaving this out means
'             "no lower bound".
' Ub       -- The upper bound(s) for the constrained cells. Leaving this out means
'             "no upper bound".
' Returns
'    The 1-based index of the newly created constraint on success or 0 if the
'    constraint could not be added.
Function CPXaddConstraint(Constraint As Variant, Optional Lb As Variant, Optional Ub As Variant) As Long
    CPXaddConstraint = Application.Run("CPLEX.ADDCONSTRAINT", Constraint, Lb, Ub)
End Function


' Update constraint at 1-based index Idx in the model's constraint list.
' All arguments but Range are optional and leaving them out is the same as leaving them
' empty in the GUI.
' Note: The function will change ALL fields of the constraint definition -- even those
'       for which you leave out parameters.
' Parameters
' Idx      -- The 1-based index of the constraint to be changed (as returned by CPXaddConstraint).
' Range    -- The cells that are to be constrained. This must not be an empty range.
' Lb       -- The lower bound(s) for the constrained cells. Leaving this out means
'             "no lower bound".
' Ub       -- The upper bound(s) for the constrained cells. Leaving this out means
'             "no upper bound".
' Returns
'     True on success and false otherwise.
Function CPXupdateConstraint(Idx As Variant, Constraint As Variant, Optional Lb As Variant, Optional Ub As Variant) As Boolean
    CPXupdateConstraint = Application.Run("CPLEX.UPDATECONSTRAINT", Idx, Constraint, Lb, Ub)
End Function


' Remove constraint at 1-based index Idx from the model's variable list.
' Parameters
'     Idx  -- The 1-based index of the constraint to be removed (as returned by CPXaddConstraint).
'             If the argument is omitted all constraints are removed from the model.
' Returns
'     True on success and false otherwise.
Function CPXremoveConstraint(Optional Idx As Variant) As Boolean
    CPXremoveConstraint = Application.Run("CPLEX.REMOVECONSTRAINT", Idx)
End Function


' Set the CPLEX Excel-specific parameters.
' Parameters left out in the argument list will not be changed.
' Parameters
' StopInt    -- Flag to control whether to stop at each integral solution found.
' LinOrQuad  -- Flag to control whether to display a warning message if CPLEX
'               found an unknown function and replaced that with its presumed linear
'               or quadratic equivalent.
' ExportModel -- Flag to control whether the optimization model is to be exported
'                for debugging purposes.
' ModelFile   -- Name of file to which model is exported if ExportModel is true.
' Returns
'     True on success and false otherwise.
Function CPXsetSpecial(Optional StopInt As Variant, Optional LinOrQuad As Variant, Optional ExportModel As Variant, Optional ModelFile As Variant) As Boolean
    CPXsetSpecial = Application.Run("CPLEX.SETSPECIAL", StopInt, LinOrQuad, ExportModel, ModelFile)
End Function


' Add a parameter to the parameter list on the active spreadsheet's model.
' Note: The function does not check for duplicate parameters in the parameter list
'       of the model.
' The index returned by this function can be used as argument to CPXupdateParameter or
' CPXremoveParameter.
' Parameters
' Number -- The number or name of the parameter to be set.
' Value  -- The value to be set for parameter Number.
' Returns
'     The 1-based index of the newly created parameter in the model's parameter list on
'     success and 0 on failure.
Function CPXaddParameter(Number As Variant, Value As Variant) As Boolean
    CPXaddParameter = Application.Run("CPLEX.ADDPARAMETER", Number, Value)
End Function


' Update parameter at 1-based index Idx.
' Parameters
' Idx    -- The 1-based index of the parameter to be changed (as returned by CPXaddParameter).
' Number -- The number or name of the parameter to be set.
' Value  -- The value to be set for parameter Number.
' Returns
'     True on success and false otherwise.
Function CPXupdateParameter(Idx As Variant, Number As Variant, Value As Variant) As Boolean
    CPXupdateParameter = Application.Run("CPLEX.UPDATEPARAMETER", Idx, Number, Value)
End Function


' Remove parameter at 1-based index Idx from parameter list.
' Idx    -- The 1-based index of the parameter to be removed (as returned by CPXaddParameter).
'           If omitted all parameters are removed.
' Returns
'     True on success and false on failure.
Function CPXremoveParameter(Idx As Variant) As Boolean
    CPXremoveParameter = Application.Run("CPLEX.REMOVEPARAMETER", Idx)
End Function


' Set the objective function for the model on the active sheet.
' Target is only optional if Sense is not 3.
' Parameters
' ObjCell -- The objective function cell. This must be a single cell.
' Sense   -- The sense of the objective function. Allowed values are
'              1 - Maximize ObjCell
'              2 - Minimize ObjCell
'              3 - Find a solution so that ObjCell equals Target
' Target  -- The target value for Sense = 3.
' Returns
Function CPXsetObjective(ObjCell As Variant, Sense As Integer, Optional Target As Double) As Boolean
    CPXsetObjective = Application.Run("CPLEX.SETOBJECTIVE", ObjCell, Sense, Target)
End Function


' Invokes CPLEX on the model defined on the active sheet.
' Note: If this function returns true that does not mean that a solution was found.
'       CPLEX may also have successfully proved infeasibility of the model.
' NoDialog -- If a true value is passed the final dialog will not be shown. In this case
'             there is no way to reset the variable cells to their original values
'             or perform a solution or sensitivity analysis.
'             If the value passed cannot be converted to a boolean it is ignored.
' Returns
'     True on success and false on failure.
Function CPXsolve(Optional NoDialog As Variant) As Boolean
    CPXsolve = Application.Run("CPLEX.SOLVE", NoDialog)
End Function


' Displays the CPLEX Excel connector's solve dialog.
Sub CPXdialog()
    Application.Run ("CPLEX.DIALOG")
End Sub


