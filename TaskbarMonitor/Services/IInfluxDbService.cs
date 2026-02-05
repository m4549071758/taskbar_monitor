using System.Threading.Tasks;
using TaskbarMonitor.Models;

namespace TaskbarMonitor.Services
{
    public interface IInfluxDbService
    {
        Task<EnvironmentData?> GetLatestDataAsync();
    }
}
