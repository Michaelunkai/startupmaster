# ⚡ Quick Start Guide - Startup Master

**Get up and running in 60 seconds!**

## 1. Launch the Application

### Option A: Double-Click
Navigate to:
```
StartupMaster\bin\Release\net8.0-windows\win-x64\publish\
```
Double-click `StartupMaster.exe`

### Option B: Use Launcher
Double-click `Launch-StartupMaster.bat` in the project root

### Option C: Command Line
```bash
cd C:\Users\micha\.openclaw\workspace\StartupMaster
dotnet run
```

**Important**: Click "Yes" when asked for administrator privileges!

---

## 2. First Look

When the app opens, you'll see:

**Top Toolbar:**
- 🔄 Refresh - Re-scan all startup locations
- ➕ Add New - Create a new startup item
- 💾 Export - Backup your configuration
- 📂 Import - Restore from backup

**Main Grid:**
- All your startup items in one place
- Status (✓ Enabled / ✗ Disabled)
- Name, Command, Location, Delay

**Action Buttons (per item):**
- ✓ Enable
- ✗ Disable
- ✏️ Edit
- 🗑️ Remove

---

## 3. Common Tasks

### Disable a Startup Item
1. Find the item in the grid
2. Click the **✗** button
3. Done! Item disabled (but not removed)

### Remove a Startup Item
1. Find the item
2. Click the **🗑️** button
3. Confirm the deletion
4. Auto-backup created ✓

### Add a New Startup Item
1. Click **➕ Add New**
2. Fill in:
   - **Name**: `My App`
   - **Command**: Browse to .exe or enter path
   - **Arguments**: (optional)
   - **Location**: Choose where to register it
   - **Delay**: (Task Scheduler only) 0-300 seconds
3. Click **Save**

### Search for an Item
- Type in the search box
- Results filter instantly

### Filter by Location
- Use the dropdown next to search
- Options: All, Enabled, Disabled, Registry, Folders, Tasks, Services

---

## 4. Optimize Your Startup

### Quick Optimization (Recommended)

**Run the optimizer:**
```powershell
.\Optimize-Startup.ps1
```

Follow the on-screen guidance!

### Manual Optimization

1. **Identify non-essential items**
   - Look for apps you rarely use
   - Check for duplicate entries

2. **Disable instead of remove**
   - Click ✗ to disable
   - Items stay available if needed

3. **Add delays to heavy apps**
   - Edit the item (✏️ button)
   - Choose "Task Scheduler" location
   - Set delay to 30-60 seconds

4. **Export a backup**
   - Click 💾 Export
   - Save the .json file somewhere safe

---

## 5. Emergency Disable

**Computer booting slowly?**

Run the emergency script:
```powershell
.\Quick-Disable-All.ps1
```

This disables **ALL** user startup items immediately.

**To restore:**
- Use the backup created by the script
- Or selectively re-enable items in Startup Master

---

## 6. Analyze Your Startup

**Get insights:**
```powershell
.\Analyze-Startup.ps1 -Detailed
```

Shows:
- Total startup items
- Breakdown by location
- Recommendations

**Export a report:**
```powershell
.\Analyze-Startup.ps1 -Detailed -ExportReport
```

---

## 7. Best Practices

### ✅ DO:
- **Backup first** - Export before making changes
- **Disable, don't remove** - Easier to undo
- **Use delays** - Spread out the boot load
- **Review regularly** - Apps add themselves to startup

### ❌ DON'T:
- Remove items you don't recognize (Google them first!)
- Disable security software
- Remove system components
- Forget to restart after changes

---

## 8. Troubleshooting

### App Won't Start
- Check .NET 8.0 is installed
- Run as Administrator
- See `INSTALL.md` for details

### Changes Not Working
- Click 🔄 Refresh
- Restart Windows
- Check you have admin rights

### Item Won't Delete
- Try disabling first
- Check if it's a system-protected item
- Run app as Administrator

---

## 9. Keyboard Shortcuts

- **Ctrl+R** - Refresh (planned)
- **Ctrl+N** - Add New (planned)
- **Ctrl+F** - Search (planned)
- **Ctrl+E** - Export (planned)

---

## 10. Next Steps

**Optimize:**
1. Run `Analyze-Startup.ps1`
2. Review recommendations
3. Disable non-essential items
4. Add delays to heavy apps

**Backup:**
1. Export configuration (💾)
2. Save to cloud/external drive
3. Test restoration

**Learn More:**
- `README.md` - Full overview
- `FEATURES.md` - Complete feature list
- `INSTALL.md` - Installation guide
- `DEVELOPER.md` - Technical docs

---

## 🎯 30-Second Checklist

- [ ] Launch app (admin required)
- [ ] Export backup first
- [ ] Disable non-essential items
- [ ] Add delays to heavy apps
- [ ] Export optimized config
- [ ] Restart and test

---

**Need Help?**
- Check documentation
- Run test suite: `.\Test-StartupMaster.ps1`
- Review source code (fully commented)

**Enjoy faster boot times! 🚀**

---

*Version 1.0 | 2026-02-05 | Built with OpenClaw*
