#nullable enable
using System.Windows;
using StartupMaster.Services;

namespace StartupMaster
{
    public partial class App : Application
    {
        private TrayIconService? _trayIconService;

        public static TrayIconService? TrayIcon { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ModernWpf.ThemeManager.Current.ApplicationTheme = ModernWpf.ApplicationTheme.Dark;

            // Initialize system tray icon
            _trayIconService = new TrayIconService();
            TrayIcon = _trayIconService;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _trayIconService?.Dispose();
            base.OnExit(e);
        }
    }
}
