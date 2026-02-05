@echo off
echo ========================================
echo   Startup Master - Ultimate Launcher
echo ========================================
echo.
echo Checking for administrator privileges...
net session >nul 2>&1
if %errorlevel% == 0 (
    echo [OK] Running with administrator privileges
    echo.
    echo Launching Startup Master...
    start "" "%~dp0bin\Release\net8.0-windows\win-x64\publish\StartupMaster.exe"
) else (
    echo [!] Administrator privileges required
    echo.
    echo Requesting elevation...
    powershell -Command "Start-Process '%~dp0bin\Release\net8.0-windows\win-x64\publish\StartupMaster.exe' -Verb RunAs"
)
