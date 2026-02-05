# 🔨 Build Instructions - Startup Master

**Complete guide to building, testing, and deploying Startup Master**

---

## Prerequisites

### Required Software
- ✅ **Windows 10 (1809+) or Windows 11**
- ✅ **.NET 8.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- ✅ **PowerShell 5.1+** (included with Windows)

### Optional but Recommended
- 💡 **Visual Studio 2022** - Full IDE experience
- 💡 **JetBrains Rider** - Alternative IDE
- 💡 **VS Code** - Lightweight editor with C# extension

### Verify Prerequisites

```powershell
# Check .NET SDK
dotnet --version
# Should show: 8.0.x or higher

# Check PowerShell
$PSVersionTable.PSVersion
# Should show: 5.1 or higher

# Check Windows version
[Environment]::OSVersion
# Should show: Windows 10 or 11
```

---

## Quick Build (3 Steps)

### 1. Navigate to Project
```powershell
cd C:\Users\micha\.openclaw\workspace\StartupMaster
```

### 2. Build
```powershell
dotnet build -c Release
```

### 3. Run
```powershell
dotnet run
```

**Done!** The application should launch.

---

## Detailed Build Process

### Step 1: Restore Dependencies

```powershell
dotnet restore
```

**What this does:**
- Downloads NuGet packages
- Restores project references
- Verifies package compatibility

**Expected output:**
```
Determining projects to restore...
Restored C:\...\StartupMaster.csproj (in X sec).
```

---

### Step 2: Build Debug Version

```powershell
dotnet build
```

**Flags:**
- `-c Debug` (default) - Debug symbols included
- `-c Release` - Optimized for performance
- `-v detailed` - Verbose output for debugging

**Output location:**
```
bin\Debug\net8.0-windows\StartupMaster.dll
bin\Debug\net8.0-windows\StartupMaster.exe
```

**Expected output:**
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
Time Elapsed 00:00:XX
```

---

### Step 3: Build Release Version

```powershell
dotnet build -c Release
```

**Optimizations:**
- Code optimization enabled
- Debug symbols removed
- Smaller output size
- Faster execution

**Output location:**
```
bin\Release\net8.0-windows\StartupMaster.dll
bin\Release\net8.0-windows\StartupMaster.exe
```

---

### Step 4: Publish (Deployment-Ready)

```powershell
dotnet publish -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true
```

**Options explained:**
- `-c Release` - Release configuration
- `-r win-x64` - Target Windows 64-bit
- `--self-contained false` - Requires .NET Runtime (smaller)
- `/p:PublishSingleFile=true` - Single executable

**Output location:**
```
bin\Release\net8.0-windows\win-x64\publish\StartupMaster.exe
```

**Alternative (Self-Contained):**
```powershell
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```
- Includes .NET Runtime (~80 MB)
- Works without .NET installation
- Larger file size

---

## Build Configurations

### Debug Build
**Use for:** Development, testing, debugging

```powershell
dotnet build -c Debug
```

**Features:**
- Debug symbols included
- Optimizations disabled
- Larger binary size
- Easier debugging

### Release Build
**Use for:** Production, distribution

```powershell
dotnet build -c Release
```

**Features:**
- Optimizations enabled
- No debug symbols
- Smaller binary size
- Better performance

---

## Testing

### Run Unit Tests (if added)
```powershell
dotnet test
```

### Run Integration Test Suite
```powershell
.\Test-StartupMaster.ps1
```

**Test coverage:**
- Project structure validation
- Build artifact verification
- Documentation completeness
- .NET runtime check
- Functional launch test

---

## Cleaning Build Artifacts

### Clean All
```powershell
dotnet clean
```

**Removes:**
- bin/ folder
- obj/ folder
- Temporary build files

### Full Clean (Manual)
```powershell
Remove-Item bin, obj -Recurse -Force -ErrorAction SilentlyContinue
```

---

## Common Build Issues

### Issue: "SDK not found"
**Solution:**
```powershell
# Verify installation
dotnet --list-sdks

# If empty, download and install .NET 8.0 SDK
# https://dotnet.microsoft.com/download/dotnet/8.0
```

### Issue: "Package restore failed"
**Solution:**
```powershell
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore again
dotnet restore
```

### Issue: "Access denied" during build
**Solution:**
```powershell
# Close running instances of the app
taskkill /IM StartupMaster.exe /F

