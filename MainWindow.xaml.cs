using Microsoft.Win32;
using StartupMaster.Models;
using StartupMaster.Services;
using StartupMaster.Utils;
using StartupMaster.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Text.Json;

namespace StartupMaster
{
    public partial class MainWindow : Window
    {
        private readonly StartupManager _startupManager;
        private readonly BackupManager _backupManager;
        private readonly PerformanceAnalyzer _analyzer;
        private ObservableCollection<StartupItem> _allItems;
        private ObservableCollection<StartupItem> _filteredItems;
        private bool _hasUnsavedChanges;

        public MainWindow()
        {
            InitializeComponent();
            _startupManager = new StartupManager();
            _backupManager = new BackupManager();
            _analyzer = new PerformanceAnalyzer();
            _allItems = new ObservableCollection<StartupItem>();
            _filteredItems = new ObservableCollection<StartupItem>();
            StartupItemsGrid.ItemsSource = _filteredItems;
            
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // F5 - Refresh
            if (e.Key == System.Windows.Input.Key.F5)
            {
                LoadStartupItems();
                e.Handled = true;
            }
            
            // Ctrl shortcuts
            if (e.KeyboardDevice.Modifiers == System.Windows.Input.ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.N: // Add New
                        AddButton_Click(null, null);
                        e.Handled = true;
                        break;
                    case System.Windows.Input.Key.E: // Export
                        ExportButton_Click(null, null);
                        e.Handled = true;
                        break;
                    case System.Windows.Input.Key.I: // Import
                        ImportButton_Click(null, null);
                        e.Handled = true;
                        break;
                    case System.Windows.Input.Key.F: // Search
                        SearchBox.Focus();
                        e.Handled = true;
                        break;
                    case System.Windows.Input.Key.T: // Statistics
                        StatisticsButton_Click(null, null);
                        e.Handled = true;
                        break;
                    case System.Windows.Input.Key.B: // Batch
                        BatchButton_Click(null, null);
                        e.Handled = true;
                        break;
                }
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStartupItems();
            CreateInitialBackup();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_hasUnsavedChanges)
            {
                var result = MessageBox.Show(
                    "You have made changes. Create a backup before closing?",
                    "Unsaved Changes",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _backupManager.CreateAutoBackup(_allItems.ToList(), "OnClose");
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void CreateInitialBackup()
        {
            try
            {
                _backupManager.CreateAutoBackup(_allItems.ToList(), "OnLoad");
            }
            catch { }
        }

        private void LoadStartupItems()
        {
            try
            {
                StatusText.Text = "Loading startup items...";
                
                var items = _startupManager.GetAllItems();
                _allItems.Clear();
                
                foreach (var item in items)
                {
                    _allItems.Add(item);
                }

                ApplyFilters();
                UpdateItemCount();
                StatusText.Text = "Ready";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading startup items: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "Error loading items";
            }
        }

        private void ApplyFilters()
        {
            _filteredItems.Clear();
            
            var searchText = SearchBox.Text?.ToLower() ?? "";
            var filterIndex = FilterComboBox.SelectedIndex;

            IEnumerable<StartupItem> filtered = _allItems;

            // Apply location/status filter
            filtered = filterIndex switch
            {
                1 => filtered.Where(i => i.IsEnabled),
                2 => filtered.Where(i => !i.IsEnabled),
                3 => filtered.Where(i => i.Location == StartupLocation.RegistryCurrentUser || 
                                        i.Location == StartupLocation.RegistryLocalMachine),
                4 => filtered.Where(i => i.Location == StartupLocation.StartupFolder),
                5 => filtered.Where(i => i.Location == StartupLocation.TaskScheduler),
                6 => filtered.Where(i => i.Location == StartupLocation.Service),
                _ => filtered
            };

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filtered = filtered.Where(i => 
                    i.Name.ToLower().Contains(searchText) ||
                    i.Command.ToLower().Contains(searchText));
            }

            foreach (var item in filtered)
            {
                _filteredItems.Add(item);
            }
        }

        private void UpdateItemCount()
        {
            ItemCountText.Text = $"{_filteredItems.Count} of {_allItems.Count} items";
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadStartupItems();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddEditDialog();
            if (dialog.ShowDialog() == true)
            {
                var item = dialog.StartupItem;
                
                if (_startupManager.AddItem(item))
                {
                    MessageBox.Show($"Successfully added '{item.Name}' to startup.", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadStartupItems();
                }
                else
                {
                    MessageBox.Show($"Failed to add '{item.Name}' to startup.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is StartupItem item)
            {
                var dialog = new AddEditDialog(item);
                if (dialog.ShowDialog() == true)
                {
                    if (item.Location == StartupLocation.TaskScheduler)
                    {
                        if (_startupManager.UpdateDelay(item))
                        {
                            MessageBox.Show("Delay updated successfully.", "Success",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to update delay.", "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    LoadStartupItems();
                }
            }
        }

        private void EnableButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is StartupItem item)
            {
                if (_startupManager.EnableItem(item))
                {
                    StatusText.Text = $"Enabled '{item.Name}'";
                    _hasUnsavedChanges = true;
                    LoadStartupItems();
                }
                else
                {
                    MessageBox.Show($"Failed to enable '{item.Name}'.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DisableButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is StartupItem item)
            {
                // Block critical items
                if (item.IsCritical)
                {
                    MessageBox.Show(
                        $"Cannot disable '{item.Name}'.\n\nReason: {item.CriticalReason}\n\nThis item is essential for Windows to function properly.",
                        "Critical System Component",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                _backupManager.CreateAutoBackup(_allItems.ToList(), $"BeforeDisable_{item.Name}");
                
                if (_startupManager.DisableItem(item))
                {
                    StatusText.Text = $"Disabled '{item.Name}'";
                    _hasUnsavedChanges = true;
                    LoadStartupItems();
                }
                else
                {
                    MessageBox.Show($"Failed to disable '{item.Name}'.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is StartupItem item)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to remove '{item.Name}' from startup?\n\nA backup will be created automatically.",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _backupManager.CreateAutoBackup(_allItems.ToList(), $"BeforeDelete_{item.Name}");
                    
                    if (_startupManager.RemoveItem(item))
                    {
                        StatusText.Text = $"Removed '{item.Name}'";
                        _hasUnsavedChanges = true;
                        LoadStartupItems();
                    }
                    else
                    {
                        MessageBox.Show($"Failed to remove '{item.Name}'.", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var report = _analyzer.AnalyzeStartupItems(_allItems.ToList());
                var textReport = _analyzer.GenerateTextReport(report);

                var dialog = new Window
                {
                    Title = "Startup Performance Analysis",
                    Width = 700,
                    Height = 600,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = this,
                    Content = new ScrollViewer
                    {
                        Content = new TextBox
                        {
                            Text = textReport,
                            IsReadOnly = true,
                            FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                            Padding = new Thickness(10),
                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                        }
                    }
                };

                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Analysis failed: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_allItems != null)
            {
                ApplyFilters();
                UpdateItemCount();
            }
        }

        private void StatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            var statsWindow = new StatisticsWindow(_allItems.ToList())
            {
                Owner = this
            };
            statsWindow.ShowDialog();
        }

        private void BatchButton_Click(object sender, RoutedEventArgs e)
        {
            var batchWindow = new BatchOperationsWindow(_allItems.ToList(), _startupManager)
            {
                Owner = this
            };
            
            if (batchWindow.ShowDialog() == true)
            {
                _hasUnsavedChanges = true;
                LoadStartupItems();
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_allItems != null)
            {
                ApplyFilters();
                UpdateItemCount();
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                DefaultExt = "json",
                FileName = $"StartupItems_{DateTime.Now:yyyyMMdd_HHmmss}.json"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var json = JsonSerializer.Serialize(_allItems, new JsonSerializerOptions 
                    { 
                        WriteIndented = true 
                    });
                    File.WriteAllText(dialog.FileName, json);
                    MessageBox.Show("Export successful!", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Export failed: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                DefaultExt = "json"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var json = File.ReadAllText(dialog.FileName);
                    var items = JsonSerializer.Deserialize<List<StartupItem>>(json);
                    
                    if (items != null && items.Count > 0)
                    {
                        var result = MessageBox.Show(
                            $"Import {items.Count} items?",
                            "Confirm Import",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            int success = 0;
                            foreach (var item in items)
                            {
                                if (_startupManager.AddItem(item))
                                    success++;
                            }

                            MessageBox.Show($"Imported {success} of {items.Count} items.", "Import Complete",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadStartupItems();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Import failed: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
