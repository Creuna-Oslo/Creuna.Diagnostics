@echo off
rem todo: setup appveyor

SET currentDir=%~dp0

echo Cleanup *.nupkg
del *.nupkg

SET project=Creuna.ApplicationInsights.TelemetryFiltering
call build-package %project%

SET project=EPiLog
call build-package %project%

SET project=Creuna.Diagnostics
call build-package %project%

SET project=Creuna.Diagnostics.Web
call build-package %project%

SET project=Creuna.Diagnostics.Web.Episerver
call build-package %project%