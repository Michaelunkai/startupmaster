#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using StartupMaster.Models;

namespace StartupMaster.Services
{
    /// <summary>
    /// Identifies critical system items that should be HIDDEN from the user
    /// Only shows items that are SAFE to disable
    /// </summary>
    public static class CriticalItemsService
    {
        // Services that should ALWAYS be hidden (system critical)
        private static readonly HashSet<string> HiddenServices = new(StringComparer.OrdinalIgnoreCase)
        {
            // Core Windows - NEVER touch
            "RpcSs", "DcomLaunch", "RpcEptMapper", "LSM", "SamSs", 
            "LanmanServer", "LanmanWorkstation", "Winmgmt", "EventLog", 
            "PlugPlay", "Power", "ProfSvc", "UserManager", "Netlogon", 
            "gpsvc", "Schedule", "CryptSvc", "Dhcp", "Dnscache", "NlaSvc", 
            "nsi", "W32Time", "Wcmsvc", "WdiServiceHost", "WdiSystemHost",
            "BrokerInfrastructure", "SystemEventsBroker", "TimeBrokerSvc",
            "StateRepository", "CoreMessagingRegistrar", "CDPSvc",
            "CDPUserSvc", "DevicesFlowUserSvc", "DeviceAssociationBrokerSvc",
            
            // Security - NEVER touch
            "WinDefend", "wscsvc", "SecurityHealthService", "mpssvc", 
            "BFE", "IKEEXT", "VaultSvc", "KeyIso", "SgrmBroker",
            "Netlogon", "SamSs", "LsaSrv",
            
            // Graphics & Display - causes black screen
            "Themes", "UxSms", "DWM", "FontCache", "GraphicsPerfSvc", 
            "DispBrokerDesktopSvc", "WMPNetworkSvc",
            
            // Storage - causes data loss
            "StorSvc", "stisvc", "VSS", "SDRSVC", "swprv",
            
            // Networking - causes no internet
            "Netman", "netprofm", "WlanSvc", "Dot3Svc", "Netwtw",
            "WwanSvc", "icssvc", "lmhosts", "NetTcpPortSharing",
            
            // System updates & install - breaks Windows
            "wuauserv", "BITS", "TrustedInstaller", "msiserver", 
            "AppXSvc", "ClipSVC", "TokenBroker", "LicenseManager",
            "InstallService", "AppReadiness",
            
            // User experience - annoying but not dangerous
            "WSearch", "SysMain", "DiagTrack",
            
            // Hardware - causes device failures  
            "hidserv", "TabletInputService", "SensorService",
            "SensrSvc", "DeviceAssociationService", "WPDBusEnum",
            "wudfsvc", "WiaRpc",
            
            // Session/Login - causes login issues
            "Appinfo", "SENS", "ShellHWDetection", "SessionEnv",
            "TermService", "UmRdpService",
            
            // Critical services by partial name match
            "AudioSrv", "AudioEndpointBuilder", "Audiosrv",
            "MMCSS", "PortableDeviceEnumerator", "WbioSrvc"
        };

        // Services to SHOW (user-installed, safe to disable)
        private static readonly HashSet<string> SafeServicePatterns = new(StringComparer.OrdinalIgnoreCase)
        {
            "docker", "vmware", "virtualbox", "hyper-v", "wsl",
            "steam", "origin", "epic", "uplay", "battlenet",
            "adobe", "creative", "dropbox", "onedrive", "google",
            "nvidia", "amd", "intel", "realtek", "gigabyte", "asus", "msi",
            "razer", "logitech", "corsair", "steelseries",
            "discord", "spotify", "zoom", "teams", "slack",
            "antivirus", "avg", "avast", "norton", "mcafee", "kaspersky",
            "backup", "sync", "cloud"
        };

        // Registry items to HIDE
        private static readonly HashSet<string> HiddenRegistryItems = new(StringComparer.OrdinalIgnoreCase)
        {
            "SecurityHealth", "SecurityHealthSystray",
            "Windows Defender", "WindowsDefender",
            "ctfmon"  // Text input - causes keyboard issues
        };

