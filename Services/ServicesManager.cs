#nullable enable
using StartupMaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;

namespace StartupMaster.Services
{
    public class ServicesManager
    {
        public List<StartupItem> GetItems()
        {
            var items = new List<StartupItem>();

            try
            {
                // Get ALL services that start automatically (Automatic, Boot, System)
                var services = ServiceController.GetServices()
                    .Where(s => s.StartType == ServiceStartMode.Automatic ||
                                s.StartType == ServiceStartMode.Boot ||
                                s.StartType == ServiceStartMode.System);

                foreach (var service in services)
                {
                    try
                    {
                        var item = new StartupItem
                        {
                            Name = service.DisplayName,
                            Command = service.ServiceName,
                            Arguments = GetServiceStartType(service.StartType),
                            Location = StartupLocation.Service,
                            IsEnabled = service.StartType == ServiceStartMode.Automatic ||
                                        service.StartType == ServiceStartMode.Boot ||
                                        service.StartType == ServiceStartMode.System,
                            ServiceName = service.ServiceName,
                            Publisher = GetServicePublisher(service.ServiceName)
                        };

                        // Mark Boot and System services as critical
                        if (service.StartType == ServiceStartMode.Boot ||
                            service.StartType == ServiceStartMode.System)
                        {
                            item.IsCritical = true;
                            item.CriticalReason = $"{service.StartType} service - required for Windows startup";
                        }

                        items.Add(item);
                    }
                    catch { }
                }

                // Also get Delayed Auto-Start services
                try
                {
                    var delayedServices = GetDelayedAutoStartServices();
                    foreach (var serviceName in delayedServices)
                    {
                        if (items.Any(i => i.ServiceName == serviceName)) continue;
                        
                        try
                        {
                            var service = new ServiceController(serviceName);
                            items.Add(new StartupItem
                            {
                                Name = service.DisplayName,
                                Command = service.ServiceName,
                                Arguments = "Automatic (Delayed)",
                                Location = StartupLocation.Service,
                                IsEnabled = true,
                                ServiceName = service.ServiceName,
                                Publisher = GetServicePublisher(serviceName)
                            });
                        }
                        catch { }
                    }
                }
                catch { }
            }
            catch { }

            return items;
        }

        private string GetServiceStartType(ServiceStartMode mode) => mode switch
        {
            ServiceStartMode.Boot => "Boot",
            ServiceStartMode.System => "System",
            ServiceStartMode.Automatic => "Automatic",
            ServiceStartMode.Manual => "Manual",
            ServiceStartMode.Disabled => "Disabled",
            _ => "Unknown"
        };

        private string? GetServicePublisher(string serviceName)
        {
            try
            {
                using var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                    $@"SYSTEM\CurrentControlSet\Services\{serviceName}");
                
                var imagePath = key?.GetValue("ImagePath")?.ToString();
                if (!string.IsNullOrEmpty(imagePath))
                {
                    // Clean the path
                    imagePath = imagePath.Trim('"');
                    if (imagePath.StartsWith(@"\SystemRoot\"))
                        imagePath = imagePath.Replace(@"\SystemRoot\", @"C:\Windows\");
                    if (imagePath.StartsWith(@"system32", StringComparison.OrdinalIgnoreCase))
                        imagePath = @"C:\Windows\" + imagePath;
                    
                    var firstSpace = imagePath.IndexOf(' ');
                    if (firstSpace > 0)
                        imagePath = imagePath.Substring(0, firstSpace);

                    if (System.IO.File.Exists(imagePath))
                    {
                        var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(imagePath);
                        return versionInfo.CompanyName;
                    }
                }
            }
            catch { }
            return null;
        }

        private List<string> GetDelayedAutoStartServices()
        {
            var services = new List<string>();
            try
            {
                using var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                    @"SYSTEM\CurrentControlSet\Services");
                
                if (key != null)
                {
                    foreach (var subKeyName in key.GetSubKeyNames())
                    {
                        try
                        {
                            using var subKey = key.OpenSubKey(subKeyName);
                            var start = subKey?.GetValue("Start");
                            var delayedAutoStart = subKey?.GetValue("DelayedAutostart");
                            
                            if (start != null && (int)start == 2 && // Automatic
                                delayedAutoStart != null && (int)delayedAutoStart == 1)
                            {
                                services.Add(subKeyName);
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }
            return services;
        }

        public bool DisableItem(StartupItem item)
        {
            // Block disabling critical services
            if (item.IsCritical)
            {
                return false;
            }

            try
            {
                using var sc = new System.Management.ManagementObject(
                    $"Win32_Service.Name='{item.ServiceName}'");
                sc.Get();
                sc.InvokeMethod("ChangeStartMode", new object[] { "Manual" });

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
                using var sc = new System.Management.ManagementObject(
                    $"Win32_Service.Name='{item.ServiceName}'");
                sc.Get();
                sc.InvokeMethod("ChangeStartMode", new object[] { "Automatic" });

                item.IsEnabled = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool StopService(StartupItem item)
        {
            // Block stopping critical services
            if (item.IsCritical)
            {
                return false;
            }

            try
            {
                var service = new ServiceController(item.ServiceName);
                if (service.Status == ServiceControllerStatus.Running)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool StartService(StartupItem item)
        {
            try
            {
                var service = new ServiceController(item.ServiceName);
                if (service.Status == ServiceControllerStatus.Stopped)
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
