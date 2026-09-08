@echo off
title Train Working Management System (TMS) - 1-Click Setup & Launch
color 0B

echo ===============================================================================
echo        TRAIN WORKING MANAGEMENT SYSTEM (TMS) - 1-CLICK LAUNCHER
echo ===============================================================================
echo.

echo [1/3] Restoring Database with all saved authority records...
powershell -ExecutionPolicy Bypass -File "%~dp0Database\Restore-Database.ps1"
if %errorlevel% neq 0 (
    echo [WARNING] Database restore script exited with warnings. Proceeding with application launch...
)

echo.
echo [2/3] Checking / Compiling Application...
set MSBUILD_EXE=
for /f "usebackq tokens=*" %%i in (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe 2^>nul`) do set MSBUILD_EXE=%%i

if defined MSBUILD_EXE (
    echo Compiling solution using Visual Studio MSBuild...
    "%MSBUILD_EXE%" "%~dp0TrainWorkingApp.sln" /t:Build /p:Configuration=Debug /v:m
) else (
    echo Visual Studio MSBuild not detected in standard path.
    echo Checking for pre-compiled executable...
)

echo.
echo [3/3] Launching Application...
if exist "%~dp0TrainWorkingApp\bin\Debug\TrainWorkingApp.exe" (
    echo Starting Train Working Management System...
    start "" "%~dp0TrainWorkingApp\bin\Debug\TrainWorkingApp.exe"
    echo.
    echo [SUCCESS] Application launched successfully!
) else (
    echo [ERROR] Compiled executable not found at TrainWorkingApp\bin\Debug\TrainWorkingApp.exe.
    echo Please open TrainWorkingApp.sln in Visual Studio and build the project.
    pause
)
