using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using TaskbarMonitor.Models;

namespace TaskbarMonitor.Services
{
    public class NetworkMonitorService : INetworkMonitorService
    {
        private readonly AppConfig _config;

        public NetworkMonitorService(AppConfig config)
        {
            _config = config;
        }

        public async Task<(double DnsPing, double GatewayPing)> MeasureLatencyAsync()
        {
            var t1 = PingHost(_config.NetworkMonitor.LocalDns);
            var t2 = PingHost(_config.NetworkMonitor.Gateway);

            await Task.WhenAll(t1, t2);

            return (t1.Result, t2.Result);
        }

        private async Task<double> PingHost(string host)
        {
            if (string.IsNullOrWhiteSpace(host)) return -1;
            using var ping = new Ping();
            try
            {
                // To get sub-millisecond precision, we measure the elapsed time ourselves.
                // Note: This includes .NET overhead, so it's "Application-to-Target and back" time.
                var sw = System.Diagnostics.Stopwatch.StartNew();
                var reply = await ping.SendPingAsync(host, 1000); // 1s timeout
                sw.Stop();

                if (reply.Status == IPStatus.Success)
                {
                    double elapsedMs = sw.Elapsed.TotalMilliseconds;
                    Console.WriteLine($"[Ping] {host}: {elapsedMs:F2}ms (RTT: {reply.RoundtripTime}ms)");
                    return elapsedMs;
                }
                else
                {
                    Console.WriteLine($"[Ping] {host}: Failed ({reply.Status})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ping] {host}: Error {ex.Message}");
            }
            return -1; // Error or Timeout
        }
    }
}
