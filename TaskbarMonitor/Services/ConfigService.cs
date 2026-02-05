using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using TaskbarMonitor.Models;

namespace TaskbarMonitor.Services
{
    public class ConfigService
    {
        private const string ConfigFileName = "appsettings.json";

        public async Task SaveConfigAsync(AppConfig config)
        {
            var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TaskbarMonitor");
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }
            var filePath = Path.Combine(appDataPath, "appsettings.json");
            
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(config, options);
            await File.WriteAllTextAsync(filePath, json);
        }
    }
}
