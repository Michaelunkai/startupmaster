# Startup Configuration Analyzer
# Provides insights into your Windows startup configuration

param(
    [switch]$Detailed,
    [switch]$ExportReport
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Windows Startup Configuration Analyzer" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$report = @{
    Timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    Registry = @{
        CurrentUser = @()
        LocalMachine = @()
    }
    StartupFolders = @{
        User = @()
        Common = @()
    }
    TaskScheduler = @()
    Services = @()
    Summary = @{}
}

# Analyze Registry - Current User
Write-Host "[1/5] Analyzing Registry (Current User)..." -ForegroundColor Yellow
$regPaths = @(
    "HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
    "HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce"
)

foreach ($path in $regPaths) {
    if (Test-Path $path) {
        $items = Get-ItemProperty -Path $path -ErrorAction SilentlyContinue
        if ($items) {
            $items.PSObject.Properties | Where-Object { $_.Name -notlike "PS*" } | ForEach-Object {
                $report.Registry.CurrentUser += @{
                    Name = $_.Name
                    Command = $_.Value
                    Location = $path
                }
            }
        }
    }
}
Write-Host "   Found: $($report.Registry.CurrentUser.Count) items" -ForegroundColor Green

# Analyze Registry - Local Machine
Write-Host "[2/5] Analyzing Registry (Local Machine)..." -ForegroundColor Yellow
$regPaths = @(
    "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
    "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce",
    "HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Run"
)

foreach ($path in $regPaths) {
    if (Test-Path $path) {
        $items = Get-ItemProperty -Path $path -ErrorAction SilentlyContinue
        if ($items) {
            $items.PSObject.Properties | Where-Object { $_.Name -notlike "PS*" } | ForEach-Object {
                $report.Registry.LocalMachine += @{
                    Name = $_.Name
                    Command = $_.Value
                    Location = $path
                }
            }
        }
    }
}
Write-Host "   Found: $($report.Registry.LocalMachine.Count) items" -ForegroundColor Green

# Analyze Startup Folders
Write-Host "[3/5] Analyzing Startup Folders..." -ForegroundColor Yellow
$userStartup = [System.IO.Path]::Combine($env:APPDATA, "Microsoft\Windows\Start Menu\Programs\Startup")
$commonStartup = [System.IO.Path]::Combine($env:ProgramData, "Microsoft\Windows\Start Menu\Programs\StartUp")

if (Test-Path $userStartup) {
    $items = Get-ChildItem -Path $userStartup -File
    foreach ($item in $items) {
        $report.StartupFolders.User += @{
            Name = $item.Name
            Path = $item.FullName
            Size = $item.Length
        }
    }
}

if (Test-Path $commonStartup) {
    $items = Get-ChildItem -Path $commonStartup -File
    foreach ($item in $items) {
        $report.StartupFolders.Common += @{
            Name = $item.Name
            Path = $item.FullName
            Size = $item.Length
        }
    }
}
Write-Host "   Found: $($report.StartupFolders.User.Count + $report.StartupFolders.Common.Count) items" -ForegroundColor Green

# Analyze Task Scheduler
Write-Host "[4/5] Analyzing Task Scheduler..." -ForegroundColor Yellow
try {
    $tasks = Get-ScheduledTask | Where-Object { 
        $_.Triggers.CimClass.CimClassName -match "MSFT_TaskLogonTrigger|MSFT_TaskBootTrigger" 
    }
    
    foreach ($task in $tasks) {
        $report.TaskScheduler += @{
            Name = $task.TaskName
            Path = $task.TaskPath
            State = $task.State
            Enabled = ($task.State -ne "Disabled")
        }
    }
    Write-Host "   Found: $($report.TaskScheduler.Count) logon/boot tasks" -ForegroundColor Green
} catch {
    Write-Host "   Error accessing Task Scheduler" -ForegroundColor Red
}

# Analyze Services
Write-Host "[5/5] Analyzing Auto-Start Services..." -ForegroundColor Yellow
try {
    $services = Get-Service | Where-Object { $_.StartType -eq "Automatic" }
    foreach ($service in $services) {
        $report.Services += @{
            Name = $service.Name
            DisplayName = $service.DisplayName
            Status = $service.Status
        }
    }
    Write-Host "   Found: $($report.Services.Count) automatic services" -ForegroundColor Green
} catch {
    Write-Host "   Error accessing Services" -ForegroundColor Red
}

