using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StartupMaster.Models
{
    public enum StartupLocation
    {
        RegistryCurrentUser,
        RegistryLocalMachine,
        StartupFolder,
        TaskScheduler,
        Service
    }

    public class StartupItem : INotifyPropertyChanged
    {
        private bool _isEnabled;
        private int _delaySeconds;
        private string _name;
        private string _command;
        private string _arguments;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string Command
        {
            get => _command;
            set { _command = value; OnPropertyChanged(); }
        }

        public string Arguments
        {
            get => _arguments;
            set { _arguments = value; OnPropertyChanged(); }
        }

        public StartupLocation Location { get; set; }

        public bool IsEnabled
        {
            get => _isEnabled;
            set { _isEnabled = value; OnPropertyChanged(); }
        }

        public int DelaySeconds
        {
            get => _delaySeconds;
            set { _delaySeconds = value; OnPropertyChanged(); }
        }

        public string RegistryKey { get; set; }
        public string RegistryValueName { get; set; }
        public string FilePath { get; set; }
        public string TaskName { get; set; }
        public string ServiceName { get; set; }
        
        // Critical items should not be disabled (system-essential)
        public bool IsCritical { get; set; }
        public string CriticalReason { get; set; }
        
        // Publisher/Company name for identification
        public string Publisher { get; set; }
        
        // Estimated boot time impact in seconds
        public double EstimatedImpactSeconds { get; set; }
        public string ImpactDisplay => EstimatedImpactSeconds > 0 
            ? $"~{EstimatedImpactSeconds:F1}s" 
            : "< 0.5s";

        public string LocationDisplay => Location switch
        {
            StartupLocation.RegistryCurrentUser => "Registry (User)",
            StartupLocation.RegistryLocalMachine => "Registry (Machine)",
            StartupLocation.StartupFolder => "Startup Folder",
            StartupLocation.TaskScheduler => "Task Scheduler",
            StartupLocation.Service => "Service",
            _ => "Unknown"
        };

        public string StatusDisplay => IsCritical ? "🔒 Critical" : (IsEnabled ? "✓ Enabled" : "✗ Disabled");

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
