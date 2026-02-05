using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StartupMaster.Models;

namespace StartupMaster.Utils
{
    public class StartupImpactCalculator
    {
        private static readonly Dictionary<string, int> KnownImpactScores = new()
        {
            // Very high impact (10)
            { "chrome", 10 }, { "firefox", 10 }, { "edge", 10 },
            { "teams", 10 }, { "slack", 10 }, { "discord", 10 },
            { "outlook", 10 }, { "onedrive", 9 }, { "dropbox", 9 },
            
            // High impact (7-8)
            { "adobe", 8 }, { "creative", 8 }, { "photoshop", 8 },
            { "steam", 7 }, { "epic", 7 }, { "spotify", 7 },
            
            // Medium impact (4-6)
            { "skype", 6 }, { "zoom", 6 }, { "nvidia", 5 },
            { "amd", 5 }, { "intel", 5 }, { "java", 4 },
            
            // Low impact (1-3)
            { "update", 2 }, { "helper", 2 }, { "tray", 1 }
        };

        public int CalculateImpact(StartupItem item)
        {
            int score = 0;

            // Base score from known apps
            var nameLower = item.Name.ToLower();
            var cmdLower = item.Command.ToLower();

            foreach (var kvp in KnownImpactScores)
            {
                if (nameLower.Contains(kvp.Key) || cmdLower.Contains(kvp.Key))
                {
                    score = Math.Max(score, kvp.Value);
                    break;
                }
            }

            // If not in known list, estimate by file size
            if (score == 0 && File.Exists(item.Command))
            {
                try
                {
                    var sizeInMB = new FileInfo(item.Command).Length / (1024.0 * 1024.0);
                    if (sizeInMB > 100) score = 9;
                    else if (sizeInMB > 50) score = 7;
                    else if (sizeInMB > 10) score = 5;
                    else score = 3;
                }
                catch { score = 5; } // Default medium if can't determine
            }

            // Adjust by location
            score += item.Location switch
            {
                StartupLocation.RegistryLocalMachine => 1, // System-wide = higher impact
                StartupLocation.Service => 2, // Services = highest impact
                _ => 0
            };

            // Adjust by delay
            if (item.DelaySeconds > 30)
                score -= 2; // Already delayed = lower effective impact

            return Math.Clamp(score, 1, 10);
        }

        public string GetImpactDescription(int score)
        {
            return score switch
            {
                >= 9 => "Critical - Major boot impact",
                >= 7 => "High - Significant boot delay",
                >= 5 => "Medium - Noticeable impact",
                >= 3 => "Low - Minor impact",
                _ => "Minimal - Negligible impact"
            };
        }

        public string GetImpactEmoji(int score)
        {
            return score switch
            {
                >= 9 => "🔴",
                >= 7 => "🟠",
                >= 5 => "🟡",
                >= 3 => "🟢",
                _ => "⚪"
            };
        }

        public List<StartupItem> GetHighImpactItems(List<StartupItem> items)
        {
            return items
                .Where(i => i.IsEnabled && CalculateImpact(i) >= 7)
                .OrderByDescending(i => CalculateImpact(i))
                .ToList();
        }

        public int EstimateBootTimeSeconds(List<StartupItem> items)
        {
            int totalTime = 0;

            foreach (var item in items.Where(i => i.IsEnabled))
            {
                int impact = CalculateImpact(item);
                
                // Estimate time based on impact
                int itemTime = impact switch
                {
                    >= 9 => 8,  // 8 seconds
                    >= 7 => 5,  // 5 seconds
                    >= 5 => 3,  // 3 seconds
                    >= 3 => 2,  // 2 seconds
                    _ => 1      // 1 second
                };

                // Items with delays don't add to initial boot time
                if (item.DelaySeconds == 0)
                {
                    totalTime += itemTime;
                }
            }

            return totalTime;
        }

        public string GetOptimizationSuggestion(StartupItem item)
        {
            int impact = CalculateImpact(item);

            if (impact >= 9)
                return "Consider adding 60+ second delay or disabling if not essential";
            if (impact >= 7)
                return "Add 30-60 second delay to reduce boot spike";
            if (impact >= 5)
                return "Consider 15-30 second delay for better boot distribution";
            
            return "No optimization needed";
        }
    }
}
