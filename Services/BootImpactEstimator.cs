#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using StartupMaster.Models;

namespace StartupMaster.Services
{
    /// <summary>
    /// Estimates boot time impact for each startup item
    /// Uses file size, type, and known app patterns to estimate
    /// </summary>
    public static class BootImpactEstimator
    {
        // Known heavy apps with estimated impact (seconds)
        private static readonly Dictionary<string, double> KnownAppImpacts = new(StringComparer.OrdinalIgnoreCase)
        {
            // Heavy apps (3-8 seconds)
            {"steam", 5.0}, {"steamservice", 3.0},
            {"discord", 4.0}, {"spotify", 3.5},
            {"teams", 6.0}, {"slack", 4.0}, {"zoom", 3.5},
            {"chrome", 3.0}, {"firefox", 2.5}, {"edge", 2.0}, {"msedge", 2.0},
            {"onedrive", 4.0}, {"dropbox", 3.5}, {"googledrive", 3.0},
            {"adobe", 5.0}, {"creative cloud", 6.0}, {"ccxprocess", 3.0},
            {"photoshop", 8.0}, {"illustrator", 7.0}, {"premiere", 8.0},
            
            // Medium apps (1-3 seconds)
            {"nvidia", 2.0}, {"nvcontainer", 1.5}, {"nvbackend", 1.0},
            {"amd", 1.5}, {"radeon", 2.0},
            {"realtek", 1.0}, {"rthdvcpl", 1.0},
            {"intel", 1.0}, {"igfx", 0.8},
            {"logitech", 1.5}, {"razer", 2.0}, {"corsair", 1.5},
            {"vmware", 3.0}, {"virtualbox", 2.5},
            {"docker", 4.0},
            {"antivirus", 3.0}, {"avg", 2.5}, {"avast", 3.0},
            
            // Light apps (0.5-1 seconds)
            {"autohotkey", 0.5}, {"ahk", 0.5},
            {"clipboard", 0.3}, {"snip", 0.3},
            {"wallpaper", 0.5},
            {"rainmeter", 1.0},
            {"f.lux", 0.3}, {"nightlight", 0.2}
        };

        // Service type multipliers
        private const double ServiceBaseImpact = 1.0;
        private const double RegistryBaseImpact = 0.8;
        private const double TaskSchedulerBaseImpact = 0.5;
        private const double StartupFolderBaseImpact = 1.0;

        /// <summary>
        /// Estimates boot impact for a single item
        /// </summary>
        public static void EstimateImpact(StartupItem item)
        {
            double impact = 0;

            // Check known apps first
            var searchText = $"{item.Name} {item.Command}".ToLower();
            foreach (var known in KnownAppImpacts)
            {
                if (searchText.Contains(known.Key.ToLower()))
                {
                    impact = Math.Max(impact, known.Value);
                }
            }

            // If no known match, estimate based on type and file size
            if (impact == 0)
            {
                impact = item.Location switch
                {
                    StartupLocation.Service => ServiceBaseImpact,
                    StartupLocation.RegistryCurrentUser => RegistryBaseImpact,
                    StartupLocation.RegistryLocalMachine => RegistryBaseImpact,
                    StartupLocation.TaskScheduler => TaskSchedulerBaseImpact,
                    StartupLocation.StartupFolder => StartupFolderBaseImpact,
                    _ => 0.5
                };

                // Adjust by file size if we can find the exe
                var exePath = GetExecutablePath(item.Command);
                if (!string.IsNullOrEmpty(exePath) && File.Exists(exePath))
                {
                    try
                    {
                        var fileInfo = new FileInfo(exePath);
                        var sizeMB = fileInfo.Length / (1024.0 * 1024.0);
                        
                        // Larger files = longer load time
                        if (sizeMB > 100) impact += 3.0;
                        else if (sizeMB > 50) impact += 2.0;
                        else if (sizeMB > 20) impact += 1.0;
                        else if (sizeMB > 5) impact += 0.5;
                    }
                    catch { }
                }

                // Services generally take longer
                if (item.Location == StartupLocation.Service)
                {
                    impact *= 1.5;
                }
            }

            // Round to 1 decimal place
            item.EstimatedImpactSeconds = Math.Round(impact, 1);
        }

        private static string? GetExecutablePath(string? command)
        {
            if (string.IsNullOrEmpty(command)) return null;

            var path = command.Trim();
            
            // Handle quoted paths
            if (path.StartsWith("\""))
            {
                var endQuote = path.IndexOf('"', 1);
                if (endQuote > 0)
                    path = path.Substring(1, endQuote - 1);
            }
            else
            {
                // Take first part before space
                var space = path.IndexOf(' ');
                if (space > 0)
                    path = path.Substring(0, space);
            }

            // Expand environment variables
            path = Environment.ExpandEnvironmentVariables(path);

            return path;
        }

        /// <summary>
        /// Estimates impact for all items
        /// </summary>
        public static void EstimateAll(IEnumerable<StartupItem> items)
        {
            foreach (var item in items)
            {
                EstimateImpact(item);
            }
        }

        /// <summary>
        /// Gets total potential boot time savings
        /// </summary>
        public static double GetTotalPotentialSavings(IEnumerable<StartupItem> items)
        {
            double total = 0;
            foreach (var item in items)
            {
                if (item.IsEnabled)
                    total += item.EstimatedImpactSeconds;
            }
            return total;
        }
    }
}
