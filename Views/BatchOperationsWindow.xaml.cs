using StartupMaster.Models;
using StartupMaster.Services;
using StartupMaster.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace StartupMaster.Views
{
    public partial class BatchOperationsWindow : Window
    {
        private readonly List<StartupItem> _items;
        private readonly StartupManager _manager;
        private readonly StartupImpactCalculator _calculator;
        private bool _changesMade = false;

        public BatchOperationsWindow(List<StartupItem> items, StartupManager manager)
        {
            InitializeComponent();
            _items = items;
            _manager = manager;
            _calculator = new StartupImpactCalculator();
            
            LoadData();
        }

        private void LoadData()
        {
            // High impact items
            var highImpact = _items
                .Where(i => i.IsEnabled && _calculator.CalculateImpact(i) >= 7)
                .Select(i => $"{i.Name} - Impact: {_calculator.CalculateImpact(i)}/10")
                .ToList();
            HighImpactList.ItemsSource = highImpact;

            // Items for delay
            var delayItems = _items
                .Where(i => i.IsEnabled && i.DelaySeconds == 0)
                .Select(i => $"{i.Name} - Suggested delay: {GetSuggestedDelay(i)}s")
                .ToList();
            DelayList.ItemsSource = delayItems;

            // Duplicates
            var duplicates = _items
                .GroupBy(i => i.Command.ToLower())
                .Where(g => g.Count() > 1)
                .SelectMany(g => g.Select(i => $"{i.Name} ({i.LocationDisplay})"))
                .ToList();
            DuplicatesList.ItemsSource = duplicates;
        }

        private int GetSuggestedDelay(StartupItem item)
        {
            int impact = _calculator.CalculateImpact(item);
            return impact >= 9 ? 60 : impact >= 7 ? 30 : 15;
        }

        private void DisableHighImpactButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = HighImpactList.SelectedItems.Cast<string>().ToList();
            if (selected.Count == 0)
            {
                MessageBox.Show("Please select items to disable.", "No Selection", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Disable {selected.Count} high-impact items?",
                "Confirm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                foreach (var sel in selected)
                {
                    var name = sel.Split(" - ")[0];
                    var item = _items.FirstOrDefault(i => i.Name == name);
                    if (item != null)
                    {
                        _manager.DisableItem(item);
                    }
                }
                _changesMade = true;
                MessageBox.Show($"Disabled {selected.Count} items.", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ApplyDelaysButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Delay application requires Task Scheduler migration.\nThis feature will be fully functional in v1.1.",
                "Feature Preview", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RemoveDuplicatesButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = DuplicatesList.SelectedItems.Cast<string>().ToList();
            if (selected.Count == 0)
            {
                MessageBox.Show("Please select duplicates to remove.", "No Selection",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MessageBox.Show($"Would remove {selected.Count} duplicate entries.\nFull duplicate detection in v1.1.",
                "Feature Preview", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DisableAllButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "⚠️ This will disable ALL startup items!\n\nOnly use this in emergency situations.\n\nContinue?",
                "Emergency Action",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                int count = 0;
                foreach (var item in _items.Where(i => i.IsEnabled))
                {
                    if (_manager.DisableItem(item))
                        count++;
                }
                _changesMade = true;
                MessageBox.Show($"Disabled {count} items.", "Complete",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void EnableAllButton_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;
            foreach (var item in _items.Where(i => !i.IsEnabled))
            {
                if (_manager.EnableItem(item))
                    count++;
            }
            _changesMade = true;
            MessageBox.Show($"Enabled {count} items.", "Complete",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RemoveDisabledButton_Click(object sender, RoutedEventArgs e)
        {
            var disabled = _items.Where(i => !i.IsEnabled).ToList();
            var result = MessageBox.Show(
                $"Permanently remove {disabled.Count} disabled items?",
                "Confirm Removal",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                int count = 0;
                foreach (var item in disabled)
                {
                    if (_manager.RemoveItem(item))
                        count++;
                }
                _changesMade = true;
                MessageBox.Show($"Removed {count} items.", "Complete",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = _changesMade;
            Close();
        }
    }
}