# Generate Summary
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$totalRegistry = $report.Registry.CurrentUser.Count + $report.Registry.LocalMachine.Count
$totalFolders = $report.StartupFolders.User.Count + $report.StartupFolders.Common.Count
$totalTasks = $report.TaskScheduler.Count
$totalServices = $report.Services.Count
$grandTotal = $totalRegistry + $totalFolders + $totalTasks

Write-Host ""
Write-Host "Registry Items:        $totalRegistry" -ForegroundColor White
Write-Host "  - Current User:      $($report.Registry.CurrentUser.Count)" -ForegroundColor Gray
Write-Host "  - Local Machine:     $($report.Registry.LocalMachine.Count)" -ForegroundColor Gray
Write-Host ""
Write-Host "Startup Folder Items:  $totalFolders" -ForegroundColor White
Write-Host "  - User:              $($report.StartupFolders.User.Count)" -ForegroundColor Gray
Write-Host "  - Common:            $($report.StartupFolders.Common.Count)" -ForegroundColor Gray
Write-Host ""
Write-Host "Task Scheduler Items:  $totalTasks" -ForegroundColor White
Write-Host "Auto-Start Services:   $totalServices" -ForegroundColor White
Write-Host ""
Write-Host "TOTAL STARTUP ITEMS:   $grandTotal" -ForegroundColor Green -BackgroundColor Black
Write-Host ""

$report.Summary = @{
    TotalRegistry = $totalRegistry
    TotalFolders = $totalFolders
    TotalTasks = $totalTasks
    TotalServices = $totalServices
    GrandTotal = $grandTotal
}

# Detailed Output
if ($Detailed) {
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "   Detailed Report" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    
    Write-Host "Registry (Current User):" -ForegroundColor Yellow
    $report.Registry.CurrentUser | ForEach-Object {
        Write-Host "  - $($_.Name): $($_.Command)" -ForegroundColor Gray
    }
    
    Write-Host ""
    Write-Host "Registry (Local Machine):" -ForegroundColor Yellow
    $report.Registry.LocalMachine | ForEach-Object {
        Write-Host "  - $($_.Name): $($_.Command)" -ForegroundColor Gray
    }
    
    Write-Host ""
    Write-Host "Startup Folders:" -ForegroundColor Yellow
    $report.StartupFolders.User | ForEach-Object {
        Write-Host "  - [User] $($_.Name)" -ForegroundColor Gray
    }
    $report.StartupFolders.Common | ForEach-Object {
        Write-Host "  - [Common] $($_.Name)" -ForegroundColor Gray
    }
    
    Write-Host ""
    Write-Host "Task Scheduler:" -ForegroundColor Yellow
    $report.TaskScheduler | ForEach-Object {
        $status = if ($_.Enabled) { "Enabled" } else { "Disabled" }
        Write-Host "  - $($_.Name) [$status]" -ForegroundColor Gray
    }
    
    Write-Host ""
    Write-Host "Top 10 Services:" -ForegroundColor Yellow
    $report.Services | Select-Object -First 10 | ForEach-Object {
        Write-Host "  - $($_.DisplayName) [$($_.Status)]" -ForegroundColor Gray
    }
}

# Export Report
if ($ExportReport) {
    $exportPath = "StartupReport_$(Get-Date -Format 'yyyyMMdd_HHmmss').json"
    $report | ConvertTo-Json -Depth 10 | Out-File $exportPath -Encoding UTF8
    Write-Host "Report exported to: $exportPath" -ForegroundColor Green
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Recommendations:" -ForegroundColor Yellow
Write-Host "  - Review items you don't recognize" -ForegroundColor White
Write-Host "  - Disable non-essential startup items" -ForegroundColor White
Write-Host "  - Use Task Scheduler for delayed starts" -ForegroundColor White
Write-Host "  - Backup configuration before changes" -ForegroundColor White
Write-Host ""
Write-Host "Use Startup Master for easy management!" -ForegroundColor Cyan
Write-Host ""
