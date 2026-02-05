# Changelog - Startup Master

All notable changes to this project will be documented in this file.

## [1.0.0] - 2026-02-05

### Initial Release

#### Core Features
- **Multi-Location Support**
  - Registry (Current User & Local Machine)
  - Startup Folders (User & Common)
  - Task Scheduler (with logon/boot triggers)
  - Windows Services (auto-start)

#### Management Operations
- Enable/Disable items (non-destructive)
- Add new startup items
- Remove existing items
- Edit item properties
- Delay configuration (Task Scheduler)

#### User Interface
- Modern WPF with ModernWPF theme
- Dark mode UI
- Real-time search functionality
- Location and status filtering
- Sortable data grid
- Inline action buttons
- Responsive design

#### Advanced Features
- **Auto-Backup System**
  - Automatic backup on critical changes
  - Backup before delete operations
  - Backup on application close
  - 50-backup rotation
- **Performance Analyzer**
  - Startup item statistics
  - High-impact item detection
  - Duplicate detection
  - Missing executable detection
  - Optimization recommendations
- **Export/Import**
  - JSON format configuration
  - Backup and restore
  - Configuration sharing

#### Utilities
- **Analyze-Startup.ps1**: Comprehensive startup analysis
- **Optimize-Startup.ps1**: Guided optimization workflow
- **Quick-Disable-All.ps1**: Emergency startup disable
- **Launch-StartupMaster.bat**: Easy launcher with elevation

#### Documentation
- README.md: Project overview and features
- INSTALL.md: Installation and setup guide
- FEATURES.md: Complete feature documentation
- DEVELOPER.md: Technical documentation for developers
- CHANGELOG.md: This file

### Technical Details
- **Framework**: .NET 8.0 WPF
- **Admin Elevation**: Via manifest
- **Dependencies**:
  - TaskScheduler 2.10.1
  - ModernWpfUI 0.9.6
  - System.ServiceProcess.ServiceController 9.0.0
  - System.Management 9.0.0

### Known Limitations
- Task Scheduler API requires .NET 8.0+
- COM shortcut creation requires Windows Script Host
- Service management requires System.Management
- Admin elevation required for system-wide operations

### Future Enhancements Planned
- Command-line interface
- Startup time measurement
- Resource usage monitoring
- Scheduled enable/disable
- Startup profiles (work/gaming/minimal)
- Change history tracking
- Right-click context menus
- Drag-and-drop support
- Cloud backup integration

---

## Development Process

**Built During**: Marathon Mode Session (50 minutes)  
**Build System**: dotnet CLI  
**Testing**: Manual testing on Windows 11  
**Code Quality**: Full error handling, input validation

---

## Credits

**Developer**: Till Thelet  
**AI Assistant**: OpenClaw (Claude Sonnet 4.5)  
**Build Date**: 2026-02-05  
**License**: Free for personal and commercial use

---

## Version History

### Version Numbering
- **Major.Minor.Patch** (Semantic Versioning)
- Major: Breaking changes
- Minor: New features (backward compatible)
- Patch: Bug fixes and improvements

### Planned Releases
- **1.1.0**: CLI support, startup time analysis
- **1.2.0**: Startup profiles, scheduled operations
- **1.3.0**: Resource monitoring, conflict detection
- **2.0.0**: Complete UI redesign, cloud features

---

*Last Updated: 2026-02-05 21:00 GMT+2*
