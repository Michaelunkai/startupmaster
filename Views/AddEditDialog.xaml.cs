using Microsoft.Win32;
using StartupMaster.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace StartupMaster.Views
{
    public partial class AddEditDialog : Window
    {
        public StartupItem StartupItem { get; private set; }
        private readonly bool _isEditMode;

        public AddEditDialog(StartupItem existingItem = null)
        {
            InitializeComponent();
            
            _isEditMode = existingItem != null;
            
            if (_isEditMode)
            {
                Title = "Edit Startup Item";
                StartupItem = existingItem;
                LoadItemData();
            }
            else
            {
                Title = "Add New Startup Item";
                StartupItem = new StartupItem
                {
                    Location = StartupLocation.RegistryCurrentUser,
                    IsEnabled = true,
                    DelaySeconds = 0
                };
            }

            DelaySlider.ValueChanged += DelaySlider_ValueChanged;
        }

        private void LoadItemData()
        {
            NameTextBox.Text = StartupItem.Name;
            CommandTextBox.Text = StartupItem.Command;
            ArgumentsTextBox.Text = StartupItem.Arguments;

            // Set location
            foreach (ComboBoxItem item in LocationComboBox.Items)
            {
                if (item.Tag.ToString() == StartupItem.Location.ToString())
                {
                    LocationComboBox.SelectedItem = item;
                    break;
                }
            }

            DelaySlider.Value = StartupItem.DelaySeconds;
        }

        private void LocationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LocationComboBox.SelectedItem is ComboBoxItem item)
            {
                var location = item.Tag.ToString();
                
                // Show delay panel only for Task Scheduler
                DelayPanel.Visibility = location == "TaskScheduler" 
                    ? Visibility.Visible 
                    : Visibility.Collapsed;
            }
        }

        private void DelaySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (DelayValueText != null)
            {
                DelayValueText.Text = $"{(int)DelaySlider.Value}s";
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Executable files (*.exe)|*.exe|Batch files (*.bat;*.cmd)|*.bat;*.cmd|All files (*.*)|*.*",
                Title = "Select Program"
            };

            if (dialog.ShowDialog() == true)
            {
                CommandTextBox.Text = dialog.FileName;
                
                // Auto-fill name if empty
                if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                {
                    NameTextBox.Text = System.IO.Path.GetFileNameWithoutExtension(dialog.FileName);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Please enter a name for the startup item.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                NameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(CommandTextBox.Text))
            {
                MessageBox.Show("Please enter a command/path.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                CommandTextBox.Focus();
                return;
            }

            // Update startup item
            StartupItem.Name = NameTextBox.Text.Trim();
            StartupItem.Command = CommandTextBox.Text.Trim();
            StartupItem.Arguments = ArgumentsTextBox.Text?.Trim() ?? string.Empty;
            
            if (LocationComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                StartupItem.Location = Enum.Parse<StartupLocation>(selectedItem.Tag.ToString());
                
                // Set registry key for registry items
                if (StartupItem.Location == StartupLocation.RegistryCurrentUser ||
                    StartupItem.Location == StartupLocation.RegistryLocalMachine)
                {
                    StartupItem.RegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
                    StartupItem.RegistryValueName = StartupItem.Name;
                }
            }

            StartupItem.DelaySeconds = (int)DelaySlider.Value;

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
