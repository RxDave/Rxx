@ECHO OFF
::
:: This script rebuilds the entire Rxx solution in Release mode, generates reference doc files (.chm) and creates 
:: deployment packages for CodePlex and NuGet in the solution's Deployment folder.  (Manual deployment required.)
::
:: See the "Deployment Procedure.txt" file in the solution's Deployment folder for details.
::
:: Running this script is effectively the same as building the entire solution in Visual Studio with the build 
:: configuration set to Release, although it also provides options to execute unit tests and whether to globally 
:: enable or disable code analysis and reference documentation (.chm) output.

:: A log file is generated in the solution's Build folder, next to this batch file.
SET LogFile=%cd%\RxxBuild.log

:: MSBuild log verbosity: q[uiet], m[inimal], n[ormal], d[etailed] and diag[nostic]
SET LogVerbosity=normal

:: Release mode is required to generate deployment packages and documentation.
SET Configuration=Release

:: Must run 32 bit MSBuild because an in-line task depends on NuGet.exe, which apparently only targets x86.
SET MSBuild=%ProgramFiles(x86)%\MSBuild\12.0\Bin\msbuild.exe

ECHO.
ECHO Preparing to build the Rxx solution.
ECHO.
ECHO Current Directory: %cd%
ECHO.

SET /P BuildDocumentationYN=Do you want to build .chm help files (Y/N)? 
SET /P StaticAnalysisYN=Do you want to run complete static analysis (Y/N)? 
SET /P UnitTestsYN=Do you want to run all unit tests? (Y/N)? 
SET /P AddProps=Optional MSBuild properties (Name=Value;)^> 

IF /I "%BuildDocumentationYN%" == "Y" (
	SET BuildDocumentation=True
) ELSE (
	SET BuildDocumentation=False
)

IF /I "%StaticAnalysisYN%" == "Y" (
	SET StaticAnalysis=True
) ELSE (
	SET StaticAnalysis=False
)

IF /I "%UnitTestsYN%" == "Y" (
	SET UnitTests=True
) ELSE (
	SET UnitTests=False
)

SET Properties=Platform="Any CPU"
SET Properties=%Properties%;Configuration=%Configuration%
SET Properties=%Properties%;BuildDocumentation=%BuildDocumentation%
SET Properties=%Properties%;StaticAnalysisEnabled=%StaticAnalysis%
SET Properties=%Properties%;UnitTestsEnabled=%UnitTests%

IF "%AddProps%" NEQ "" (
	SET Properties=%Properties%;%AddProps%
)

ECHO.
ECHO.
ECHO Ready to build Rxx solution.
ECHO.

SETLOCAL ENABLEDELAYEDEXPANSION

SET NewLine=^& ECHO.
SET PropertyList=%Properties:;=!NewLine!%

ECHO %PropertyList%
ECHO.
ECHO Log File: %LogFile%
ECHO Log Verbosity: %LogVerbosity%
ECHO.

PAUSE

"%MSBuild%" "..\Rxx.sln" /fl /flp:verbosity=%LogVerbosity%;LogFile="%LogFile%" /p:%Properties% /t:Rebuild

SET Output=%cd%\..\Deployment\%Configuration%

IF EXIST "%Output%" ( explorer "%Output%" )

PAUSE