#nullable enable
using Microsoft.Win32;
using StartupMaster.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace StartupMaster.Services
{
    public class RegistryStartupManager
    {
        // Standard Run keys
        private static readonly string[] RegistryPaths = new[]
        {
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Run",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\RunOnce",
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\RunServices",
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\RunServicesOnce"
        };

        // Startup Approved keys (contain disabled items)
        private static readonly string StartupApprovedRun = 
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";
        private static readonly string StartupApprovedRun32 = 
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run32";

        public List<StartupItem> GetItems()
        {
            var items = new List<StartupItem>();
            var disabledItems = GetDisabledItemNames();

            // Current User
            foreach (var path in RegistryPaths)
            {
                try
                {
                    using var key = Registry.CurrentUser.OpenSubKey(path);
                    if (key != null)
                    {
                        items.AddRange(ReadRegistryKey(key, path, StartupLocation.RegistryCurrentUser, disabledItems));
                    }
                }
                catch { }
            }

            // Local Machine
            foreach (var path in RegistryPaths)
            {
                try
                {
                    using var key = Registry.LocalMachine.OpenSubKey(path);
                    if (key != null)
                    {
                        items.AddRange(ReadRegistryKey(key, path, StartupLocation.RegistryLocalMachine, disabledItems));
                    }
                }
                catch { }
            }

            // Also read disabled items that might only exist in StartupApproved
            items.AddRange(GetDisabledOnlyItems(disabledItems));

            return items;
        }

        private HashSet<string> GetDisabledItemNames()
        {
            var disabled = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            
            // Check Current User StartupApproved
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(StartupApprovedRun);
                if (key != null)
                {
                    foreach (var name in key.GetValueNames())
                    {
                        var value = key.GetValue(name) as byte[];
                        if (value != null && value.Length >= 12)
                        {
                            // First bytes indicate enabled (02) or disabled (03)
                            if (value[0] == 0x03)
                            {
                                disabled.Add(name);
                            }
                        }
                    }
                }
            }
            catch { }

            // Check Local Machine StartupApproved
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(StartupApprovedRun);
                if (key != null)
                {
                    foreach (var name in key.GetValueNames())
                    {
                        var value = key.GetValue(name) as byte[];
                        if (value != null && value.Length >= 12)
                        {
                            if (value[0] == 0x03)
                            {
                                disabled.Add(name);
                            }
                        }
                    }
                }
            }
            catch { }

            // Check Run32 variants
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(StartupApprovedRun32);
                if (key != null)
                {
                    foreach (var name in key.GetValueNames())
                    {
                        var value = key.GetValue(name) as byte[];
                        if (value != null && value.Length >= 12 && value[0] == 0x03)
                        {
                            disabled.Add(name);
                        }
                    }
                }
            }
            catch { }

            return disabled;
        }

        private List<StartupItem> GetDisabledOnlyItems(HashSet<string> disabledItems)
        {
            var items = new List<StartupItem>();
            // Items that are in StartupApproved but not in Run key are disabled
            // They need to be shown too
            return items;
        }

        private List<StartupItem> ReadRegistryKey(RegistryKey key, string path, StartupLocation location, HashSet<string> disabledItems)
        {
            var items = new List<StartupItem>();
            
            foreach (var valueName in key.GetValueNames())
            {
                try
                {
                    var value = key.GetValue(valueName)?.ToString();
                    if (string.IsNullOrEmpty(value)) continue;

                    var parts = SplitCommandLine(value);
                    var isDisabled = disabledItems.Contains(valueName);
                    
                    var item = new StartupItem
                    {
                        Name = valueName,
                        Command = parts.command,
                        Arguments = parts.arguments,
                        Location = location,
                        IsEnabled = !isDisabled,
                        RegistryKey = path,
                        RegistryValueName = valueName,
                        Publisher = GetPublisher(parts.command)
                    };

                    items.Add(item);
                }
                catch { }
            }

            return items;
        }

        private string? GetPublisher(string? command)
        {
            if (string.IsNullOrEmpty(command)) return null;
            
            try
            {
                var path = command;
                
                // Handle quoted paths
                if (path.StartsWith("\""))
                {
                    var endQuote = path.IndexOf('"', 1);
                    if (endQuote > 0)
                        path = path.Substring(1, endQuote - 1);
                }
                
                // Handle environment variables
                path = Environment.ExpandEnvironmentVariables(path);
                
                if (File.Exists(path))
                {
                    var versionInfo = FileVersionInfo.GetVersionInfo(path);
                    return versionInfo.CompanyName;
                }
            }
            catch { }
            
            return null;
        }

        public bool AddItem(StartupItem item)
        {
            try
            {
                var rootKey = item.Location == StartupLocation.RegistryCurrentUser 
                    ? Registry.CurrentUser 
                    : Registry.LocalMachine;

                using var key = rootKey.OpenSubKey(item.RegistryKey ?? @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (key == null) return false;

                var value = string.IsNullOrEmpty(item.Arguments)
                    ? item.Command
                    : $"\"{item.Command}\" {item.Arguments}";

                key.SetValue(item.RegistryValueName ?? item.Name, value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool RemoveItem(StartupItem item)
        {
            try
            {
                var rootKey = item.Location == StartupLocation.RegistryCurrentUser
                    ? Registry.CurrentUser
                    : Registry.LocalMachine;

                using var key = rootKey.OpenSubKey(item.RegistryKey ?? "", true);
                key?.DeleteValue(item.RegistryValueName ?? "", false);
                
                // Also remove from StartupApproved if present
                RemoveFromStartupApproved(item);
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DisableItem(StartupItem item)
        {
            // Block disabling critical items
            if (item.IsCritical)
            {
                return false;
            }

            try
            {
                // Use StartupApproved method (Windows 8+ style)
                var rootKey = item.Location == StartupLocation.RegistryCurrentUser
                    ? Registry.CurrentUser
                    : Registry.LocalMachine;

                using var key = rootKey.CreateSubKey(StartupApprovedRun);
                if (key != null)
                {
                    // Create disabled entry (03 00 00 00 00 00 00 00 00 00 00 00)
                    var disabledValue = new byte[12];
                    disabledValue[0] = 0x03;
                    key.SetValue(item.RegistryValueName ?? item.Name, disabledValue, RegistryValueKind.Binary);
                }
                
                item.IsEnabled = false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool EnableItem(StartupItem item)
        {
            try
            {
                var rootKey = item.Location == StartupLocation.RegistryCurrentUser
                    ? Registry.CurrentUser
                    : Registry.LocalMachine;

                using var key = rootKey.CreateSubKey(StartupApprovedRun);
                if (key != null)
                {
                    // Create enabled entry (02 00 00 00 00 00 00 00 00 00 00 00)
                    var enabledValue = new byte[12];
                    enabledValue[0] = 0x02;
                    key.SetValue(item.RegistryValueName ?? item.Name, enabledValue, RegistryValueKind.Binary);
                }
                
                item.IsEnabled = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void RemoveFromStartupApproved(StartupItem item)
        {
            try
            {
                var rootKey = item.Location == StartupLocation.RegistryCurrentUser
                    ? Registry.CurrentUser
                    : Registry.LocalMachine;

                using var key = rootKey.OpenSubKey(StartupApprovedRun, true);
                key?.DeleteValue(item.RegistryValueName ?? item.Name, false);
            }
            catch { }
        }

        private (string command, string arguments) SplitCommandLine(string commandLine)
        {
            if (string.IsNullOrWhiteSpace(commandLine))
                return (string.Empty, string.Empty);

            commandLine = commandLine.Trim();

            if (commandLine.StartsWith("\""))
            {
                var endQuote = commandLine.IndexOf('\"', 1);
                if (endQuote > 0)
                {
                    var command = commandLine.Substring(1, endQuote - 1);
                    var arguments = commandLine.Substring(endQuote + 1).Trim();
                    return (command, arguments);
                }
            }

            var firstSpace = commandLine.IndexOf(' ');
            if (firstSpace > 0)
            {
                return (commandLine.Substring(0, firstSpace), commandLine.Substring(firstSpace + 1).Trim());
            }

            return (commandLine, string.Empty);
        }
    }
}
