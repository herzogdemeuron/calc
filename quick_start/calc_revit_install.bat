@echo off
setlocal enabledelayedexpansion

:: Set the base target directory
set "BASE_TARGET_DIR=C:\ProgramData\Autodesk\Revit\Addins"

:: Prompt user to choose Revit version
echo Choose Revit version:
echo 2022
echo 2023
echo 2024
set /p "choice=Enter your choice: "

:: Set the chosen year based on user input
if "%choice%"=="2022" (
    set "YEAR=2022"
) else if "%choice%"=="2023" (
    set "YEAR=2023"
) else if "%choice%"=="2024" (
    set "YEAR=2024"
) else (
    echo Invalid choice. Exiting...
	pause
    exit /b 1
)

:: Set source and target directories
set "BIN_SOURCE=..\bin"
set "ADDIN_SOURCE=.\revit\RevitCalc%YEAR%.addin"
set "TARGET_DIR=%BASE_TARGET_DIR%\%YEAR%"
set "CALCREVIT_DIR=%TARGET_DIR%\CalcRevit"

:: Check if source directories and files exist
if not exist "%BIN_SOURCE%" (
    echo Error: bin folder not found.
    exit /b 1
)

if not exist "%ADDIN_SOURCE%" (
    echo Error: RevitCalc%YEAR%.addin file not found.
    exit /b 1
)

:: Create target directories if they don't exist
if not exist "%TARGET_DIR%" mkdir "%TARGET_DIR%"
if not exist "%CALCREVIT_DIR%" mkdir "%CALCREVIT_DIR%"

:: Copy bin folder to CalcRevit directory
echo Copying bin folder to CalcRevit directory...
if exist "%CALCREVIT_DIR%" (
    echo Overwriting existing files in CalcRevit directory...
)
xcopy "%BIN_SOURCE%" "%CALCREVIT_DIR%" /E /I /Y /Q

:: Copy addin file to target directory
echo Copying RevitCalc%YEAR%.addin file...
if exist "%TARGET_DIR%\RevitCalc%YEAR%.addin" (
    echo Overwriting existing RevitCalc%YEAR%.addin file...
)
copy /Y "%ADDIN_SOURCE%" "%TARGET_DIR%" >nul

echo Calc installed for Revit %YEAR% successfully.
pause