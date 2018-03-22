@echo off
SET project=%1
IF "%project%" == "" GOTO error

SET currentDir=%~dp0

PUSHD %project%
rem CALL nuget pack -Build -Symbols -Properties Configuration=Release -OutputDirectory %currentDir% -includereferencedprojects 
rem for some reason -Build is incompatible with our .ps1 post-build step
CALL nuget pack -Symbols -Properties Configuration=Release;releaseNotes="https://creuna.visualstudio.com/DefaultCollection/Creuna.Basis/_git/Creuna.Diagnostics/?_a=readme" -OutputDirectory %currentDir% -includereferencedprojects 
POPD

GOTO fin

:error
ECHO Usage: 
ECHO 	build-package {MyProjectName} 
ECHO for example:
ECHO 	%~n0 Operations
:fin