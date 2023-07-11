@echo off
set "sourceFolder=C:\HdM-DT\calc\ConnectorRevit\ConnectorRevit2023\bin\Release"
set "destinationFolder=C:\ProgramData\Autodesk\Revit\Addins\2023\CalcRevit\Release"

rem Clear the destination folder
rmdir /S /Q "%destinationFolder%"
mkdir "%destinationFolder%"

rem Copy the contents from the source folder to the destination folder
xcopy /E /I "%sourceFolder%\*" "%destinationFolder%\"