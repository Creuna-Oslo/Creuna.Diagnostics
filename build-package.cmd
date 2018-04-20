@echo off
SET project=%1
IF "%project%" == "" GOTO error

SET currentDir=%~dp0

PUSHD %project%

SET projectUrl=https://github.com/Creuna-Oslo/Creuna.Diagnostics
SET master=%projectUrl%/blob/master
SET license=%master%/LICENSE
SET icon=https://raw.githubusercontent.com/Creuna-Oslo/Creuna.Diagnostics/master/creuna.gif

rem CALL nuget pack -Build -Symbols -Properties Configuration=Release -OutputDirectory %currentDir% -includereferencedprojects 
rem for some reason -Build is incompatible with our .ps1 post-build step
CALL nuget pack -Symbols -Properties Configuration=Release;license="%license%";projectUrl="%projectUrl%";icon="%icon%" -OutputDirectory %currentDir% -includereferencedprojects 
POPD

GOTO fin

:error
ECHO Usage: 
ECHO 	build-package {MyProjectName} 
ECHO for example:
ECHO 	%~n0 Operations
:fin