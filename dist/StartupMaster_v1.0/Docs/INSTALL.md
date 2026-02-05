# 📦 Startup Master - Installation & Quick Start Guide

## System Requirements

- **Operating System**: Windows 10 (1809+) or Windows 11
- **Framework**: .NET 8.0 Runtime or higher
- **Privileges**: Administrator access (for system-wide management)
- **Disk Space**: ~50 MB
- **RAM**: 100 MB minimum

## Installation Methods

### Method 1: Quick Start (Recommended)

1. **Download/Navigate to Release**
   ```
   C:\Users\micha\.openclaw\workspace\StartupMaster\bin\Release\net8.0-windows\win-x64\publish\
   ```

2. **Run the Application**
   - Double-click `StartupMaster.exe`
   - OR use the launcher: `Launch-StartupMaster.bat` (from project root)
   - Click "Yes" on the UAC elevation prompt

3. **First Run**
   - Application scans all startup locations automatically
   - Main window displays all discovered items
   - You're ready to manage startup items!

### Method 2: Build from Source

**Prerequisites:**
- Visual Studio 2022 or JetBrains Rider
- .NET 8.0 SDK installed

**Steps:**

1. **Open Project**
   ```bash
   cd C:\Users\micha\.openclaw\workspace\StartupMaster
   ```

2. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

3. **Build**
   ```bash
   # Debug build
   dotnet build
   
   # Release build
   dotnet build -c Release
   ```

4. **Run**
   ```bash
   dotnet run
   ```

### Method 3: Publish Standalone

Create a redistributable version:

```bash
cd C:\Users\micha\.openclaw\workspace\StartupMaster

# Publish single-file executable
dotnet publish -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true

# Output location:
# bin\Release\net8.0-windows\win-x64\publish\StartupMaster.exe
```

**Self-Contained (No .NET Required):**
```bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```
*Note: Self-contained builds are larger (~80 MB) but don't require .NET Runtime*

## .NET Runtime Installation

If you don't have .NET 8.0 Runtime installed:

1. **Download**: https://dotnet.microsoft.com/download/dotnet/8.0
2. **Select**: ".NET Desktop Runtime 8.0.x" for Windows x64
3. **Install**: Run the installer
4. **Verify**: Open PowerShell and run `dotnet --version`

## First Launch Setup

### 1. Administrator Elevation

**Why?** Startup Master needs admin rights to:
- Read/modify Local Machine registry
- Manage Task Scheduler items
- Control Windows Services
- Access common startup folders

**What to expect:**
- UAC prompt appears on first launch
- Click "Yes" to grant elevation
- Application icon shows admin shield

**Without admin:**
- User-level items still work (Current User registry, user startup folder)
- System-level features will show errors

### 2. Initial Scan

On first launch, the app scans:
- ✅ Registry (HKCU & HKLM)
- ✅ Startup Folders (User & Common)
- ✅ Task Scheduler (logon/boot triggers)
- ✅ Windows Services (automatic startup)

**Scan time:** 2-5 seconds (depending on number of services)

### 3. Main Window Overview

**Toolbar:**
- 🔄 **Refresh**: Re-scan all locations
- ➕ **Add New**: Create a new startup item
- 💾 **Export**: Backup current configuration
- 📂 **Import**: Restore from backup

**Filter Area:**
- Dropdown: Filter by location or status
- Search box: Find items by name/command

**Item Grid:**
- Status, Name, Command, Location, Delay
- Action buttons: Enable/Disable/Edit/Remove

**Status Bar:**
- Current action feedback
- Item count (filtered / total)

## Quick Start Tutorial

### Adding Your First Startup Item

1. Click **"➕ Add New"**
2. Enter details:
   - **Name**: `My Application`
   - **Command**: Click 📁 Browse and select an .exe
   - **Arguments**: (optional) e.g., `--minimized`
   - **Location**: `Task Scheduler` (for delay support)
   - **Delay**: Slide to 30 seconds
3. Click **Save**
4. Item appears in the main grid ✅

### Disabling a Startup Item

1. Locate the item in the grid
2. Click the **✗ Disable** button
3. Status changes to "✗ Disabled"
4. Item won't run at next startup
5. Re-enable anytime with **✓ Enable**

### Removing a Startup Item

