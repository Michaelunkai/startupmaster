# Quick Disable All Startup Items (Emergency Script)
# Use this to quickly disable ALL startup items if Windows is booting slowly

param(
    [switch]$Confirm = $true,
    [switch]$Backup = $true
)

Write-Host "========================================" -ForegroundColor Red
Write-Host "   EMERGENCY STARTUP DISABLE" -ForegroundColor Red
Write-Host "========================================" -ForegroundColor Red
Write-Host ""
Write-Host "⚠ WARNING: This will disable ALL user startup items!" -ForegroundColor Yellow
Write-Host ""

if ($Confirm) {
    $response = Read-Host "Are you sure? Type 'YES' to continue"
    if ($response -ne "YES") {
        Write-Host "Cancelled." -ForegroundColor Yellow
        exit
    }
}

# Check admin
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "⚠ Running without admin - only user-level items will be disabled" -ForegroundColor Yellow
}

$disabledCount = 0

# Backup first
if ($Backup) {
    Write-Host "Creating backup..." -ForegroundColor Yellow
    $backupPath = "StartupBackup_Emergency_$(Get-Date -Format 'yyyyMMdd_HHmmss').reg"
    
    reg export "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run" $backupPath /y | Out-Null
    
    Write-Host "  Backup saved to: $backupPath" -ForegroundColor Green
}

# Disable Registry - Current User
Write-Host ""
Write-Host "Disabling Registry (Current User) items..." -ForegroundColor Yellow

$runPath = "HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Run"
$disabledPath = "HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Run-Disabled"

if (Test-Path $runPath) {
    $items = Get-ItemProperty -Path $runPath -ErrorAction SilentlyContinue
    
    if ($items) {
        # Create disabled key if it doesn't exist
        if (-not (Test-Path $disabledPath)) {
            New-Item -Path $disabledPath -Force | Out-Null
        }
        
        $items.PSObject.Properties | Where-Object { $_.Name -notlike "PS*" } | ForEach-Object {
            try {
                # Copy to disabled
                Set-ItemProperty -Path $disabledPath -Name $_.Name -Value $_.Value -ErrorAction Stop
                # Remove from active
                Remove-ItemProperty -Path $runPath -Name $_.Name -ErrorAction Stop
                Write-Host "  ✓ Disabled: $($_.Name)" -ForegroundColor Gray
                $disabledCount++
            } catch {
                Write-Host "  ✗ Failed: $($_.Name)" -ForegroundColor Red
            }
        }
    }
}

# Disable Startup Folder items
Write-Host ""
Write-Host "Disabling Startup Folder items..." -ForegroundColor Yellow

$userStartup = [System.IO.Path]::Combine($env:APPDATA, "Microsoft\Windows\Start Menu\Programs\Startup")

if (Test-Path $userStartup) {
    Get-ChildItem -Path $userStartup -File | ForEach-Object {
        try {
            $newName = $_.FullName + ".disabled"
            Rename-Item -Path $_.FullName -NewName $newName -ErrorAction Stop
            Write-Host "  ✓ Disabled: $($_.Name)" -ForegroundColor Gray
            $disabledCount++
        } catch {
            Write-Host "  ✗ Failed: $($_.Name)" -ForegroundColor Red
        }
    }
}

# Disable Task Scheduler (user tasks only)
Write-Host ""
Write-Host "Disabling Task Scheduler items..." -ForegroundColor Yellow

try {
    $tasks = Get-ScheduledTask | Where-Object { 
        $_.TaskPath -like "*Users*" -and
        $_.Triggers.CimClass.CimClassName -match "MSFT_TaskLogonTrigger"
    }
    
    foreach ($task in $tasks) {
        try {
            Disable-ScheduledTask -TaskName $task.TaskName -ErrorAction Stop | Out-Null
            Write-Host "  ✓ Disabled: $($task.TaskName)" -ForegroundColor Gray
            $disabledCount++
        } catch {
            Write-Host "  ✗ Failed: $($task.TaskName)" -ForegroundColor Red
        }
    }
} catch {
    Write-Host "  ⚠ Could not access Task Scheduler" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "   SUMMARY" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Total items disabled: $disabledCount" -ForegroundColor White
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Restart your computer" -ForegroundColor White
Write-Host "  2. Boot should be much faster" -ForegroundColor White
Write-Host "  3. Use Startup Master to selectively re-enable needed items" -ForegroundColor White
Write-Host ""

if ($Backup) {
    Write-Host "To restore everything:" -ForegroundColor Yellow
    Write-Host "  reg import $backupPath" -ForegroundColor White
    Write-Host ""
}

Write-Host "Done!" -ForegroundColor Green
