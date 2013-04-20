@echo off
pushd

:: CD to script's directory
cd /D %~dp0

if '%1'=='' ( 
	Set Mode=Debug
) else (
	Set Mode=%1
)

set doPause=1
if not "%2" == "" set doPause=0
%systemroot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "Catpic.Demo.sln" /t:Build /p:Configuration=%Mode%;TargetFrameworkVersion=v4.0
@if ERRORLEVEL 1 goto fail

:fail
if "%doPause%" == "1" pause
popd
exit /b 1

:end
popd
if "%doPause%" == "1" pause
exit /b 0