1. Find the item you want to remove
2. Click **🗑️ Remove**
3. Confirm the deletion
4. Item is permanently removed

**TIP:** Export a backup before removing items!

### Backing Up Your Configuration

1. Click **"💾 Export"**
2. Choose a save location
3. Enter filename (e.g., `StartupBackup_2026-02-05.json`)
4. Click **Save**
5. File contains all items and settings

### Restoring from Backup

1. Click **"📂 Import"**
2. Select your backup JSON file
3. Review the import count
4. Click **Yes** to confirm
5. Items are added to current configuration

## Utility Scripts

### Startup Analyzer

Analyze your current startup configuration without the GUI:

```powershell
cd C:\Users\micha\.openclaw\workspace\StartupMaster
.\Analyze-Startup.ps1
```

**Options:**
- `-Detailed` - Show all items in detail
- `-ExportReport` - Save JSON report to file

**Example:**
```powershell
.\Analyze-Startup.ps1 -Detailed -ExportReport
```

## Troubleshooting

### "Access Denied" Errors

**Solution 1:** Run as Administrator
- Right-click `StartupMaster.exe`
- Select "Run as administrator"

**Solution 2:** Check UAC Settings
- Open Control Panel → User Accounts
- Change User Account Control settings
- Set to at least "Default" level

### Application Won't Start

**Check .NET Installation:**
```powershell
dotnet --version
```

Should show `8.0.x` or higher.

**If not installed:**
- Download from https://dotnet.microsoft.com/download/dotnet/8.0
- Install ".NET Desktop Runtime"

### Changes Not Taking Effect

**For Registry/Folders:**
- Changes are immediate
- Close and reopen app to verify

**For Task Scheduler:**
- Changes are immediate
- Check Task Scheduler (taskschd.msc) to verify

**For Services:**
- Startup type changes are immediate
- Service start/stop requires system restart

### Items Not Showing Up

**Refresh the list:**
- Click 🔄 Refresh button
- App re-scans all locations

**Check filters:**
- Set filter to "All Items"
- Clear search box
- Verify location filter

### "Task Scheduler Error"

**Verify Task Scheduler service:**
```powershell
Get-Service -Name "Schedule" | Select-Object Status
```

**Should show:** `Running`

**If stopped:**
```powershell
Start-Service -Name "Schedule"
```

## Uninstallation

Startup Master is portable - no installation required.

**To remove:**
1. Delete the application folder
2. (Optional) Remove any startup items you added via the app

**Note:** Your Windows startup configuration remains unchanged unless you explicitly modified it through the app.

## Advanced Configuration

### Creating a Desktop Shortcut

1. Navigate to:
   ```
   C:\Users\micha\.openclaw\workspace\StartupMaster\bin\Release\net8.0-windows\win-x64\publish\
   ```
2. Right-click `StartupMaster.exe`
3. Send to → Desktop (create shortcut)
4. (Optional) Right-click shortcut → Properties → Advanced → "Run as administrator" ✓

### Adding to Quick Access

1. Right-click the publish folder
2. Pin to Quick access

### Pinning to Taskbar

1. Run `StartupMaster.exe`
2. Right-click the taskbar icon
3. Pin to taskbar

## Data & Privacy

**No telemetry:** Startup Master doesn't send any data anywhere.

**No internet required:** Works completely offline.

**Local only:** All operations are local to your PC.

**Export files:** JSON format, human-readable, no encryption.

## Getting Help

**Documentation:**
- `README.md` - Overview and features
- `FEATURES.md` - Complete feature list
- `DEVELOPER.md` - Technical details
- `INSTALL.md` - This file

**Logs:**
- Check Windows Event Viewer for errors
- Application doesn't create log files by default

**Support:**
- Review documentation first
- Check the source code (fully commented)
- Examine exported JSON to understand format

## Next Steps

1. ✅ **Backup First**: Export your current startup config
2. 🧹 **Clean Up**: Disable/remove unused startup items
3. ⚡ **Optimize**: Add delays to non-critical items (Task Scheduler)
4. 📊 **Analyze**: Run `Analyze-Startup.ps1` to see the impact
5. 💾 **Backup Again**: Export your optimized configuration

---

**Version**: 1.0  
**Last Updated**: 2026-02-05  
**Project**: StartupMaster  
**Built With**: OpenClaw Marathon Mode
