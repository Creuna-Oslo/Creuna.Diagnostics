@echo off
SET project=%1
IF "%project%" == "" GOTO error

SET currentDir=%~dp0

PUSHD %project%
CALL nuget pack -Build -Symbols -Properties Configuration=Release -OutputDirectory %currentDir% -includereferencedprojects 
POPD

GOTO fin

:error
ECHO Usage: 
ECHO 	build-package {MyProjectName} 
ECHO for example:
ECHO 	%~n0 Operations
:fin