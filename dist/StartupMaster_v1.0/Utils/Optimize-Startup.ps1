# Startup Optimization Script
# Provides guided optimization of Windows startup

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Windows Startup Optimizer" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "This script will help you optimize your Windows startup configuration." -ForegroundColor White
Write-Host ""

# Check for admin
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "⚠ WARNING: Not running as Administrator" -ForegroundColor Yellow
    Write-Host "Some optimizations may require admin privileges." -ForegroundColor Yellow
    Write-Host ""
    
    $elevate = Read-Host "Restart as Administrator? (Y/N)"
    if ($elevate -eq "Y" -or $elevate -eq "y") {
        Start-Process powershell -Verb RunAs -ArgumentList "-ExecutionPolicy Bypass -File `"$PSCommandPath`""
        exit
    }
}

Write-Host "Analyzing current startup configuration..." -ForegroundColor Yellow
Write-Host ""

# Count startup items
$regCurrentUser = 0
$regLocalMachine = 0
$startupFolder = 0
$tasks = 0

# Registry - Current User
$paths = @(
    "HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
    "HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce"
)

foreach ($path in $paths) {
    if (Test-Path $path) {
        $items = Get-ItemProperty -Path $path -ErrorAction SilentlyContinue
        if ($items) {
            $regCurrentUser += ($items.PSObject.Properties | Where-Object { $_.Name -notlike "PS*" }).Count
        }
    }
}

# Registry - Local Machine
$paths = @(
    "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
    "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce"
)

foreach ($path in $paths) {
    if (Test-Path $path) {
        $items = Get-ItemProperty -Path $path -ErrorAction SilentlyContinue
        if ($items) {
            $regLocalMachine += ($items.PSObject.Properties | Where-Object { $_.Name -notlike "PS*" }).Count
        }
    }
}

# Startup Folders
$userStartup = [System.IO.Path]::Combine($env:APPDATA, "Microsoft\Windows\Start Menu\Programs\Startup")
$commonStartup = [System.IO.Path]::Combine($env:ProgramData, "Microsoft\Windows\Start Menu\Programs\StartUp")

if (Test-Path $userStartup) {
    $startupFolder += (Get-ChildItem -Path $userStartup -File).Count
}
if (Test-Path $commonStartup) {
    $startupFolder += (Get-ChildItem -Path $commonStartup -File).Count
}

# Task Scheduler
try {
    $tasks = (Get-ScheduledTask | Where-Object { 
        $_.Triggers.CimClass.CimClassName -match "MSFT_TaskLogonTrigger|MSFT_TaskBootTrigger" 
    }).Count
} catch {
    $tasks = 0
}

$total = $regCurrentUser + $regLocalMachine + $startupFolder + $tasks

Write-Host "Current Startup Configuration:" -ForegroundColor Cyan
Write-Host "  Registry (Current User):  $regCurrentUser items" -ForegroundColor White
Write-Host "  Registry (Local Machine): $regLocalMachine items" -ForegroundColor White
Write-Host "  Startup Folders:          $startupFolder items" -ForegroundColor White
Write-Host "  Task Scheduler:           $tasks items" -ForegroundColor White
Write-Host "  TOTAL:                    $total items" -ForegroundColor Green
Write-Host ""

# Performance rating
$rating = "Excellent"
$color = "Green"

if ($total -gt 30) {
    $rating = "Poor"
    $color = "Red"
} elseif ($total -gt 20) {
    $rating = "Fair"
    $color = "Yellow"
} elseif ($total -gt 10) {
    $rating = "Good"
    $color = "Cyan"
}

Write-Host "Performance Rating: $rating" -ForegroundColor $color
Write-Host ""

# Recommendations
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Optimization Recommendations" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$recommendations = @()

if ($total -gt 20) {
    $recommendations += "🔴 HIGH PRIORITY: Reduce startup items (current: $total, recommended: <15)"
}

if ($regCurrentUser + $regLocalMachine -gt 10) {
    $recommendations += "🟡 MEDIUM: Move registry items to Task Scheduler for delay support"
}

if ($tasks -lt 5 -and $total -gt 10) {
    $recommendations += "🟢 LOW: Consider using Task Scheduler for better startup control"
}

$recommendations += "💡 TIP: Disable non-essential applications"
$recommendations += "💡 TIP: Use Startup Master for easy management"
$recommendations += "💡 TIP: Create a backup before making changes"

foreach ($rec in $recommendations) {
    Write-Host "  $rec" -ForegroundColor White
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$action = Read-Host "Open Startup Master for optimization? (Y/N)"

if ($action -eq "Y" -or $action -eq "y") {
    $exePath = Join-Path $PSScriptRoot "bin\Release\net8.0-windows\win-x64\publish\StartupMaster.exe"
    
    if (Test-Path $exePath) {
        Start-Process $exePath -Verb RunAs
    } else {
        Write-Host "Startup Master not found at: $exePath" -ForegroundColor Red
        Write-Host "Please build the project first using: dotnet build -c Release" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "Optimization complete!" -ForegroundColor Green
Write-Host ""