        // Task paths to HIDE (all Microsoft\Windows tasks)
        private static readonly string[] HiddenTaskPrefixes = new[]
        {
            @"\Microsoft\Windows\",
            @"\Microsoft\Office\",
            @"\Microsoft\EdgeUpdate",
            @"\Microsoft\VisualStudio",
            @"\WPD\"
        };

        // Tasks to SHOW even if in Microsoft folder
        private static readonly HashSet<string> ShowableTasks = new(StringComparer.OrdinalIgnoreCase)
        {
            // User might want to control these
        };

        /// <summary>
        /// Checks if an item should be HIDDEN (critical)
        /// </summary>
        public static void EvaluateItem(StartupItem item)
        {
            item.IsCritical = false;
            item.CriticalReason = null;

            // === SERVICES ===
            if (item.Location == StartupLocation.Service)
            {
                var serviceName = item.ServiceName ?? item.Command ?? "";
                
                // Check if explicitly hidden
                if (HiddenServices.Contains(serviceName))
                {
                    item.IsCritical = true;
                    item.CriticalReason = "Core Windows service";
                    return;
                }

                // Check if it's a Windows system service (svchost-based)
                if (item.Command?.Contains("svchost", StringComparison.OrdinalIgnoreCase) == true)
                {
                    item.IsCritical = true;
                    item.CriticalReason = "Windows system service";
                    return;
                }

                // Check if service path is in Windows folder
                if (item.Command?.Contains(@"\Windows\", StringComparison.OrdinalIgnoreCase) == true &&
                    item.Command?.Contains(@"\Windows\System32\drivers", StringComparison.OrdinalIgnoreCase) == false)
                {
                    // Most Windows services should be hidden unless explicitly safe
                    if (!IsSafeService(serviceName, item.Name ?? ""))
                    {
                        item.IsCritical = true;
                        item.CriticalReason = "Windows system component";
                        return;
                    }
                }

                // Microsoft publisher and in system path = hide
                if (item.Publisher?.Contains("Microsoft", StringComparison.OrdinalIgnoreCase) == true &&
                    item.Command?.Contains(@"\Windows\", StringComparison.OrdinalIgnoreCase) == true)
                {
                    item.IsCritical = true;
                    item.CriticalReason = "Microsoft system service";
                    return;
                }
            }

            // === REGISTRY ===
            if (item.Location == StartupLocation.RegistryCurrentUser || 
                item.Location == StartupLocation.RegistryLocalMachine)
            {
                var name = item.Name ?? item.RegistryValueName ?? "";
                
                if (HiddenRegistryItems.Contains(name))
                {
                    item.IsCritical = true;
                    item.CriticalReason = "Windows security component";
                    return;
                }

                // SecurityHealth in command path
                if (item.Command?.Contains("SecurityHealth", StringComparison.OrdinalIgnoreCase) == true)
                {
                    item.IsCritical = true;
                    item.CriticalReason = "Windows Security";
                    return;
                }
            }

            // === SCHEDULED TASKS ===
            if (item.Location == StartupLocation.TaskScheduler)
            {
                var taskPath = item.TaskName ?? "";
                
                // Hide all Microsoft\Windows tasks
                foreach (var prefix in HiddenTaskPrefixes)
                {
                    if (taskPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        item.IsCritical = true;
                        item.CriticalReason = "Windows system task";
                        return;
                    }
                }
            }
        }

        private static bool IsSafeService(string serviceName, string displayName)
        {
            var combined = $"{serviceName} {displayName}".ToLower();
            
            foreach (var pattern in SafeServicePatterns)
            {
                if (combined.Contains(pattern.ToLower()))
                    return true;
            }
            
            return false;
        }

        /// <summary>
        /// Batch evaluate all items
        /// </summary>
        public static void EvaluateAll(IEnumerable<StartupItem> items)
        {
            foreach (var item in items)
            {
                EvaluateItem(item);
            }
        }
    }
}
