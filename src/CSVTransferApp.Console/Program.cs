using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using CSVTransferApp.Console.DependencyInjection;
using CSVTransferApp.Infrastructure.Configuration;

namespace CSVTransferApp.Console;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        
        var configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            // Load connection files from working directory
            .AddJsonFile("database-connections.json", optional: true)
            .AddJsonFile($"database-connections.{environment}.json", optional: true)
            .AddJsonFile("sftp-connections.json", optional: true)
            .AddJsonFile($"sftp-connections.{environment}.json", optional: true)
            // Also attempt to load from ./config folder to support repository layout
            .AddJsonFile("config/database-connections.json", optional: true)
            .AddJsonFile($"config/database-connections.{environment}.json", optional: true)
            .AddJsonFile("config/sftp-connections.json", optional: true)
            .AddJsonFile($"config/sftp-connections.{environment}.json", optional: true)
            // Load environment variables and command line last
            .AddEnvironmentVariables()
            .AddEnvironmentVariablePlaceholderResolver(skipUnresolved: false)
            .AddCommandLine(args);

        var configuration = configurationBuilder.Build();

        var services = new ServiceCollection();
        ConfigureServices(services, configuration);
        
        using var serviceProvider = services.BuildServiceProvider();
        var app = serviceProvider.GetRequiredService<Application>();
        
        return await app.RunAsync(args);
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Registra la configurazione
        services.AddSingleton(configuration);

        // Configura il logging
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        services.AddLogging(builder =>
        {
            builder.AddConsole()
                  .AddSerilog(dispose: true)
                  .SetMinimumLevel(LogLevel.Information);
        });

        // Registra tutti i servizi dell'applicazione usando l'estensione
        services.AddCsvTransferServices(configuration);
    }
}
