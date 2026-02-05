@echo off
echo ========================================
echo   Startup Master - Launcher
echo ========================================
echo.
echo Checking for administrator privileges...
net session >nul 2>&1
if %errorlevel% == 0 (
    echo [OK] Running with administrator privileges
    echo.
    echo Launching Startup Master...
    start "" "%~dp0StartupMaster.exe"
) else (
    echo [!] Administrator privileges required
    echo.
    echo Requesting elevation...
    powershell -Command "Start-Process '%~dp0StartupMaster.exe' -Verb RunAs"
)
