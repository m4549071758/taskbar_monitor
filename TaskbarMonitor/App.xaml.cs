using System;
using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using TaskbarMonitor.Models;

namespace TaskbarMonitor;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    public static AppConfig Config { get; private set; } = new();

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var exePath = AppContext.BaseDirectory;
        var userSettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TaskbarMonitor", "appsettings.json");

        var builder = new ConfigurationBuilder()
            .SetBasePath(exePath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);

        if (File.Exists(userSettingsPath))
        {
            builder.AddJsonFile(userSettingsPath, optional: true, reloadOnChange: true);
        }

        var configuration = builder.Build();
        Config = new AppConfig();
        configuration.Bind(Config);
    }
}