# Try build again
dotnet build
```

### Issue: "COM reference not supported"
**Solution:**
- Already fixed in current version
- Uses dynamic COM late binding instead

---

## IDE-Specific Instructions

### Visual Studio 2022

1. **Open Solution**
   - File → Open → Project/Solution
   - Select `StartupMaster.csproj`

2. **Set Configuration**
   - Toolbar: Debug → Release

3. **Build**
   - Build → Build Solution (Ctrl+Shift+B)

4. **Run**
   - Debug → Start Without Debugging (Ctrl+F5)

### JetBrains Rider

1. **Open Project**
   - File → Open → Select `StartupMaster.csproj`

2. **Build**
   - Build → Build Solution

3. **Run**
   - Run → Run 'StartupMaster'

### VS Code

1. **Open Folder**
   ```
   code C:\Users\micha\.openclaw\workspace\StartupMaster
   ```

2. **Install C# Extension**
   - Install "C# Dev Kit" from extensions

3. **Build**
   ```powershell
   dotnet build
   ```

4. **Run**
   ```powershell
   dotnet run
   ```

---

## Deployment

### Option 1: Published Folder
```powershell
dotnet publish -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true
```

**Distribute:**
- Copy `bin\Release\net8.0-windows\win-x64\publish\` folder
- User needs .NET 8.0 Runtime installed
- Smaller download size (~8 MB)

### Option 2: Self-Contained
```powershell
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true
```

**Distribute:**
- Single .exe file
- No .NET Runtime required
- Larger download size (~80 MB)

### Option 3: ZIP Distribution
```powershell
# Build
dotnet publish -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true

# Create ZIP
Compress-Archive -Path "bin\Release\net8.0-windows\win-x64\publish\*" -DestinationPath "StartupMaster-v1.0.zip"
```

---

## CI/CD Integration

### GitHub Actions (Example)

```yaml
name: Build Startup Master

on: [push, pull_request]

jobs:
  build:
    runs-on: windows-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Restore
      run: dotnet restore
    
    - name: Build
      run: dotnet build -c Release --no-restore
    
    - name: Test
      run: .\Test-StartupMaster.ps1
    
    - name: Publish
      run: dotnet publish -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true
    
    - name: Upload Artifact
      uses: actions/upload-artifact@v3
      with:
        name: StartupMaster
        path: bin/Release/net8.0-windows/win-x64/publish/
```

---

## Performance Optimization

### Build Performance
```powershell
# Parallel build
dotnet build -c Release /m

# Skip restore if already done
dotnet build -c Release --no-restore
```

### Output Size Optimization
```powershell
# Trim unused code
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishTrimmed=true

# Enable compression
dotnet publish -c Release -r win-x64 --self-contained true /p:EnableCompressionInSingleFile=true
```

---

## Versioning

### Update Version Number
Edit `StartupMaster.csproj`:

```xml
<PropertyGroup>
  <Version>1.0.0</Version>
  <AssemblyVersion>1.0.0.0</AssemblyVersion>
  <FileVersion>1.0.0.0</FileVersion>
</PropertyGroup>
```

### Build with Version
```powershell
dotnet build -c Release /p:Version=1.0.0
```

---

## Build Scripts

### Automated Build Script

Create `build.ps1`:

```powershell
#!/usr/bin/env pwsh

param(
    [ValidateSet('Debug','Release')]
    [string]$Configuration = 'Release',
    
    [switch]$Publish,
    [switch]$Clean
)

if ($Clean) {
    Write-Host "Cleaning..." -ForegroundColor Yellow
    dotnet clean
    Remove-Item bin, obj -Recurse -Force -ErrorAction SilentlyContinue
}

Write-Host "Building $Configuration..." -ForegroundColor Cyan
dotnet build -c $Configuration

if ($Publish) {
    Write-Host "Publishing..." -ForegroundColor Cyan
    dotnet publish -c $Configuration -r win-x64 --self-contained false /p:PublishSingleFile=true
}

Write-Host "Done!" -ForegroundColor Green
```

**Usage:**
```powershell
.\build.ps1 -Configuration Release -Publish -Clean
```

---

## Troubleshooting Build Errors

| Error | Solution |
|-------|----------|
| MSB3021: Unable to copy file | Close running instances, retry |
| NU1100: Unable to resolve package | Clear NuGet cache, restore |
| CS0103: Name does not exist | Check using directives, namespaces |
| NETSDK1045: Current .NET SDK does not support... | Update .NET SDK to 8.0+ |

---

## Post-Build Validation

### Checklist
- [ ] Build succeeded with 0 errors, 0 warnings
- [ ] Executable file created
- [ ] Application launches without errors
- [ ] Admin elevation prompt appears
- [ ] Main window displays correctly
- [ ] All startup items load
- [ ] Test suite passes

### Run Validation
```powershell
.\Test-StartupMaster.ps1 -Verbose
```

---

## Distribution Checklist

Before releasing:
- [ ] Build in Release mode
- [ ] Run test suite
- [ ] Update CHANGELOG.md
- [ ] Update version number
- [ ] Test on clean system
- [ ] Verify admin elevation
- [ ] Check all docs are current
- [ ] Create ZIP/installer
- [ ] Test installation process

---

**Ready to build!** Follow these instructions for consistent, reliable builds every time.

---

*Last Updated: 2026-02-05*  
*Build System: dotnet CLI*  
*Target Framework: .NET 8.0*
