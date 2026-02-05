using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using StartupMaster.Models;

namespace StartupMaster.Utils
{
    public class BackupManager
    {
        private static readonly string BackupDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "StartupMaster", "Backups");

        public BackupManager()
        {
            if (!Directory.Exists(BackupDirectory))
            {
                Directory.CreateDirectory(BackupDirectory);
            }
        }

        public string CreateAutoBackup(List<StartupItem> items, string reason = "Auto")
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var filename = $"Backup_{reason}_{timestamp}.json";
            var path = Path.Combine(BackupDirectory, filename);

            var backup = new BackupData
            {
                Timestamp = DateTime.Now,
                Reason = reason,
                ItemCount = items.Count,
                Items = items
            };

            var json = JsonSerializer.Serialize(backup, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(path, json);
            CleanOldBackups();

            return path;
        }

        public List<BackupInfo> GetBackups()
        {
            var backups = new List<BackupInfo>();

            if (!Directory.Exists(BackupDirectory))
                return backups;

            foreach (var file in Directory.GetFiles(BackupDirectory, "*.json"))
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var data = JsonSerializer.Deserialize<BackupData>(json);

                    if (data != null)
                    {
                        backups.Add(new BackupInfo
                        {
                            FilePath = file,
                            FileName = Path.GetFileName(file),
                            Timestamp = data.Timestamp,
                            Reason = data.Reason,
                            ItemCount = data.ItemCount,
                            FileSize = new FileInfo(file).Length
                        });
                    }
                }
                catch { }
            }

            backups.Sort((a, b) => b.Timestamp.CompareTo(a.Timestamp));
            return backups;
        }

        public BackupData RestoreBackup(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<BackupData>(json);
        }

        public void DeleteBackup(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private void CleanOldBackups()
        {
            var backups = GetBackups();
            const int maxBackups = 50;

            if (backups.Count > maxBackups)
            {
                for (int i = maxBackups; i < backups.Count; i++)
                {
                    try
                    {
                        File.Delete(backups[i].FilePath);
                    }
                    catch { }
                }
            }
        }

        public long GetTotalBackupSize()
        {
            long total = 0;
            if (Directory.Exists(BackupDirectory))
            {
                foreach (var file in Directory.GetFiles(BackupDirectory, "*.json"))
                {
                    total += new FileInfo(file).Length;
                }
            }
            return total;
        }
    }

    public class BackupData
    {
        public DateTime Timestamp { get; set; }
        public string Reason { get; set; }
        public int ItemCount { get; set; }
        public List<StartupItem> Items { get; set; }
    }

    public class BackupInfo
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public DateTime Timestamp { get; set; }
        public string Reason { get; set; }
        public int ItemCount { get; set; }
        public long FileSize { get; set; }

        public string DisplayName => $"{Timestamp:yyyy-MM-dd HH:mm} - {Reason} ({ItemCount} items)";
        public string FileSizeFormatted => FormatFileSize(FileSize);

        private string FormatFileSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024} KB";
            return $"{bytes / (1024 * 1024)} MB";
        }
    }
}
