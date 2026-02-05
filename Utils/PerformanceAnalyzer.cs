using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using StartupMaster.Models;

namespace StartupMaster.Utils
{
    public class PerformanceAnalyzer
    {
        public AnalysisReport AnalyzeStartupItems(List<StartupItem> items)
        {
            var report = new AnalysisReport
            {
                TotalItems = items.Count,
                EnabledItems = items.Count(i => i.IsEnabled),
                DisabledItems = items.Count(i => !i.IsEnabled),
                ByLocation = new Dictionary<string, int>(),
                Recommendations = new List<string>(),
                HighImpactItems = new List<StartupItem>(),
                PotentialIssues = new List<string>()
            };

            // Group by location
            foreach (var item in items)
            {
                var location = item.LocationDisplay;
                if (!report.ByLocation.ContainsKey(location))
                    report.ByLocation[location] = 0;
                report.ByLocation[location]++;
            }

            // Analyze delays
            var delayedItems = items.Where(i => i.DelaySeconds > 0).Count();
            var immediateItems = items.Where(i => i.IsEnabled && i.DelaySeconds == 0).Count();

            report.DelayedItems = delayedItems;
            report.ImmediateStartItems = immediateItems;

            // Detect potential issues
            DetectIssues(items, report);

            // Generate recommendations
            GenerateRecommendations(items, report);

            // Identify high-impact items
            IdentifyHighImpactItems(items, report);

            return report;
        }

        private void DetectIssues(List<StartupItem> items, AnalysisReport report)
        {
            // Check for missing executables
            foreach (var item in items.Where(i => i.IsEnabled))
            {
                if (!string.IsNullOrEmpty(item.Command))
                {
                    var path = item.Command.Trim('"');
                    if (!File.Exists(path) && !path.Contains("%") && !path.StartsWith("$"))
                    {
                        report.PotentialIssues.Add(
                            $"Missing executable: {item.Name} → {path}");
                    }
                }
            }

            // Check for duplicates
            var commandGroups = items
                .Where(i => i.IsEnabled)
                .GroupBy(i => i.Command.ToLower())
                .Where(g => g.Count() > 1);

            foreach (var group in commandGroups)
            {
                report.PotentialIssues.Add(
                    $"Duplicate startup entry: {group.First().Name} ({group.Count()}x)");
            }

            // Check for too many immediate starts
            if (report.ImmediateStartItems > 15)
            {
                report.PotentialIssues.Add(
                    $"High number of immediate starts ({report.ImmediateStartItems}). Consider adding delays.");
            }

            // Check for registry vs task scheduler preference
            var registryItems = items.Count(i => 
                i.Location == StartupLocation.RegistryCurrentUser || 
                i.Location == StartupLocation.RegistryLocalMachine);
            
            var taskItems = items.Count(i => i.Location == StartupLocation.TaskScheduler);

            if (registryItems > taskItems * 2)
            {
                report.PotentialIssues.Add(
                    "Many registry startup items. Consider moving some to Task Scheduler for better delay control.");
            }
        }

        private void GenerateRecommendations(List<StartupItem> items, AnalysisReport report)
        {
            // Recommend delays for heavy applications
            var heavyApps = new[] { "chrome", "firefox", "teams", "slack", "discord", "outlook" };
            
            foreach (var item in items.Where(i => i.IsEnabled && i.DelaySeconds == 0))
            {
                var cmd = item.Command.ToLower();
                if (heavyApps.Any(app => cmd.Contains(app)))
                {
                    report.Recommendations.Add(
                        $"Add startup delay to {item.Name} (detected resource-heavy application)");
                }
            }

            // Recommend Task Scheduler migration
            var registryNoDelay = items.Where(i => 
                i.IsEnabled && 
                (i.Location == StartupLocation.RegistryCurrentUser || 
                 i.Location == StartupLocation.RegistryLocalMachine))
                .Count();

            if (registryNoDelay > 5)
            {
                report.Recommendations.Add(
                    $"Consider moving {registryNoDelay} registry items to Task Scheduler for delay support");
            }

            // Recommend disabling unused items
            if (report.EnabledItems > 20)
            {
                report.Recommendations.Add(
                    $"High startup item count ({report.EnabledItems}). Review and disable non-essential items.");
            }

            // Recommend backup
            report.Recommendations.Add("Create a backup before making changes");

            // Recommend cleanup
            if (report.PotentialIssues.Count > 0)
            {
                report.Recommendations.Add($"Address {report.PotentialIssues.Count} potential issues detected");
            }
        }

