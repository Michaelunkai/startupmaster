#nullable enable
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Win32;
using Application = System.Windows.Application;

namespace StartupMaster.Services
{
    public class TrayIconService : IDisposable
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly ToolStripMenuItem _startupMenuItem;
        private readonly string _appPath;
        private readonly string _appName = "StartupMaster";
        private readonly string _registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private bool _disposed;

        public TrayIconService()
        {
            _appPath = Environment.ProcessPath ?? AppContext.BaseDirectory;
            
            _notifyIcon = new NotifyIcon
            {
                Visible = true,
                Text = "Startup Master - Windows Startup Manager"
            };

            // Load icon from embedded resource
            LoadIcon();

            // Create context menu
            var contextMenu = new ContextMenuStrip();

            // Show/Hide window
            var showMenuItem = new ToolStripMenuItem("Show Window", null, (s, e) => ShowMainWindow());
            showMenuItem.Font = new Font(showMenuItem.Font, System.Drawing.FontStyle.Bold);
            contextMenu.Items.Add(showMenuItem);

            contextMenu.Items.Add(new ToolStripSeparator());

            // Run on Startup toggle
            _startupMenuItem = new ToolStripMenuItem("Run on Windows Startup", null, (s, e) => ToggleStartup());
            _startupMenuItem.Checked = IsInStartup();
            contextMenu.Items.Add(_startupMenuItem);

            contextMenu.Items.Add(new ToolStripSeparator());

            // Exit
            var exitMenuItem = new ToolStripMenuItem("Exit", null, (s, e) => ExitApplication());
            contextMenu.Items.Add(exitMenuItem);

            _notifyIcon.ContextMenuStrip = contextMenu;

            // Double-click to show window
            _notifyIcon.DoubleClick += (s, e) => ShowMainWindow();
        }

        private void LoadIcon()
        {
            try
            {
                // Try to load from app resources
                var uri = new Uri("pack://application:,,,/app.ico", UriKind.Absolute);
                var streamInfo = Application.GetResourceStream(uri);
                if (streamInfo != null)
                {
                    _notifyIcon.Icon = new Icon(streamInfo.Stream);
                    return;
                }
            }
            catch { }

            try
            {
                // Fallback: load from file next to exe
                var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ico");
                if (File.Exists(iconPath))
                {
                    _notifyIcon.Icon = new Icon(iconPath);
                    return;
                }
            }
            catch { }

            // Last resort: use system application icon
            _notifyIcon.Icon = SystemIcons.Application;
        }

        private void ShowMainWindow()
        {
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow != null)
            {
                mainWindow.Show();
                mainWindow.WindowState = WindowState.Normal;
                mainWindow.Activate();
            }
        }

        private bool IsInStartup()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(_registryKey, false);
                var value = key?.GetValue(_appName) as string;
                return !string.IsNullOrEmpty(value);
            }
            catch
            {
                return false;
            }
        }

        private void ToggleStartup()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(_registryKey, true);
                if (key == null) return;

                if (IsInStartup())
                {
                    // Remove from startup
                    key.DeleteValue(_appName, false);
                    _startupMenuItem.Checked = false;
                    _notifyIcon.ShowBalloonTip(2000, "Startup Master", "Removed from Windows Startup", ToolTipIcon.Info);
                }
                else
                {
                    // Add to startup
                    key.SetValue(_appName, $"\"{_appPath}\"");
                    _startupMenuItem.Checked = true;
                    _notifyIcon.ShowBalloonTip(2000, "Startup Master", "Added to Windows Startup", ToolTipIcon.Info);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Failed to modify startup settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExitApplication()
        {
            _notifyIcon.Visible = false;
            Application.Current.Shutdown();
        }

        public void ShowNotification(string title, string message, ToolTipIcon icon = ToolTipIcon.Info)
        {
            _notifyIcon.ShowBalloonTip(3000, title, message, icon);
        }

        public void RefreshStartupStatus()
        {
            _startupMenuItem.Checked = IsInStartup();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
        }
    }
}
