# Create Distribution Package for Startup Master
# Creates a redistributable ZIP with all necessary files

param(
    [switch]$SelfContained = $false,
    [string]$OutputPath = ".\dist"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Startup Master - Package Builder" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Ensure output directory exists
if (-not (Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
}

# Build configuration
$buildConfig = if ($SelfContained) { "self-contained" } else { "framework-dependent" }
Write-Host "Build Configuration: $buildConfig" -ForegroundColor Yellow
Write-Host ""

# Build the project
Write-Host "Building Startup Master..." -ForegroundColor Cyan
if ($SelfContained) {
    dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true
} else {
    dotnet publish -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true
}

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "Build successful!" -ForegroundColor Green
Write-Host ""

# Prepare distribution folder
Write-Host "Preparing distribution package..." -ForegroundColor Cyan

$publishPath = "bin\Release\net8.0-windows\win-x64\publish"
$distFolder = Join-Path $OutputPath "StartupMaster_v1.0"

if (Test-Path $distFolder) {
    Remove-Item $distFolder -Recurse -Force
}

New-Item -ItemType Directory -Path $distFolder -Force | Out-Null

# Copy executable
Write-Host "  Copying executable..." -ForegroundColor Gray
Copy-Item "$publishPath\StartupMaster.exe" $distFolder

# Copy documentation
Write-Host "  Copying documentation..." -ForegroundColor Gray
$docs = @(
    "README.md",
    "QUICKSTART.md",
    "INSTALL.md",
    "FEATURES.md",
    "CHANGELOG.md",
    "INDEX.md"
)

$docsFolder = Join-Path $distFolder "Docs"
New-Item -ItemType Directory -Path $docsFolder -Force | Out-Null

foreach ($doc in $docs) {
    if (Test-Path $doc) {
        Copy-Item $doc $docsFolder
    }
}

# Copy utilities
Write-Host "  Copying utility scripts..." -ForegroundColor Gray
$utils = @(
    "Analyze-Startup.ps1",
    "Optimize-Startup.ps1",
    "Quick-Disable-All.ps1"
)

$utilsFolder = Join-Path $distFolder "Utils"
New-Item -ItemType Directory -Path $utilsFolder -Force | Out-Null

foreach ($util in $utils) {
    if (Test-Path $util) {
        Copy-Item $util $utilsFolder
    }
}

# Create launcher batch file
Write-Host "  Creating launcher..." -ForegroundColor Gray
$launcherContent = @"
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
"@

$launcherContent | Out-File -FilePath (Join-Path $distFolder "Launch.bat") -Encoding ASCII

# Create README for distribution
Write-Host "  Creating distribution README..." -ForegroundColor Gray
$distReadme = @"
# Startup Master v1.0 - Distribution Package

## Quick Start

1. **Run the application:**
   - Double-click ``Launch.bat`` (recommended)
   - OR double-click ``StartupMaster.exe`` directly

2. **Click "Yes"** when prompted for administrator elevation

3. **Start managing** your Windows startup items!

## Requirements

- Windows 10 (1809+) or Windows 11
- Administrator privileges (for system-wide changes)
$(if (-not $SelfContained) { "- .NET 8.0 Desktop Runtime (download from https://dotnet.microsoft.com/download/dotnet/8.0)" })

## Documentation

Full documentation is available in the ``Docs\`` folder:
- ``QUICKSTART.md`` - 60-second getting started guide
- ``README.md`` - Complete overview
- ``INSTALL.md`` - Detailed installation guide
- ``FEATURES.md`` - Complete feature list
- ``INDEX.md`` - Documentation index

## Utility Scripts

Powerful automation scripts in the ``Utils\`` folder:
- ``Analyze-Startup.ps1`` - Analyze startup configuration
- ``Optimize-Startup.ps1`` - Guided optimization
- ``Quick-Disable-All.ps1`` - Emergency disable all startup items

## Support

For help:
1. Check the documentation in ``Docs\``
2. Review the source code (available separately)
3. Run the test suite to diagnose issues

## Version Information

- **Version:** 1.0.0
- **Release Date:** 2026-02-05
- **Build Type:** $buildConfig
- **Platform:** Windows x64

## License

Free for personal and commercial use.
No warranty provided - use at your own risk.

---

**Enjoy faster boot times with Startup Master!** 🚀
"@

$distReadme | Out-File -FilePath (Join-Path $distFolder "README.txt") -Encoding UTF8

# Create ZIP
Write-Host ""
Write-Host "Creating ZIP archive..." -ForegroundColor Cyan

$zipName = if ($SelfContained) {
    "StartupMaster_v1.0_Standalone.zip"
} else {
    "StartupMaster_v1.0.zip"
}

$zipPath = Join-Path $OutputPath $zipName

if (Test-Path $zipPath) {
    Remove-Item $zipPath -Force
}

Compress-Archive -Path "$distFolder\*" -DestinationPath $zipPath -CompressionLevel Optimal

# Calculate sizes
$exeSize = [math]::Round((Get-Item (Join-Path $distFolder "StartupMaster.exe")).Length / 1MB, 2)
$zipSize = [math]::Round((Get-Item $zipPath).Length / 1MB, 2)

# Summary
Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "   Package Created Successfully!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Package Details:" -ForegroundColor White
Write-Host "  Type:         $buildConfig" -ForegroundColor Gray
Write-Host "  Executable:   $exeSize MB" -ForegroundColor Gray
Write-Host "  ZIP Archive:  $zipSize MB" -ForegroundColor Gray
Write-Host ""
Write-Host "Output Files:" -ForegroundColor White
Write-Host "  Folder:       $distFolder" -ForegroundColor Gray
Write-Host "  ZIP:          $zipPath" -ForegroundColor Gray
Write-Host ""
Write-Host "Contents:" -ForegroundColor White
Write-Host "  • StartupMaster.exe" -ForegroundColor Gray
Write-Host "  • Launch.bat (easy launcher)" -ForegroundColor Gray
Write-Host "  • Docs\ ($($docs.Count) documentation files)" -ForegroundColor Gray
Write-Host "  • Utils\ ($($utils.Count) utility scripts)" -ForegroundColor Gray
Write-Host "  • README.txt (distribution guide)" -ForegroundColor Gray
Write-Host ""
Write-Host "Ready to distribute! 🎉" -ForegroundColor Green
Write-Host ""
