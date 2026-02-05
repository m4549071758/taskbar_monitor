using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Threading;
using TaskbarMonitor.Models;
using TaskbarMonitor.Services;

namespace TaskbarMonitor.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IInfluxDbService _influxService;
        private readonly INetworkMonitorService _networkService;
        private readonly DispatcherTimer _timer;

        private string _temperature = "-";
        public string Temperature
        {
            get => _temperature;
            set
            {
                _temperature = value;
                OnPropertyChanged();
            }
        }

        private string _humidity = "-";
        public string Humidity
        {
            get => _humidity;
            set
            {
                _humidity = value;
                OnPropertyChanged();
            }
        }

        private string _co2 = "-";
        public string Co2
        {
            get => _co2;
            set
            {
                _co2 = value;
                OnPropertyChanged();
            }
        }

        private string _dnsPing = "-";
        public string DnsPing
        {
            get => _dnsPing;
            set
            {
                _dnsPing = value;
                OnPropertyChanged();
            }
        }

        private string _gatewayPing = "-";
        public string GatewayPing
        {
            get => _gatewayPing;
            set
            {
                _gatewayPing = value;
                OnPropertyChanged();
            }
        }

        private string _lastUpdated = "";
        public string LastUpdated
        {
            get => _lastUpdated;
            set 
            {
                _lastUpdated = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            // Initialize services properly using App.Config
            // In a real DI scenario, these would be injected.
            _influxService = new InfluxDbService(App.Config);
            _networkService = new NetworkMonitorService(App.Config);

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(5); // Update every 5 seconds for Ping, maybe slower for Influx
            _timer.Tick += async (s, e) => await UpdateDataAsync();
            _timer.Start();

            // Initial load
            _ = UpdateDataAsync();
        }

        private int _tickCounter = 0;

        private async Task UpdateDataAsync()
        {
            try
            {
                // Ping every 5 seconds
                var (dns, gateway) = await _networkService.MeasureLatencyAsync();
                DnsPing = dns >= 0 ? $"{dns:F2}ms" : "-";
                GatewayPing = gateway >= 0 ? $"{gateway:F2}ms" : "-";
                
                Console.WriteLine($"[ViewModel] Ping更新: DNS={DnsPing}, GW={GatewayPing}");

                // Influx every 60 seconds (12 ticks of 5s)
                if (_tickCounter % 12 == 0)
                {
                    Console.WriteLine("[ViewModel] InfluxDBデータ取得開始...");
                    var data = await _influxService.GetLatestDataAsync();
                    if (data != null)
                    {
                        Temperature = data.Temperature.HasValue ? $"{data.Temperature.Value:F1}°C" : "-";
                        Humidity = data.Humidity.HasValue ? $"{data.Humidity.Value:F0}%" : "-";
                        Co2 = data.Co2.HasValue ? $"{data.Co2.Value}ppm" : "-";
                        LastUpdated = DateTime.Now.ToString("HH:mm");
                        Console.WriteLine($"[ViewModel] Influxデータ更新: 気温={Temperature}, 湿度={Humidity}, CO2={Co2}");
                    }
                    else
                    {
                         Console.WriteLine("[ViewModel] Influxデータはnullでした。");
                    }
                }
                
                _tickCounter++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ViewModel] 更新ループでエラー: {ex}");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
