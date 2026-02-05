# 🎓 Complete Guide - Startup Master

**Everything you need to know about managing Windows startup with Startup Master**

---

## Table of Contents

1. [Introduction](#introduction)
2. [Installation](#installation)
3. [First Launch](#first-launch)
4. [Understanding Startup Locations](#understanding-startup-locations)
5. [Basic Operations](#basic-operations)
6. [Advanced Features](#advanced-features)
7. [Optimization Strategies](#optimization-strategies)
8. [Troubleshooting](#troubleshooting)
9. [Best Practices](#best-practices)
10. [FAQ](#faq)

---

## Introduction

### What is Startup Master?

Startup Master is a comprehensive Windows startup management tool that gives you complete control over what runs when your computer boots. Unlike built-in Windows tools, it provides:

- **Unified interface** - All startup locations in one place
- **Non-destructive control** - Disable without deleting
- **Delay management** - Control when items start
- **Auto-backup** - Safety-first design
- **Performance analysis** - Built-in optimization tools

### Why Do You Need It?

**Slow Boot Times**
- Too many startup items slow down Windows boot
- Resource-heavy apps compete for CPU/memory
- Proper management speeds up boot significantly

**System Clutter**
- Programs add themselves to startup without asking
- Old entries remain after uninstallation
- Duplicate entries waste resources

**Performance Optimization**
- Strategic delays reduce boot spike
- Non-essential items can start later
- Services can be managed efficiently

---

## Installation

### Requirements

**Operating System:**
- Windows 10 (version 1809 or later)
- Windows 11 (any version)

**Software:**
- .NET 8.0 Desktop Runtime (if using framework-dependent build)
- Administrator privileges (for system-wide changes)

**Hardware:**
- 100 MB RAM minimum
- 50 MB disk space
- Any modern Windows PC

### Installation Steps

**Method 1: ZIP Distribution**

1. Download `StartupMaster_v1.0.zip`
2. Extract to desired location (e.g., `C:\Program Files\StartupMaster`)
3. Run `Launch.bat` or `StartupMaster.exe`
4. Click "Yes" on UAC prompt

**Method 2: Self-Contained Build**

1. Download `StartupMaster_v1.0_Standalone.zip`
2. Extract anywhere
3. Run `StartupMaster.exe` (no .NET required)
4. Larger file (~80 MB) but no dependencies

### First-Time Setup

**Admin Elevation:**
- UAC prompt appears on first launch
- Click "Yes" to grant administrator rights
- Required for system-wide management

**Initial Scan:**
- Application scans all startup locations
- Takes 2-5 seconds
- Results displayed immediately

---

## First Launch

### What You'll See

**Main Window Layout:**

```
┌─────────────────────────────────────────────────────┐
│ 🚀 Startup Master - Ultimate Windows Startup Manager│
├─────────────────────────────────────────────────────┤
│ [🔄 Refresh] [➕ Add] [💾 Export] [📂 Import]       │
├─────────────────────────────────────────────────────┤
│ Filter: [All Items ▼] Search: [____________]        │
├─────────────────────────────────────────────────────┤
│ Status │ Name         │ Command      │ Location     │
│───────┼──────────────┼──────────────┼──────────────│
│ ✓     │ OneDrive     │ C:\...exe    │ Registry (U) │
│ ✗     │ OldApp       │ C:\...exe    │ Task Sched.  │
│ ✓     │ AutoStart    │ C:\...exe    │ Startup Fld. │
├─────────────────────────────────────────────────────┤
│ Ready                             3 of 12 items     │
└─────────────────────────────────────────────────────┘
```

**Status Bar:**
- Left: Current action/status
- Right: Item count (filtered / total)

---

## Understanding Startup Locations

Windows has **5 main startup locations**. Startup Master manages them all:

### 1. Registry (Current User)

**Path:** `HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run`

**Characteristics:**
- User-specific (only affects your account)
- Requires NO admin rights
- Most common location for user apps
- Easy to add/remove

**Best for:**
- Personal applications
- User-specific tools
- Development tools

### 2. Registry (Local Machine)

**Path:** `HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Run`

**Characteristics:**
- System-wide (affects all users)
- Requires admin rights
- Used by system software
- More persistent

**Best for:**
- Enterprise software
- System utilities
- Multi-user applications

### 3. Startup Folder

**Paths:**
- User: `%AppData%\Microsoft\Windows\Start Menu\Programs\Startup`
- Common: `%ProgramData%\Microsoft\Windows\Start Menu\Programs\StartUp`

**Characteristics:**
- Simple shortcut files (.lnk)
- Visible in Windows Explorer
- Easy manual management
- No delay support

**Best for:**
- Scripts (.bat, .vbs)
- Simple applications
- User-visible startup items

### 4. Task Scheduler

**Location:** Windows Task Scheduler service

**Characteristics:**
- **Supports startup delays** (0-300 seconds)
- Advanced trigger control
- Most flexible option
- Requires admin rights

**Best for:**
- Resource-heavy applications
- Delayed startup items
- Advanced scheduling needs
- Boot optimization

### 5. Windows Services

**Location:** Windows Services Manager

**Characteristics:**
- Background system services
- Automatic startup type
- Requires admin rights
- System-critical items

**Best for:**
- System services only
- Background daemons
- Server applications

---

## Basic Operations

### Adding a Startup Item

**Step-by-step:**

1. Click **"➕ Add New"** in toolbar
2. Fill in the form:

   **Name:**
   ```
   My Application
   ```
   
   **Command:**
   ```
   C:\Program Files\MyApp\app.exe
   ```
   (Click 📁 Browse to select file)
   
   **Arguments:** (optional)
   ```
   --minimized --silent
   ```
   
   **Location:** Choose where to register:
   - Registry (Current User) - User-level
   - Registry (Local Machine) - System-wide
   - Startup Folder - Simple shortcut
   - Task Scheduler - With delay support
   
   **Delay:** (Task Scheduler only)
   ```
   [Slider: 0 ──────●──────── 300] 30s
   ```

3. Click **"Save"**

**Result:** Item appears in main grid, enabled

---

### Disabling a Startup Item

**Why disable instead of remove?**
- Non-destructive (can re-enable easily)
- Preserves configuration
- Safer than deletion

**Steps:**

1. Locate item in grid
2. Click **✗ Disable** button
3. Status changes to "✗ Disabled"
4. Auto-backup created

**Result:** Item won't run at next startup

---

### Re-enabling a Disabled Item

**Steps:**

1. Find disabled item (filter: "Disabled Only")
2. Click **✓ Enable** button
3. Status changes to "✓ Enabled"

**Result:** Item runs at next startup

---

### Removing a Startup Item

⚠️ **Warning:** This permanently removes the item!

**Steps:**

1. Find item to remove
2. Click **🗑️ Remove** button
3. Confirmation dialog appears
4. Click **"Yes"** to confirm
5. Auto-backup created before deletion

**Result:** Item permanently removed from startup

---

### Editing an Item

**Steps:**

1. Click **✏️ Edit** button on item
2. Modify properties (mainly delay for Task Scheduler items)
3. Click **"Save"**

**Result:** Item updated with new settings

---

## Advanced Features

### Search & Filter

**Real-Time Search:**
```
[Search: chrome____________]
```
- Searches name and command path
- Case-insensitive
- Instant results

**Filtering:**
```
[Filter: ▼]
- All Items
- Enabled Only
- Disabled Only
- Registry
- Startup Folder
- Task Scheduler
- Services
```

---

### Export Configuration

**Purpose:**
- Backup current startup configuration
- Migrate to another PC
- Share configurations

**Steps:**

1. Click **"💾 Export"**
2. Choose save location
3. Enter filename: `StartupBackup_2026-02-05.json`
4. Click **"Save"**

**Output:** JSON file with all startup items

---

### Import Configuration

**Purpose:**
- Restore from backup
- Apply saved configuration
- Bulk add items

**Steps:**

1. Click **"📂 Import"**
2. Select JSON file
3. Review item count
4. Click **"Yes"** to import

**Result:** Items added to current configuration (doesn't remove existing)

---

### Auto-Backup System

**Automatic backups created:**
- Before deleting an item
- Before disabling an item
- On application close (if changes made)
- On first load

**Backup location:**
```
%AppData%\StartupMaster\Backups\
```

**Rotation:** Keeps last 50 backups

---

## Optimization Strategies

### The 3-Tier Startup Strategy

**Tier 1: Immediate (0s delay)**
- Security software (antivirus)
- System utilities (drivers)
- Essential tools (input methods)

**Tier 2: Fast (10-30s delay)**
- Productivity apps (Office, browsers)
- Communication tools (email, chat)
- Frequently used software

**Tier 3: Delayed (60-300s delay)**
- Cloud sync (OneDrive, Dropbox)
- Update checkers
- Non-essential background apps
- System monitors

### Identification Guide

**Essential (Keep Enabled):**
- Security software
- Device drivers
- Input method editors
- System utilities

**Optional (Can Disable):**
- Updaters (Adobe, Java, etc.)
- Helper tools (browser helpers)
- Duplicate entries
- Old/unused software

**Bloatware (Disable/Remove):**
- Trial software
- Pre-installed apps you don't use
- Manufacturer utilities (if not needed)
- Multiple versions of same app

---

### Performance Analysis

**Run the analyzer:**
```powershell
cd Utils
.\Analyze-Startup.ps1 -Detailed
```

**Output includes:**
- Total startup items
- Breakdown by location
- Performance rating
- Recommendations

**Performance Ratings:**
- **Excellent:** 0-10 items
- **Good:** 11-15 items
- **Fair:** 16-20 items
- **Poor:** 21+ items

---

### Guided Optimization

**Run the optimizer:**
```powershell
cd Utils
.\Optimize-Startup.ps1
```

**Workflow:**
1. Analyzes current configuration
2. Provides performance rating
3. Lists recommendations
4. Offers to launch Startup Master

---

## Troubleshooting

### Application Won't Start

**Symptom:** Double-click does nothing

**Solution:**
1. Check .NET Runtime installed
   ```powershell
   dotnet --version
   ```
   Should show 8.0.x or higher

2. Download .NET 8.0 Desktop Runtime if missing:
   https://dotnet.microsoft.com/download/dotnet/8.0

3. Run as Administrator:
   Right-click → "Run as administrator"

---

### "Access Denied" Errors

**Symptom:** Can't modify system items

**Solution:**
1. Ensure you clicked "Yes" on UAC prompt
2. Right-click app → "Run as administrator"
3. Check User Account Control settings

---

### Changes Not Taking Effect

**Symptom:** Disabled items still run

**Solution:**
1. Click 🔄 Refresh to verify change
2. Restart computer for changes to apply
3. Check if item exists in multiple locations

---

### Item Won't Delete

**Symptom:** Remove button fails

**Solution:**
1. Try disabling first
2. Check if protected by Group Policy
3. Use Task Manager to end running process
4. Restart and try again

---

### Missing Startup Items

**Symptom:** Expected items don't show

**Solution:**
1. Click 🔄 Refresh
2. Clear all filters (set to "All Items")
3. Clear search box
4. Check Windows Task Manager startup tab for comparison

---

## Best Practices

### DO ✅

**Before Making Changes:**
- Export a backup first
- Note what you're changing
- Research unfamiliar items

**When Optimizing:**
- Disable (don't remove) unknown items
- Test one change at a time
- Restart to verify changes
- Keep security software enabled

**Regular Maintenance:**
- Review startup items monthly
- Remove uninstalled program entries
- Update startup configuration after new installs
- Keep backup exports

---

### DON'T ❌

**Don't Remove:**
- Security software (antivirus, firewall)
- System drivers
- Windows components
- Items you don't recognize (Google first!)

**Don't Disable:**
- Critical system services
- Device drivers
- Security software
- Input methods you use

**Don't Forget:**
- To restart after changes
- To backup before major changes
- To document what you changed
- To test boot performance

---

## FAQ

### Q: Is it safe to use?

**A:** Yes! Startup Master:
- Creates auto-backups before changes
- Non-destructive disable feature
- Confirmation for deletions
- No hidden operations

### Q: Will it speed up my computer?

**A:** It speeds up boot time by:
- Reducing immediate startup load
- Enabling strategic delays
- Removing unnecessary items

**Won't affect:** Overall system speed after boot completes.

### Q: Can I break Windows?

**A:** Very unlikely if you:
- Don't remove Windows components
- Keep security software enabled
- Research before removing unknown items
- Use backups

### Q: What's the difference between disable and remove?

**A:**
- **Disable:** Turns off but keeps configuration (reversible)
- **Remove:** Permanently deletes entry (use for uninstalled apps)

### Q: Can I use it without admin rights?

**A:** Partially:
- ✅ View all items
- ✅ Manage Current User registry
- ✅ Manage user startup folder
- ❌ Can't manage system-wide items
- ❌ Can't manage Task Scheduler
- ❌ Can't manage Services

### Q: How do I restore a backup?

**A:** Use Import:
1. Click "📂 Import"
2. Select your backup JSON file
3. Confirm import

### Q: What if I disabled something important?

**A:** 
1. Open Startup Master
2. Filter: "Disabled Only"
3. Find the item
4. Click ✓ Enable
5. Restart

### Q: How often should I review startup?

**A:** 
- **After installing new software** (check for new entries)
- **Monthly** (remove obsolete entries)
- **When boot slows down** (optimization needed)

### Q: Can I export/import between computers?

**A:** Yes! Export creates portable JSON:
- Works across different PCs
- Manually edit paths if needed
- Good for standardizing configurations

---

## Conclusion

Startup Master gives you complete, safe control over Windows startup. Use it to:

✅ Speed up boot times  
✅ Reduce system clutter  
✅ Optimize performance  
✅ Understand what's running  
✅ Maintain your system  

**Remember:**
- Backup before changes
- Disable before removing
- Test one change at a time
- Keep documentation

**Happy optimizing!** 🚀

---

*Complete Guide v1.0 | Startup Master | 2026-02-05*
