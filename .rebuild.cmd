@echo off

PowerShell.exe -ExecutionPolicy ByPass -Command "& { %~dp0build.ps1 -Script %~dp0build.cake -Target %1 -ScriptArgs '--BuildConfig=%2'; exit $LASTEXITCODE }" || goto err

echo Rebuild succeeded for %1 %2
if "%3"=="PauseOnExit" pause
exit /b 0

:err
echo Rebuild failed for %1 %2
if "%3"=="PauseOnExit" pause
exit /b 1