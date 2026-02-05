using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Forms; // For Screen
using TaskbarMonitor.Models;
using TaskbarMonitor.Services;
using System.Threading.Tasks;

namespace TaskbarMonitor.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private readonly AppConfig _config;
        private readonly ConfigService _configService;

        public event PropertyChangedEventHandler? PropertyChanged;
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        // Bindable Properties
        public ObservableCollection<DisplayOption> AvailableDisplays { get; } = new();
        
        private DisplayOption _selectedDisplay = null!;
        public DisplayOption SelectedDisplay
        {
            get => _selectedDisplay;
            set { _selectedDisplay = value; OnPropertyChanged(); }
        }

        public string InfluxUrl { get; set; }
        public string InfluxToken { get; set; }
        public string InfluxOrg { get; set; }
        public string InfluxBucket { get; set; }
        public string InfluxMeasurement { get; set; }

        public string NetworkLocalDns { get; set; }
        public string NetworkGateway { get; set; }

        public int UiOffsetRight { get; set; }

        // Actions to close window
        public Action? RequestClose { get; set; }

        public SettingsViewModel(AppConfig config)
        {
            _config = config;
            _configService = new ConfigService();

            // Initialize from config
            InfluxUrl = config.InfluxDb.Url;
            InfluxToken = config.InfluxDb.Token;
            InfluxOrg = config.InfluxDb.Org;
            InfluxBucket = config.InfluxDb.Bucket;
            InfluxMeasurement = config.InfluxDb.Measurement;

            NetworkLocalDns = config.NetworkMonitor.LocalDns;
            NetworkGateway = config.NetworkMonitor.Gateway;

            UiOffsetRight = config.DisplaySettings.UiOffsetRight;

            // Load Displays
            LoadDisplays();

            SaveCommand = new RelayCommand(async () => await SaveAsync());
            CancelCommand = new RelayCommand(() => RequestClose?.Invoke());
        }

        private void LoadDisplays()
        {
            var screens = Screen.AllScreens;
            for (int i = 0; i < screens.Length; i++)
            {
                var screen = screens[i];
                var option = new DisplayOption
                {
                    Index = i,
                    Name = $"Display {i + 1}: {screen.Bounds.Width}x{screen.Bounds.Height} {(screen.Primary ? "[Primary]" : "")}"
                };
                AvailableDisplays.Add(option);

                if (i == _config.DisplaySettings.TargetDisplayIndex)
                {
                    SelectedDisplay = option;
                }
            }

            if (SelectedDisplay == null && AvailableDisplays.Any())
            {
                SelectedDisplay = AvailableDisplays.First();
            }
        }

        private async Task SaveAsync()
        {
            // Update config object
            _config.InfluxDb.Url = InfluxUrl;
            _config.InfluxDb.Token = InfluxToken;
            _config.InfluxDb.Org = InfluxOrg;
            _config.InfluxDb.Bucket = InfluxBucket;
            _config.InfluxDb.Measurement = InfluxMeasurement;

            _config.NetworkMonitor.LocalDns = NetworkLocalDns;
            _config.NetworkMonitor.Gateway = NetworkGateway;

            _config.DisplaySettings.TargetDisplayIndex = SelectedDisplay?.Index ?? 0;
            _config.DisplaySettings.UiOffsetRight = UiOffsetRight;

            // Save to file
            await _configService.SaveConfigAsync(_config);

            // Restart Application
            // Use WinForms restart for simplicity as it handles process restart
            System.Windows.Forms.Application.Restart();
            System.Windows.Application.Current.Shutdown();
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class DisplayOption
    {
        public int Index { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        public RelayCommand(Action execute) => _execute = execute;
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => _execute();
        
        // Fix for CS0067: The event is never used
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
