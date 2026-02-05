using StartupMaster.Models;
using System.Collections.Generic;
using System.Linq;

namespace StartupMaster.Services
{
    public class StartupManager
    {
        private readonly RegistryStartupManager _registryManager = new();
        private readonly StartupFolderManager _folderManager = new();
        private readonly TaskSchedulerManager _taskManager = new();
        private readonly ServicesManager _servicesManager = new();

        public List<StartupItem> GetAllItems()
        {
            var items = new List<StartupItem>();

            items.AddRange(_registryManager.GetItems());
            items.AddRange(_folderManager.GetItems());
            items.AddRange(_taskManager.GetItems());
            items.AddRange(_servicesManager.GetItems());

            // Evaluate critical items
            CriticalItemsService.EvaluateAll(items);

            // Filter out critical items - user shouldn't see things they can't disable
            var safeItems = items.Where(i => !i.IsCritical).ToList();

            // Estimate boot impact for each item
            BootImpactEstimator.EstimateAll(safeItems);

            return safeItems.OrderByDescending(i => i.EstimatedImpactSeconds).ToList();
        }

        public bool AddItem(StartupItem item)
        {
            return item.Location switch
            {
                StartupLocation.RegistryCurrentUser => _registryManager.AddItem(item),
                StartupLocation.RegistryLocalMachine => _registryManager.AddItem(item),
                StartupLocation.StartupFolder => _folderManager.AddItem(item),
                StartupLocation.TaskScheduler => _taskManager.AddItem(item),
                _ => false
            };
        }

        public bool RemoveItem(StartupItem item)
        {
            return item.Location switch
            {
                StartupLocation.RegistryCurrentUser => _registryManager.RemoveItem(item),
                StartupLocation.RegistryLocalMachine => _registryManager.RemoveItem(item),
                StartupLocation.StartupFolder => _folderManager.RemoveItem(item),
                StartupLocation.TaskScheduler => _taskManager.RemoveItem(item),
                _ => false
            };
        }

        public bool DisableItem(StartupItem item)
        {
            return item.Location switch
            {
                StartupLocation.RegistryCurrentUser => _registryManager.DisableItem(item),
                StartupLocation.RegistryLocalMachine => _registryManager.DisableItem(item),
                StartupLocation.StartupFolder => _folderManager.DisableItem(item),
                StartupLocation.TaskScheduler => _taskManager.DisableItem(item),
                StartupLocation.Service => _servicesManager.DisableItem(item),
                _ => false
            };
        }

        public bool EnableItem(StartupItem item)
        {
            return item.Location switch
            {
                StartupLocation.RegistryCurrentUser => _registryManager.EnableItem(item),
                StartupLocation.RegistryLocalMachine => _registryManager.EnableItem(item),
                StartupLocation.StartupFolder => _folderManager.EnableItem(item),
                StartupLocation.TaskScheduler => _taskManager.EnableItem(item),
                StartupLocation.Service => _servicesManager.EnableItem(item),
                _ => false
            };
        }

        public bool UpdateDelay(StartupItem item)
        {
            if (item.Location == StartupLocation.TaskScheduler)
            {
                return _taskManager.UpdateDelay(item);
            }
            return false;
        }
    }
}
