# 👨‍💻 Startup Master - Developer Documentation

## Project Structure

```
StartupMaster/
├── Models/
│   └── StartupItem.cs          # Data model for startup items
├── Services/
│   ├── RegistryStartupManager.cs    # Registry operations
│   ├── StartupFolderManager.cs      # Startup folder operations
│   ├── TaskSchedulerManager.cs      # Task Scheduler integration
│   ├── ServicesManager.cs           # Windows Services management
│   └── StartupManager.cs            # Unified manager facade
├── Views/
│   ├── AddEditDialog.xaml           # Add/Edit dialog UI
│   └── AddEditDialog.xaml.cs        # Add/Edit dialog logic
├── Converters/
│   └── BoolToVisibilityConverter.cs # UI value converters
├── App.xaml                    # Application resources
├── App.xaml.cs                 # Application entry point
├── MainWindow.xaml             # Main window UI
├── MainWindow.xaml.cs          # Main window logic
├── StartupMaster.csproj        # Project file
└── app.manifest                # Admin elevation manifest
```

## Architecture

### Design Pattern: Service Layer + MVVM-Light

The application uses a layered architecture:

1. **Model Layer** (`Models/`)
   - `StartupItem`: Core data model
   - Implements `INotifyPropertyChanged` for UI binding

2. **Service Layer** (`Services/`)
   - Each manager handles one startup location
   - `StartupManager`: Facade pattern to unify all managers
   - Encapsulates Windows API calls

3. **View Layer** (`Views/`, `MainWindow.xaml`)
   - WPF/XAML for UI definition
   - Code-behind for event handling
   - Minimal business logic in views

### Key Classes

#### StartupItem (Model)
```csharp
public class StartupItem : INotifyPropertyChanged
{
    // Core properties
    string Name
    string Command
    string Arguments
    StartupLocation Location
    bool IsEnabled
    int DelaySeconds
    
    // Location-specific properties
    string RegistryKey
    string RegistryValueName
    string FilePath
    string TaskName
    string ServiceName
}
```

#### StartupManager (Facade)
```csharp
public class StartupManager
{
    List<StartupItem> GetAllItems()
    bool AddItem(StartupItem item)
    bool RemoveItem(StartupItem item)
    bool DisableItem(StartupItem item)
    bool EnableItem(StartupItem item)
    bool UpdateDelay(StartupItem item)
}
```

## Technology Stack

### Core Framework
- **.NET 8.0**: Modern .NET for Windows Desktop
- **WPF**: Windows Presentation Foundation for UI
- **XAML**: Markup for declarative UI

### NuGet Packages
```xml
<PackageReference Include="TaskScheduler" Version="2.10.1" />
<PackageReference Include="ModernWpfUI" Version="0.9.6" />
<PackageReference Include="System.ServiceProcess.ServiceController" Version="9.0.0" />
<PackageReference Include="System.Management" Version="9.0.0" />
```

### Windows APIs
- **Registry API**: Microsoft.Win32.Registry
- **Task Scheduler**: Microsoft.Win32.TaskScheduler
- **Services**: System.ServiceProcess, System.Management
- **COM Interop**: WScript.Shell (for shortcuts)

## Building & Development

### Prerequisites
- Visual Studio 2022 or Rider
- .NET 8.0 SDK
- Windows 10/11

### Build Commands

```bash
# Restore dependencies
dotnet restore

# Debug build
dotnet build

# Release build
dotnet build -c Release

# Publish (single-file)
dotnet publish -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true

# Run
dotnet run
```

### Project Configuration

**Target Framework**: `net8.0-windows`  
**Output Type**: `WinExe` (Windows GUI)  
**Requires Admin**: Yes (via manifest)

## Key Implementation Details

### Registry Management

**Locations Scanned**:
- `HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run`
- `HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce`
- `HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Run`
- `HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce`
- `HKLM\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Run`

**Disable Mechanism**:
- Move value from `\Run` to `\Run-Disabled`
- Enable reverses the operation

**Add/Remove**:
- Direct Registry API calls
- Value format: `"C:\Path\To\App.exe" args`

### Startup Folder Management

**Locations**:
- User: `%AppData%\Microsoft\Windows\Start Menu\Programs\Startup`
- Common: `%ProgramData%\Microsoft\Windows\Start Menu\Programs\StartUp`

**Shortcut Creation**:
```csharp
var shell = Type.GetTypeFromProgID("WScript.Shell");
dynamic wsh = Activator.CreateInstance(shell);
var shortcut = wsh.CreateShortcut(path);
shortcut.TargetPath = target;
shortcut.Arguments = args;
shortcut.Save();
```

**Disable Mechanism**:
- Rename `.lnk` to `.lnk.disabled`
- Windows ignores `.disabled` files

### Task Scheduler Integration

**Task Creation**:
```csharp
var ts = new TaskService();
var td = ts.NewTask();

// Add logon trigger with delay
var trigger = new LogonTrigger();
trigger.Delay = TimeSpan.FromSeconds(delay);
td.Triggers.Add(trigger);

// Add action
td.Actions.Add(new ExecAction(command, arguments));

// Register
ts.RootFolder.RegisterTaskDefinition(name, td);
```

**Filter Logic**:
- Only shows tasks with `BootTrigger` or `LogonTrigger`
- Ignores scheduled tasks, event-triggered tasks, etc.

### Services Management

