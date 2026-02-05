using System.Threading.Tasks;

namespace TaskbarMonitor.Services
{
    public interface INetworkMonitorService
    {
        Task<(double DnsPing, double GatewayPing)> MeasureLatencyAsync();
    }
}
