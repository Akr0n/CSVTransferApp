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
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .AddEnvironmentVariablePlaceholderResolver(skipUnresolved: false)
            .AddCommandLine(args)
            .Build();

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
