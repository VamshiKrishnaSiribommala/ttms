@echo off
title Train Management System (TMS) - 1-Click Setup & Launch
color 0B

echo ===============================================================================
echo            TRAIN MANAGEMENT SYSTEM (TMS) - 1-CLICK LAUNCHER
echo ===============================================================================
echo.

echo [1/3] Restoring Database with all saved railway records...
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
    "%MSBUILD_EXE%" "%~dp0TMSfinal1.sln" /t:Build /p:Configuration=Debug /v:m
) else (
    echo Visual Studio MSBuild not detected in standard path.
    echo Checking for pre-compiled executable...
)

echo.
echo [3/3] Launching Application...
if exist "%~dp0TMSfinal1\bin\Debug\TMSfinal1.exe" (
    echo Starting Train Management System...
    start "" "%~dp0TMSfinal1\bin\Debug\TMSfinal1.exe"
    echo.
    echo [SUCCESS] Application launched successfully!
) else (
    echo [ERROR] Compiled executable not found at TMSfinal1\bin\Debug\TMSfinal1.exe.
    echo Please open TMSfinal1.sln in Visual Studio and build the project.
    pause
)
