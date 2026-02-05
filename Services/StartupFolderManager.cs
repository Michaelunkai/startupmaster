using StartupMaster.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace StartupMaster.Services
{
    public class StartupFolderManager
    {
        private static readonly string UserStartupFolder = 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup));
        
        private static readonly string CommonStartupFolder = 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup));

        public List<StartupItem> GetItems()
        {
            var items = new List<StartupItem>();

            items.AddRange(GetFolderItems(UserStartupFolder, StartupLocation.RegistryCurrentUser));
            items.AddRange(GetFolderItems(CommonStartupFolder, StartupLocation.RegistryLocalMachine));

            return items;
        }

        private List<StartupItem> GetFolderItems(string folderPath, StartupLocation location)
        {
            var items = new List<StartupItem>();

            if (!Directory.Exists(folderPath))
                return items;

            foreach (var file in Directory.GetFiles(folderPath))
            {
                var ext = Path.GetExtension(file).ToLower();
                
                if (ext == ".lnk")
                {
                    try
                    {
                        var (targetPath, arguments) = ResolveShortcut(file);
                        
                        items.Add(new StartupItem
                        {
                            Name = Path.GetFileNameWithoutExtension(file),
                            Command = targetPath,
                            Arguments = arguments,
                            Location = StartupLocation.StartupFolder,
                            IsEnabled = true,
                            FilePath = file
                        });
                    }
                    catch { }
                }
                else if (ext == ".exe" || ext == ".bat" || ext == ".cmd" || ext == ".vbs")
                {
                    items.Add(new StartupItem
                    {
                        Name = Path.GetFileNameWithoutExtension(file),
                        Command = file,
                        Arguments = string.Empty,
                        Location = StartupLocation.StartupFolder,
                        IsEnabled = true,
                        FilePath = file
                    });
                }
            }

            return items;
        }

        public bool AddItem(StartupItem item, bool allUsers = false)
        {
            try
            {
                var folder = allUsers ? CommonStartupFolder : UserStartupFolder;
                var shortcutPath = Path.Combine(folder, item.Name + ".lnk");

                CreateShortcut(shortcutPath, item.Command, item.Arguments ?? string.Empty);

                item.FilePath = shortcutPath;
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
                if (File.Exists(item.FilePath))
                {
                    File.Delete(item.FilePath);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool DisableItem(StartupItem item)
        {
            try
            {
                var disabledPath = item.FilePath + ".disabled";
                File.Move(item.FilePath, disabledPath);
                item.FilePath = disabledPath;
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
                if (item.FilePath.EndsWith(".disabled"))
                {
                    var enabledPath = item.FilePath.Replace(".disabled", "");
                    File.Move(item.FilePath, enabledPath);
                    item.FilePath = enabledPath;
                    item.IsEnabled = true;
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Native shortcut creation using Shell32
        private void CreateShortcut(string shortcutPath, string targetPath, string arguments)
        {
            var shell = Type.GetTypeFromProgID("WScript.Shell");
            dynamic wsh = Activator.CreateInstance(shell);
            var shortcut = wsh.CreateShortcut(shortcutPath);
            shortcut.TargetPath = targetPath;
            shortcut.Arguments = arguments;
            shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath) ?? "";
            shortcut.Save();
            Marshal.ReleaseComObject(shortcut);
            Marshal.ReleaseComObject(wsh);
        }

        // Native shortcut resolution using Shell32
        private (string targetPath, string arguments) ResolveShortcut(string shortcutPath)
        {
            try
            {
                var shell = Type.GetTypeFromProgID("WScript.Shell");
                dynamic wsh = Activator.CreateInstance(shell);
                var shortcut = wsh.CreateShortcut(shortcutPath);
                var target = shortcut.TargetPath as string ?? "";
                var args = shortcut.Arguments as string ?? "";
                Marshal.ReleaseComObject(shortcut);
                Marshal.ReleaseComObject(wsh);
                return (target, args);
            }
            catch
            {
                return (string.Empty, string.Empty);
            }
        }
    }
}