**WMI for Startup Type**:
```csharp
using var sc = new System.Management.ManagementObject(
    $"Win32_Service.Name='{serviceName}'");
sc.Get();
sc.InvokeMethod("ChangeStartMode", new object[] { "Automatic" });
```

**Service Control**:
```csharp
var service = new ServiceController(serviceName);
service.Start();
service.WaitForStatus(ServiceControllerStatus.Running);
```

## UI Implementation

### Data Binding
```xml
<DataGrid ItemsSource="{Binding FilteredItems}">
  <DataGridTextColumn Binding="{Binding Name}" />
  <DataGridTextColumn Binding="{Binding StatusDisplay}" />
</DataGrid>
```

### Value Converters
- `BoolToVisibilityConverter`: Hide enable button if disabled
- `InverseBoolToVisibilityConverter`: Hide disable button if enabled

### Styling
- ModernWPF theme system
- Dark mode default
- Windows 11-style controls

## Testing Approach

### Manual Testing Checklist
- [ ] Add item to each location
- [ ] Enable/disable each location type
- [ ] Remove items from each location
- [ ] Search functionality
- [ ] Filter by location
- [ ] Filter by status
- [ ] Export configuration
- [ ] Import configuration
- [ ] Edit delay (Task Scheduler)
- [ ] Admin elevation prompt

### Edge Cases
- Empty startup lists
- Long command paths (>260 chars)
- Special characters in names
- Paths with spaces
- Missing executables
- Registry permission errors
- Task Scheduler errors
- Service access denied

## Extension Points

### Adding a New Startup Location

1. **Create Manager Class**:
   ```csharp
   public class NewLocationManager
   {
       public List<StartupItem> GetItems() { }
       public bool AddItem(StartupItem item) { }
       public bool RemoveItem(StartupItem item) { }
       public bool DisableItem(StartupItem item) { }
       public bool EnableItem(StartupItem item) { }
   }
   ```

2. **Update StartupLocation Enum**:
   ```csharp
   public enum StartupLocation
   {
       // ... existing ...
       NewLocation
   }
   ```

3. **Integrate in StartupManager**:
   ```csharp
   private readonly NewLocationManager _newManager = new();
   
   public List<StartupItem> GetAllItems()
   {
       items.AddRange(_newManager.GetItems());
   }
   ```

4. **Add UI Filter**:
   ```xml
   <ComboBoxItem Content="New Location" />
   ```

### Adding UI Features

**New Actions**:
1. Add button/menu item in XAML
2. Add event handler in MainWindow.xaml.cs
3. Call appropriate service method

**New Dialogs**:
1. Create XAML + code-behind in `Views/`
2. Show with `ShowDialog()`
3. Return data via properties

## Performance Considerations

### Startup Item Discovery
- **Registry**: Fast (< 50ms for all keys)
- **Startup Folders**: Fast (< 10ms)
- **Task Scheduler**: Moderate (~500ms for full scan)
- **Services**: Slow (~2-3 seconds for all services)

### Optimization Techniques
- Parallel loading could speed up initial scan
- Cache service list (expensive operation)
- Lazy-load Task Scheduler details

### UI Responsiveness
- All operations are synchronous currently
- Consider `async/await` for slow operations
- Progress indicator for long operations

## Security Considerations

### Elevation
- Manifest requests `requireAdministrator`
- User-level operations work without admin
- System-level operations require elevation

### Input Validation
- Path validation for executables
- Registry key/value name sanitization
- Command-line argument parsing

### Error Handling
- Try-catch around all Windows API calls
- Silent failure for non-critical operations
- MessageBox for critical errors

## Known Limitations

1. **No UAC Bypass**: Requires admin for system changes
2. **No Undo**: Operations are immediate and permanent (except disable)
3. **No Conflict Detection**: Doesn't detect duplicate entries
4. **Limited Task Scheduler**: Only shows logon/boot tasks
5. **Service Read-Only**: Can't create new services
6. **No GPO Support**: Can't modify Group Policy startup items

## Future Development Ideas

### High Priority
- Async operations for responsiveness
- Undo/redo system
- Backup before major changes
- Startup time analyzer

### Medium Priority
- Duplicate detection
- Conflict resolution
- Command-line interface
- Batch operations

### Low Priority
- Profiles (work/gaming/minimal)
- Cloud backup
- Change history
- Telemetry/analytics

## Debugging Tips

### Common Issues

**"Access Denied"**
- Run as Administrator
- Check UAC settings
- Verify file permissions

**"Task Not Found"**
- Refresh task list
- Check Task Scheduler service status
- Verify task exists in taskschd.msc

**"Service Doesn't Start"**
- Check service dependencies
- Verify service executable exists
- Check Windows Event Log

### Logging
Add logging to diagnose issues:
```csharp
try
{
    // operation
}
catch (Exception ex)
{
    File.AppendAllText("debug.log", 
        $"{DateTime.Now}: {ex.Message}\n{ex.StackTrace}\n");
}
```

## Contributing Guidelines

1. **Code Style**: Follow C# conventions
2. **Comments**: Document complex logic
3. **Error Handling**: Always try-catch Windows API calls
4. **Testing**: Manual test all changes
5. **Documentation**: Update relevant .md files

---

**Version**: 1.0  
**Last Updated**: 2026-02-05  
**Maintainer**: Till Thelet  
**Built With**: OpenClaw AI + Marathon Mode
