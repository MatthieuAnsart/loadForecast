@echo off
rem Run the example

rem If CPLEX_STUDIO_DIR1262 is not already defined, add the following line :  
rem CPLEX_STUDIO_DIR1262=%~dp0\..\..\..\..\..

cd %~dp0

set "arg="
:Loop
IF "%1"=="" GOTO Continue
    set "arg=%arg% %1"
SHIFT
GOTO Loop
:Continue

call "%CPLEX_STUDIO_DIR1262%\opl\ant\bin\ant" -Dexample.arg.line="%arg%" run

pause
