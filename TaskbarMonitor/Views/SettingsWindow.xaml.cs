using System;
using System.Windows;
using TaskbarMonitor.ViewModels;

namespace TaskbarMonitor.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(SettingsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            
            // Allow ViewModel to close the window
            viewModel.RequestClose += () => this.Close();
        }
    }
}
