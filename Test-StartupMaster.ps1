# Startup Master Test Suite
# Comprehensive testing script for validation

param(
    [switch]$Verbose
)

$testsPassed = 0
$testsFailed = 0
$warnings = 0

function Test-Case {
    param(
        [string]$Name,
        [scriptblock]$Test
    )
    
    Write-Host "Testing: $Name..." -NoNewline -ForegroundColor Cyan
    
    try {
        $result = & $Test
        if ($result) {
            Write-Host " ✓ PASS" -ForegroundColor Green
            $script:testsPassed++
            return $true
        } else {
            Write-Host " ✗ FAIL" -ForegroundColor Red
            $script:testsFailed++
            return $false
        }
    } catch {
        Write-Host " ✗ ERROR: $($_.Exception.Message)" -ForegroundColor Red
        $script:testsFailed++
        return $false
    }
}

function Write-Warning-Custom {
    param([string]$Message)
    Write-Host "  ⚠ $Message" -ForegroundColor Yellow
    $script:warnings++
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Startup Master Test Suite" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Test 1: Project Structure
Test-Case "Project structure exists" {
    $projectPath = "$PSScriptRoot\StartupMaster.csproj"
    Test-Path $projectPath
}

# Test 2: Source Files
Test-Case "Core source files present" {
    $files = @(
        "App.xaml",
        "App.xaml.cs",
        "MainWindow.xaml",
        "MainWindow.xaml.cs",
        "Models\StartupItem.cs",
        "Services\StartupManager.cs"
    )
    
    $allExist = $true
    foreach ($file in $files) {
        if (-not (Test-Path (Join-Path $PSScriptRoot $file))) {
            if ($Verbose) { Write-Host "Missing: $file" }
            $allExist = $false
        }
    }
    $allExist
}

# Test 3: Build Output
Test-Case "Release build exists" {
    $exePath = Join-Path $PSScriptRoot "bin\Release\net8.0-windows\StartupMaster.dll"
    Test-Path $exePath
}

# Test 4: Published Output
Test-Case "Published executable exists" {
    $publishPath = Join-Path $PSScriptRoot "bin\Release\net8.0-windows\win-x64\publish\StartupMaster.exe"
    Test-Path $publishPath
}

# Test 5: Documentation
Test-Case "Documentation files present" {
    $docs = @("README.md", "INSTALL.md", "FEATURES.md", "DEVELOPER.md", "CHANGELOG.md")
    $allExist = $true
    foreach ($doc in $docs) {
        if (-not (Test-Path (Join-Path $PSScriptRoot $doc))) {
            $allExist = $false
        }
    }
    $allExist
}

# Test 6: Utility Scripts
Test-Case "Utility scripts present" {
    $scripts = @("Analyze-Startup.ps1", "Optimize-Startup.ps1", "Quick-Disable-All.ps1")
    $allExist = $true
    foreach ($script in $scripts) {
        if (-not (Test-Path (Join-Path $PSScriptRoot $script))) {
            $allExist = $false
        }
    }
    $allExist
}

# Test 7: .NET Runtime
Write-Host "Testing: .NET Runtime available..." -NoNewline -ForegroundColor Cyan
try {
    $dotnetVersion = dotnet --version 2>&1 | Out-String
    $dotnetVersion = $dotnetVersion.Trim()
    if ($dotnetVersion -and $dotnetVersion -match '\d+\.\d+') {
        $major = [int]($dotnetVersion.Split('.')[0])
        if ($major -ge 8) {
            Write-Host " ✓ PASS (v$dotnetVersion)" -ForegroundColor Green
            $testsPassed++
        } else {
            Write-Host " ✗ FAIL (v$dotnetVersion < 8.0)" -ForegroundColor Red
            Write-Warning-Custom ".NET 8.0 or higher required"
            $testsFailed++
        }
    } else {
        Write-Host " ✗ FAIL (.NET not found)" -ForegroundColor Red
        $testsFailed++
    }
} catch {
    Write-Host " ✗ FAIL (.NET not installed)" -ForegroundColor Red
    $testsFailed++
}

# Test 8: Admin Elevation Manifest
Test-Case "Admin elevation manifest exists" {
    Test-Path (Join-Path $PSScriptRoot "app.manifest")
}

# Test 9: Project Configuration
Test-Case "Project targets net8.0-windows" {
    $csproj = Get-Content (Join-Path $PSScriptRoot "StartupMaster.csproj") -Raw
    $csproj -match "<TargetFramework>net8.0-windows</TargetFramework>"
}

# Test 10: NuGet Dependencies
Test-Case "NuGet packages configured" {
    $csproj = Get-Content (Join-Path $PSScriptRoot "StartupMaster.csproj") -Raw
    ($csproj -match "TaskScheduler") -and ($csproj -match "ModernWpfUI")
}

# Test 11: Functional Test (if executable exists)
$exePath = Join-Path $PSScriptRoot "bin\Release\net8.0-windows\win-x64\publish\StartupMaster.exe"
if (Test-Path $exePath) {
    Write-Host "Testing: Application launches..." -NoNewline -ForegroundColor Cyan
    try {
        $process = Start-Process $exePath -PassThru -ErrorAction Stop
        Start-Sleep -Seconds 2
        
        if ($process.HasExited) {
            Write-Host " ✗ FAIL (crashed on launch)" -ForegroundColor Red
            $testsFailed++
        } else {
            try { $process.Kill() } catch {}
            Write-Host " ✓ PASS" -ForegroundColor Green
            $testsPassed++
        }
    } catch {
        Write-Host " ✗ FAIL ($($_.Exception.Message))" -ForegroundColor Red
        $testsFailed++
    }
} else {
    Write-Host "Testing: Application launches..." -NoNewline -ForegroundColor Cyan
    Write-Host " ⚠ SKIP (executable not found)" -ForegroundColor Yellow
    $warnings++
}

# Test 12: File Sizes
Test-Case "Documentation is comprehensive (>1KB each)" {
    $docs = @("README.md", "INSTALL.md", "FEATURES.md", "DEVELOPER.md")
    $allGood = $true
    foreach ($doc in $docs) {
        $path = Join-Path $PSScriptRoot $doc
        if (Test-Path $path) {
            $size = (Get-Item $path).Length
            if ($size -lt 1024) {
                if ($Verbose) { Write-Host "$doc is too small: $size bytes" }
                $allGood = $false
            }
        }
    }
    $allGood
}

# Test 13: Code Quality Checks
Test-Case "Source files have error handling" {
    $servicesPath = Join-Path $PSScriptRoot "Services\RegistryStartupManager.cs"
    if (Test-Path $servicesPath) {
        $content = Get-Content $servicesPath -Raw
        ($content -match "try") -and ($content -match "catch")
    } else {
        $false
    }
}

# Environment Checks (non-critical)
Write-Host ""
Write-Host "Environment Checks:" -ForegroundColor Cyan

# Check Windows version
$os = Get-CimInstance Win32_OperatingSystem
Write-Host "  OS: $($os.Caption) (Build $($os.BuildNumber))" -ForegroundColor Gray

if ([int]$os.BuildNumber -lt 17763) {
    Write-Warning-Custom "Windows 10 1809+ recommended"
}

# Check RAM
$ram = [math]::Round((Get-CimInstance Win32_ComputerSystem).TotalPhysicalMemory / 1GB, 1)
Write-Host "  RAM: $ram GB" -ForegroundColor Gray

if ($ram -lt 4) {
    Write-Warning-Custom "4GB+ RAM recommended"
}

# Check disk space
$drive = Get-PSDrive C
$freeGB = [math]::Round($drive.Free / 1GB, 1)
Write-Host "  Free Space (C:): $freeGB GB" -ForegroundColor Gray

if ($freeGB -lt 1) {
    Write-Warning-Custom "Low disk space on C: drive"
}

# Check PowerShell version
Write-Host "  PowerShell: $($PSVersionTable.PSVersion)" -ForegroundColor Gray

# Summary
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Test Results" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Passed:   $testsPassed" -ForegroundColor Green
Write-Host "Failed:   $testsFailed" -ForegroundColor $(if ($testsFailed -eq 0) { "Green" } else { "Red" })
Write-Host "Warnings: $warnings" -ForegroundColor Yellow
Write-Host ""

$total = $testsPassed + $testsFailed
$percentage = if ($total -gt 0) { [math]::Round(($testsPassed / $total) * 100, 1) } else { 0 }

Write-Host "Success Rate: $percentage%" -ForegroundColor $(
    if ($percentage -eq 100) { "Green" }
    elseif ($percentage -ge 80) { "Cyan" }
    elseif ($percentage -ge 60) { "Yellow" }
    else { "Red" }
)
Write-Host ""

if ($testsFailed -eq 0) {
    Write-Host "✓ All tests passed! Startup Master is ready to use." -ForegroundColor Green
} else {
    Write-Host "⚠ Some tests failed. Review errors above." -ForegroundColor Yellow
}

Write-Host ""

# Exit code
exit $testsFailed
