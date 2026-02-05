# 📚 Startup Master - Documentation Index

**Your complete guide to Startup Master - find what you need instantly!**

---

## 🚀 Quick Navigation

### Getting Started (Choose Your Path)

**Brand New User?**
1. Start here: [QUICKSTART.md](QUICKSTART.md) - 60-second guide
2. Then read: [README.md](README.md) - Full overview
3. If stuck: [INSTALL.md](INSTALL.md) - Installation help

**Experienced User?**
1. Jump to: [FEATURES.md](FEATURES.md) - Complete feature list
2. Check out: [Utility Scripts](#utility-scripts) - Powerful automation

**Developer?**
1. Technical docs: [DEVELOPER.md](DEVELOPER.md)
2. Build guide: [BUILD_INSTRUCTIONS.md](BUILD_INSTRUCTIONS.md)
3. Source code: Browse `Models/`, `Services/`, `Views/`

---

## 📖 Core Documentation

### [README.md](README.md)
**The main overview** - Start here for project introduction

**Contents:**
- ✨ Features overview
- 📋 Requirements
- 🚀 Quick start
- 📖 Detailed usage
- 🎯 Use cases
- 🛡️ Safety features
- 🔧 Technical details

**Best for:** First-time users, general overview

---

### [QUICKSTART.md](QUICKSTART.md)
**60-second guide** - Get productive immediately

**Contents:**
- Launch instructions (3 methods)
- First look walkthrough
- Common tasks (4 examples)
- Quick optimization guide
- Emergency procedures
- 30-second checklist

**Best for:** Impatient users, quick reference

---

### [INSTALL.md](INSTALL.md)
**Complete installation guide** - Detailed setup instructions

**Contents:**
- System requirements
- Installation methods (3 options)
- .NET Runtime installation
- First launch setup
- Quick start tutorial
- Troubleshooting
- Uninstallation

**Best for:** Installation problems, new setups

---

### [FEATURES.md](FEATURES.md)
**Complete feature list** - Everything the app can do

**Contents:**
- Core features breakdown
- 5 startup locations explained
- Management operations
- Discovery & filtering
- Backup & restore
- UI features
- Advanced capabilities
- Export/import format

**Best for:** Feature discovery, capabilities reference

---

### [DEVELOPER.md](DEVELOPER.md)
**Technical documentation** - For developers and power users

**Contents:**
- Project structure
- Architecture patterns
- Technology stack
- Implementation details
- Building & development
- Extension points
- Performance considerations
- Security notes

**Best for:** Developers, contributors, advanced users

---

### [BUILD_INSTRUCTIONS.md](BUILD_INSTRUCTIONS.md)
**Build guide** - Compile from source

**Contents:**
- Prerequisites
- Quick build (3 steps)
- Detailed build process
- Build configurations
- Testing procedures
- IDE instructions
- Deployment options
- CI/CD integration

**Best for:** Developers, custom builds

---

### [CHANGELOG.md](CHANGELOG.md)
**Version history** - What's changed

**Contents:**
- Release notes (v1.0.0)
- Feature list
- Technical details
- Known limitations
- Future roadmap
- Development process

**Best for:** Version tracking, what's new

---

### [PROJECT_SUMMARY.md](PROJECT_SUMMARY.md)
**Project overview** - High-level summary

**Contents:**
- Project statistics
- Key achievements
- Technical specifications
- Use cases
- Innovation highlights
- Future roadmap
- Metrics & credits

**Best for:** Quick overview, presentations

---

## 🛠️ Utility Scripts

### [Analyze-Startup.ps1](Analyze-Startup.ps1)
**Comprehensive startup analysis**

```powershell
.\Analyze-Startup.ps1 -Detailed -ExportReport
```

**Features:**
- Scans all startup locations
- Counts items by type
- Performance rating
- Recommendations
- JSON export

**Use when:** Diagnosing startup, creating reports

---

### [Optimize-Startup.ps1](Optimize-Startup.ps1)
**Guided optimization workflow**

```powershell
.\Optimize-Startup.ps1
```

**Features:**
- Analyzes current configuration
- Performance rating
- Tailored recommendations
- Launches Startup Master

**Use when:** Optimizing startup, first-time setup

---

### [Quick-Disable-All.ps1](Quick-Disable-All.ps1)
**Emergency startup disable**

```powershell
.\Quick-Disable-All.ps1
```

**Features:**
- Disables ALL user startup items
- Creates automatic backup
- Registry + folders + tasks
- Reversible

**Use when:** Emergency boot slowdown, troubleshooting

---

### [Test-StartupMaster.ps1](Test-StartupMaster.ps1)
**Comprehensive test suite**

```powershell
.\Test-StartupMaster.ps1 -Verbose
```

**Features:**
- 13+ test cases
- Build validation
- Documentation checks
- Environment verification
- Launch test

**Use when:** After building, before distributing

---

### [Launch-StartupMaster.bat](Launch-StartupMaster.bat)
**Easy launcher with elevation**

```cmd
Launch-StartupMaster.bat
```

**Features:**
- Checks admin privileges
- Auto-elevates if needed
- Launches from anywhere
- Error handling

**Use when:** Quick launch, creating shortcuts

---

## 🗂️ Source Code Reference

### Models/
**Data structures**

| File | Purpose |
|------|---------|
| `StartupItem.cs` | Core data model with INotifyPropertyChanged |

---

### Services/
**Business logic layer**

| File | Purpose |
|------|---------|
| `RegistryStartupManager.cs` | Registry (HKCU & HKLM) operations |
| `StartupFolderManager.cs` | Startup folder management |
| `TaskSchedulerManager.cs` | Windows Task Scheduler integration |
| `ServicesManager.cs` | Windows Services control |
| `StartupManager.cs` | Unified facade for all managers |

---

### Utils/
**Utility classes**

| File | Purpose |
|------|---------|
| `BackupManager.cs` | Auto-backup system with rotation |
| `PerformanceAnalyzer.cs` | Startup analysis and recommendations |

---

### Views/
**User interface**

| File | Purpose |
|------|---------|
| `AddEditDialog.xaml` | Add/Edit item UI |
| `AddEditDialog.xaml.cs` | Add/Edit logic |
| `MainWindow.xaml` | Main window UI |
| `MainWindow.xaml.cs` | Main window logic |

---

### Converters/
**UI helpers**

| File | Purpose |
|------|---------|
| `BoolToVisibilityConverter.cs` | Boolean to visibility conversion |

---

## 🎯 Find By Topic

### Installation & Setup
- [System requirements](INSTALL.md#system-requirements)
- [.NET installation](INSTALL.md#net-runtime-installation)
- [First launch](INSTALL.md#first-launch-setup)
- [Quick start](QUICKSTART.md)

### Using the Application
- [Add startup item](QUICKSTART.md#add-a-new-startup-item)
- [Disable item](QUICKSTART.md#disable-a-startup-item)
- [Remove item](QUICKSTART.md#remove-a-startup-item)
- [Export backup](QUICKSTART.md#common-tasks)
- [Search items](README.md#search--filter)

### Optimization
- [Performance tips](README.md#startup-delay-configuration)
- [Best practices](QUICKSTART.md#best-practices)
- [Optimization script](Optimize-Startup.ps1)
- [Analysis tool](Analyze-Startup.ps1)

### Troubleshooting
- [Common issues](INSTALL.md#troubleshooting)
- [Build errors](BUILD_INSTRUCTIONS.md#common-build-issues)
- [Test suite](Test-StartupMaster.ps1)

### Development
- [Architecture](DEVELOPER.md#architecture)
- [Building](BUILD_INSTRUCTIONS.md)
- [Extending](DEVELOPER.md#extension-points)
- [Contributing](DEVELOPER.md#contributing-guidelines)

---

## 📊 Documentation Stats

| Metric | Value |
|--------|-------|
| Total Docs | 9 files |
| Total Words | ~35,000+ |
| Code Examples | 100+ |
| Scripts | 5 |
| Total Pages (printed) | ~120 |

---

## 🔍 Quick Reference

### File Paths

**Executable:**
```
bin\Release\net8.0-windows\win-x64\publish\StartupMaster.exe
```

**Documentation:**
```
C:\Users\micha\.openclaw\workspace\StartupMaster\
├── README.md
├── QUICKSTART.md
├── INSTALL.md
├── FEATURES.md
├── DEVELOPER.md
├── BUILD_INSTRUCTIONS.md
├── CHANGELOG.md
├── PROJECT_SUMMARY.md
└── INDEX.md (this file)
```

**Utilities:**
```
C:\Users\micha\.openclaw\workspace\StartupMaster\
├── Analyze-Startup.ps1
├── Optimize-Startup.ps1
├── Quick-Disable-All.ps1
├── Test-StartupMaster.ps1
└── Launch-StartupMaster.bat
```

---

## 💡 Recommended Reading Order

### For Users
1. QUICKSTART.md
2. README.md
3. FEATURES.md
4. INSTALL.md (if problems)

### For Developers
1. PROJECT_SUMMARY.md
2. DEVELOPER.md
3. BUILD_INSTRUCTIONS.md
4. Source code exploration

### For Distributors
1. README.md
2. INSTALL.md
3. BUILD_INSTRUCTIONS.md
4. Test-StartupMaster.ps1

---

## 🆘 Still Need Help?

### Check These Resources
1. **Search this index** - Use Ctrl+F
2. **Read QUICKSTART.md** - Solves 80% of questions
3. **Browse FEATURES.md** - Find capabilities
4. **Check INSTALL.md** - Installation issues
5. **Review source code** - Fully commented

### Common Questions → Answers
- **How do I install?** → [INSTALL.md](INSTALL.md)
- **How do I use it?** → [QUICKSTART.md](QUICKSTART.md)
- **What can it do?** → [FEATURES.md](FEATURES.md)
- **How do I build it?** → [BUILD_INSTRUCTIONS.md](BUILD_INSTRUCTIONS.md)
- **What's new?** → [CHANGELOG.md](CHANGELOG.md)
- **How does it work?** → [DEVELOPER.md](DEVELOPER.md)

---

## 📝 Document Maintenance

**Last Updated:** 2026-02-05  
**Version:** 1.0  
**Maintained By:** Till Thelet  

**Update Frequency:**
- With each release
- When new features added
- When significant bugs fixed
- When user feedback requires clarification

---

**Navigate confidently!** This index has everything you need. 🧭

---

*Documentation Index v1.0 | Startup Master Project*
