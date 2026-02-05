using Microsoft.Win32.TaskScheduler;
using StartupMaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StartupMaster.Services
{
    public class TaskSchedulerManager
    {
        public List<StartupItem> GetItems()
        {
            var items = new List<StartupItem>();

            try
            {
                using var ts = new TaskService();
                var allTasks = ts.RootFolder.AllTasks;

                foreach (var task in allTasks)
                {
                    try
                    {
                        // Only include tasks that run at startup/logon
                        var hasStartupTrigger = task.Definition.Triggers.Any(t => 
                            t is BootTrigger || t is LogonTrigger);

                        if (!hasStartupTrigger) continue;

                        var action = task.Definition.Actions.FirstOrDefault() as ExecAction;
                        if (action == null) continue;

                        var trigger = task.Definition.Triggers.FirstOrDefault(t => 
                            t is BootTrigger || t is LogonTrigger);

                        int delaySeconds = 0;
                        if (trigger is BootTrigger bootTrigger && bootTrigger.Delay.TotalSeconds > 0)
                        {
                            delaySeconds = (int)bootTrigger.Delay.TotalSeconds;
                        }
                        else if (trigger is LogonTrigger logonTrigger && logonTrigger.Delay.TotalSeconds > 0)
                        {
                            delaySeconds = (int)logonTrigger.Delay.TotalSeconds;
                        }

                        items.Add(new StartupItem
                        {
                            Name = task.Name,
                            Command = action.Path,
                            Arguments = action.Arguments ?? string.Empty,
                            Location = StartupLocation.TaskScheduler,
                            IsEnabled = task.Enabled,
                            TaskName = task.Path,
                            DelaySeconds = delaySeconds
                        });
                    }
                    catch { }
                }
            }
            catch { }

            return items;
        }

        public bool AddItem(StartupItem item)
        {
            try
            {
                using var ts = new TaskService();
                var td = ts.NewTask();
                
                td.RegistrationInfo.Description = $"Startup task: {item.Name}";
                
                // Add logon trigger
                var trigger = new LogonTrigger();
                if (item.DelaySeconds > 0)
                {
                    trigger.Delay = TimeSpan.FromSeconds(item.DelaySeconds);
                }
                td.Triggers.Add(trigger);

                // Add action
                td.Actions.Add(new ExecAction(item.Command, item.Arguments, null));

                // Settings
                td.Settings.DisallowStartIfOnBatteries = false;
                td.Settings.StopIfGoingOnBatteries = false;
                td.Settings.ExecutionTimeLimit = TimeSpan.Zero;

                // Register task
                ts.RootFolder.RegisterTaskDefinition(
                    item.Name,
                    td,
                    TaskCreation.CreateOrUpdate,
                    null,
                    null,
                    TaskLogonType.InteractiveToken);

                item.TaskName = $"\\{item.Name}";
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
                using var ts = new TaskService();
                ts.RootFolder.DeleteTask(item.Name, false);
                return true;
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
                using var ts = new TaskService();
                var task = ts.GetTask(item.TaskName);
                if (task != null)
                {
                    task.Enabled = false;
                    item.IsEnabled = false;
                    return true;
                }
                return false;
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
                using var ts = new TaskService();
                var task = ts.GetTask(item.TaskName);
                if (task != null)
                {
                    task.Enabled = true;
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

        public bool UpdateDelay(StartupItem item)
        {
            try
            {
                using var ts = new TaskService();
                var task = ts.GetTask(item.TaskName);
                if (task == null) return false;

                var td = task.Definition;
                var trigger = td.Triggers.FirstOrDefault(t => t is BootTrigger || t is LogonTrigger);
                
                if (trigger != null)
                {
                    if (trigger is BootTrigger bootTrigger)
                    {
                        bootTrigger.Delay = TimeSpan.FromSeconds(item.DelaySeconds);
                    }
                    else if (trigger is LogonTrigger logonTrigger)
                    {
                        logonTrigger.Delay = TimeSpan.FromSeconds(item.DelaySeconds);
                    }
                    task.RegisterChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
