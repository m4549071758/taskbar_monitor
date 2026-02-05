namespace TaskbarMonitor.Models
{
    public class AppConfig
    {
        public InfluxDbConfig InfluxDb { get; set; } = new();
        public NetworkMonitorConfig NetworkMonitor { get; set; } = new();
        public DisplaySettingsConfig DisplaySettings { get; set; } = new();
    }

    public class InfluxDbConfig
    {
        public string Url { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Org { get; set; } = string.Empty;
        public string Bucket { get; set; } = string.Empty;
        public string Measurement { get; set; } = "environment";
    }

    public class NetworkMonitorConfig
    {
        public string LocalDns { get; set; } = string.Empty;
        public string Gateway { get; set; } = string.Empty;
        public int PingIntervalSeconds { get; set; } = 5;
    }

    public class DisplaySettingsConfig
    {
        public int TargetDisplayIndex { get; set; } = 0;
        public int UiOffsetRight { get; set; } = 10;
    }
}
