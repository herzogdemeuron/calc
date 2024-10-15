@echo off
setlocal enabledelayedexpansion

:: Prompt user to confirm if Revit is not running
echo WARNING: Please ensure that Revit is not running before proceeding.
set /p "confirm=Is Revit closed? (Y/N): "

if /i "%confirm%" NEQ "Y" (
    echo Please close Revit and try again. Exiting...
    pause
    exit /b 1
)

:: Set the base target directory
set "BASE_TARGET_DIR=C:\ProgramData\Autodesk\Revit\Addins"

:: Prompt user to choose Revit version
echo Choose Revit version to uninstall Calc from:
echo 2023
echo 2024
set /p "choice=Enter your choice: "

:: Set the chosen year based on user input
if "%choice%"=="2023" (
    set "YEAR=2023"
) else if "%choice%"=="2024" (
    set "YEAR=2024"
) else (
    echo Invalid choice. Exiting...
    pause
    exit /b 1
)

:: Set target directories
set "TARGET_DIR=%BASE_TARGET_DIR%\%YEAR%"
set "CALCREVIT_DIR=%TARGET_DIR%\CalcRevit"
set "ADDIN_FILE=%TARGET_DIR%\RevitCalc%YEAR%.addin"

:: Check if CalcRevit folder exists
if not exist "%CALCREVIT_DIR%" (
    echo CalcRevit folder not found for Revit %YEAR%.
) else (
    echo Deleting CalcRevit folder for Revit %YEAR%...
    rd /s /q "%CALCREVIT_DIR%"
    echo CalcRevit folder deleted.
)

:: Check if addin file exists
if not exist "%ADDIN_FILE%" (
    echo Addin file RevitCalc%YEAR%.addin not found.
) else (
    echo Deleting addin file RevitCalc%YEAR%.addin...
    del /f /q "%ADDIN_FILE%"
    echo Addin file deleted.
)

echo Uninstallation completed for Calc Revit %YEAR%.
pause
