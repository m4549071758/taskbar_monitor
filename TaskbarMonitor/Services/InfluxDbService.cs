using System;
using System.Linq;
using System.Threading.Tasks;
using InfluxDB.Client;
using TaskbarMonitor.Models;

namespace TaskbarMonitor.Services
{
    public class InfluxDbService : IInfluxDbService, IDisposable
    {
        private readonly InfluxDBClient _client;
        private readonly string _org;
        private readonly string _bucket;
        private readonly string _measurement;

        public InfluxDbService(AppConfig config)
        {
            _client = new InfluxDBClient(config.InfluxDb.Url, config.InfluxDb.Token);
            _org = config.InfluxDb.Org;
            _bucket = config.InfluxDb.Bucket;
            _measurement = config.InfluxDb.Measurement;
        }

        public async Task<EnvironmentData?> GetLatestDataAsync()
        {
            try
            {
                Console.WriteLine($"[InfluxDB] Fetching data from bucket: {_bucket}...");
                // User provided specific queries for Grafana:
                // Bucket: "environment", Measurement: "environment", Fields: "temperature", "humidity", "co2"
                // We optimize by combining them into a single query for efficiency.
                var query = $@"from(bucket: ""{_bucket}"")
                    |> range(start: -1h)
                    |> filter(fn: (r) => r[""_measurement""] == ""{_measurement}"")
                    |> filter(fn: (r) => r[""_field""] == ""temperature"" or r[""_field""] == ""humidity"" or r[""_field""] == ""co2"")
                    |> last()";

                var tables = await _client.GetQueryApi().QueryAsync(query, _org);

                if (tables == null || !tables.Any()) 
                {
                    Console.WriteLine("[InfluxDB] No data found.");
                    return null;
                }

                var data = new EnvironmentData();
                data.Timestamp = DateTime.Now; // Or use the record timestamp

                foreach (var record in tables.SelectMany(table => table.Records))
                {
                    var field = record.GetValueByKey("_field")?.ToString();
                    var value = record.GetValue();
                    
                    Console.WriteLine($"[InfluxDB] Record: Field={field}, Value={value}");

                    if (field == "temperature" && value is double temp)
                    {
                        data.Temperature = temp;
                    }
                    else if (field == "humidity" && value is double hum)
                    {
                        data.Humidity = hum;
                    }
                    else if (field == "co2" && (value is double || value is long || value is int))
                    {
                        data.Co2 = Convert.ToInt32(value);
                    }
                }

                if (data.Temperature.HasValue || data.Humidity.HasValue || data.Co2.HasValue)
                {
                     return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[InfluxDB] Error: {ex.Message}");
                return null;
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
