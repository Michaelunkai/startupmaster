# 🚀 Startup Master - Ultimate Windows Startup Manager

**The most comprehensive Windows startup management tool with full control over everything that runs at boot.**

## ✨ Features

### Complete Startup Coverage
- **Registry (Current User)** - User-specific startup items
- **Registry (Local Machine)** - System-wide startup items (requires admin)
- **Startup Folders** - Both user and common startup folders
- **Task Scheduler** - Scheduled tasks that run at logon/boot with delay support
- **Windows Services** - View and manage auto-start services

### Powerful Management
- ✅ Enable/Disable items without removing them
- ❌ Remove items completely
- ➕ Add new startup items with full configuration
- ✏️ Edit existing items
- ⏱️ Set startup delays (Task Scheduler items)
- 📊 View all startup locations in one place

### Advanced Features
- 🔍 **Real-time Search** - Filter by name or command
- 🎯 **Smart Filtering** - Filter by location, status, or type
- 💾 **Export/Import** - Backup and restore your startup configuration
- 🎨 **Modern Dark UI** - Built with ModernWPF for a sleek Windows 11-style interface
- 🔐 **Admin Elevation** - Automatically requests admin rights for system-wide changes

## 📋 Requirements

- Windows 10/11
- .NET 8.0 Runtime or higher
- Administrator privileges (for system-wide changes)

## 🚀 Quick Start

### Running the Application

1. **Navigate to the publish folder:**
   ```
   C:\Users\micha\.openclaw\workspace\StartupMaster\bin\Release\net8.0-windows\win-x64\publish\
   ```

2. **Run `StartupMaster.exe`**

3. The application will request administrator elevation - click "Yes" to grant full access

### First Steps

1. **View All Startup Items**
   - The main window shows all startup items from all locations
   - Items are color-coded: Enabled (✓) or Disabled (✗)

2. **Manage Items**
   - Click the ✓ button to disable an enabled item
   - Click the ✗ button to re-enable a disabled item
   - Click the ✏️ button to edit item properties
   - Click the 🗑️ button to remove an item

3. **Add New Items**
   - Click "➕ Add New" in the toolbar
   - Enter the program name, path, and optional arguments
   - Choose where to register the startup item
   - Set an optional delay (Task Scheduler only)

4. **Search & Filter**
   - Use the search box to find specific items
   - Use the filter dropdown to view specific categories

## 📖 Detailed Usage

### Adding a Startup Item

1. Click **"➕ Add New"**
2. Fill in:
   - **Name**: Display name for the item
   - **Command/Path**: Full path to the executable
   - **Arguments**: Optional command-line arguments
   - **Location**: Choose where to register:
     - **Registry (Current User)**: Runs only for your account
     - **Registry (Local Machine)**: Runs for all users (requires admin)
     - **Startup Folder**: Creates a shortcut in the startup folder
     - **Task Scheduler**: Advanced option with delay support
   - **Delay**: (Task Scheduler only) Delay in seconds before starting

3. Click **Save**

### Startup Delay Configuration

For Task Scheduler items, you can configure a delay:
- **0 seconds**: Start immediately at logon
- **10-300 seconds**: Delay the start to reduce boot load

This is useful for:
- Non-critical applications
- Resource-intensive programs
- Preventing boot slowdown

### Export Configuration

1. Click **"💾 Export"**
2. Choose a location to save the JSON file
3. File contains all startup items with full configuration

### Import Configuration

1. Click **"📂 Import"**
2. Select a previously exported JSON file
3. Confirm the import
4. All items from the file will be added

## 🎯 Use Cases

### Performance Optimization
- Identify resource-heavy startup items
- Disable non-essential programs
- Add startup delays to spread the boot load

### System Cleanup
- Find and remove unwanted startup items
- Disable bloatware that starts automatically
- Clean up old startup entries

### Configuration Backup
- Export your startup configuration before changes
- Restore startup items after Windows reinstall
- Share startup configurations across machines

### Development & Testing
- Quickly add/remove debug tools from startup
- Test application startup behavior
- Manage multiple development environments

## 🛡️ Safety Features

- **Backup Before Changes**: Always export before major changes
- **Non-Destructive Disable**: Disabling items doesn't delete them
- **Confirmation Dialogs**: Important actions require confirmation
- **Admin Protection**: System-wide changes require elevation

## 🔧 Technical Details

### Technology Stack
- **Framework**: .NET 8.0 WPF
- **UI**: ModernWPF (Windows 11-style dark theme)
- **Task Scheduler**: Microsoft Task Scheduler Library
- **Registry**: Native Windows Registry API
- **Services**: System.ServiceProcess API

### Supported Startup Locations

| Location | Path | Scope | Requires Admin |
|----------|------|-------|----------------|
| Registry HKCU\Run | `HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run` | User | No |
| Registry HKLM\Run | `HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Run` | System | Yes |
| Startup Folder (User) | `%AppData%\Microsoft\Windows\Start Menu\Programs\Startup` | User | No |
| Startup Folder (Common) | `%ProgramData%\Microsoft\Windows\Start Menu\Programs\StartUp` | System | Yes |
| Task Scheduler | Windows Task Scheduler with Logon/Boot triggers | System | Yes |
| Services | Windows Services with Automatic startup | System | Yes |

### File Format (Export/Import)

JSON format containing:
```json
[
  {
    "Name": "Example App",
    "Command": "C:\\Program Files\\Example\\app.exe",
    "Arguments": "--minimized",
    "Location": "RegistryCurrentUser",
    "IsEnabled": true,
    "DelaySeconds": 0
  }
]
```

## 🎨 Screenshots

### Main Window
- Clean, modern interface
- All startup items in one view
- Easy-to-use toolbar
- Real-time search and filtering

### Add/Edit Dialog
- Intuitive form layout
- File browser for easy path selection
- Location selector with descriptions
- Delay slider for Task Scheduler items

## 🐛 Troubleshooting

### Application Won't Start
- Ensure .NET 8.0 Runtime is installed
- Run as Administrator
- Check Windows Event Viewer for errors

### Can't Modify System Items
- Ensure you clicked "Yes" on the UAC prompt
- Some items may be protected by Group Policy
- Try running as Administrator explicitly

### Changes Not Taking Effect
- Restart the application to see updated items
- Some services may require a system restart
- Task Scheduler items may need Windows restart

## 📝 License

This project is provided as-is for personal and commercial use.

## 🤝 Contributing

Built with OpenClaw AI assistance. For improvements or bug reports, please document and share.

## 📧 Support

For issues or questions, refer to the source code and documentation provided.

---

**Built with ❤️ using OpenClaw Marathon Mode**  
*Compiled: 2026-02-05*

<!-- gitit-sync: 2026-02-05 22:00:36.916946 -->

<!-- gitit-sync: 2026-02-05 22:03:26.987265 -->

<!-- gitit-sync: 2026-02-05 22:07:17.227471 -->

<!-- gitit-sync: 2026-02-05 22:14:32.263542 -->

<!-- gitit-sync: 2026-02-05 22:18:35.571124 -->