        private void IdentifyHighImpactItems(List<StartupItem> items, AnalysisReport report)
        {
            var highImpactKeywords = new[]
            {
                "update", "cloud", "sync", "backup", "antivirus", "security",
                "chrome", "firefox", "edge", "teams", "slack", "discord",
                "onedrive", "dropbox", "google", "adobe", "creative"
            };

            foreach (var item in items.Where(i => i.IsEnabled))
            {
                var name = item.Name.ToLower();
                var command = item.Command.ToLower();

                if (highImpactKeywords.Any(kw => name.Contains(kw) || command.Contains(kw)))
                {
                    report.HighImpactItems.Add(item);
                }
            }
        }

        public string GenerateTextReport(AnalysisReport report)
        {
            var text = new System.Text.StringBuilder();

            text.AppendLine("========================================");
            text.AppendLine("   STARTUP PERFORMANCE ANALYSIS");
            text.AppendLine("========================================");
            text.AppendLine();
            text.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            text.AppendLine();

            text.AppendLine("SUMMARY");
            text.AppendLine("-------");
            text.AppendLine($"Total Items:          {report.TotalItems}");
            text.AppendLine($"Enabled:              {report.EnabledItems}");
            text.AppendLine($"Disabled:             {report.DisabledItems}");
            text.AppendLine($"Immediate Starts:     {report.ImmediateStartItems}");
            text.AppendLine($"Delayed Starts:       {report.DelayedItems}");
            text.AppendLine();

            text.AppendLine("BY LOCATION");
            text.AppendLine("-----------");
            foreach (var kvp in report.ByLocation.OrderByDescending(x => x.Value))
            {
                text.AppendLine($"{kvp.Key,-25} {kvp.Value}");
            }
            text.AppendLine();

            if (report.HighImpactItems.Count > 0)
            {
                text.AppendLine($"HIGH-IMPACT ITEMS ({report.HighImpactItems.Count})");
                text.AppendLine("------------------");
                foreach (var item in report.HighImpactItems.Take(10))
                {
                    text.AppendLine($"- {item.Name}");
                }
                text.AppendLine();
            }

            if (report.PotentialIssues.Count > 0)
            {
                text.AppendLine($"POTENTIAL ISSUES ({report.PotentialIssues.Count})");
                text.AppendLine("----------------");
                foreach (var issue in report.PotentialIssues)
                {
                    text.AppendLine($"⚠ {issue}");
                }
                text.AppendLine();
            }

            if (report.Recommendations.Count > 0)
            {
                text.AppendLine($"RECOMMENDATIONS ({report.Recommendations.Count})");
                text.AppendLine("---------------");
                foreach (var rec in report.Recommendations)
                {
                    text.AppendLine($"→ {rec}");
                }
                text.AppendLine();
            }

            text.AppendLine("========================================");

            return text.ToString();
        }
    }

    public class AnalysisReport
    {
        public int TotalItems { get; set; }
        public int EnabledItems { get; set; }
        public int DisabledItems { get; set; }
        public int ImmediateStartItems { get; set; }
        public int DelayedItems { get; set; }
        public Dictionary<string, int> ByLocation { get; set; }
        public List<string> Recommendations { get; set; }
        public List<string> PotentialIssues { get; set; }
        public List<StartupItem> HighImpactItems { get; set; }
    }
}
