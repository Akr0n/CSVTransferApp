using System.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using CSVTransferApp.Core.Interfaces;
using CSVTransferApp.Services;
using CSVTransferApp.Data.Factories;
using CSVTransferApp.Data.Configuration;
using CSVTransferApp.Data.Services;
using CSVTransferApp.Infrastructure.Security;
using CSVTransferApp.Infrastructure.Health;
using CSVTransferApp.Console.Parsers;

namespace CSVTransferApp.Console.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCsvTransferServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuration
        services.AddSingleton(configuration);
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        services.AddSingleton<ILoggerService, LoggingService>();
        
        // Infrastructure services
        services.AddSingleton<IEncryptionService, EncryptionService>();
        services.AddSingleton<ICredentialManager, CredentialManager>();
        
        // Data services
        services.AddScoped<IConnectionFactory, ConnectionFactory>();
        services.AddScoped<DataAdapterFactory>();
        services.AddScoped<DatabaseConnectionManager>();
        services.AddScoped<IDatabaseService, DatabaseService>();
        
        // Business services
        services.AddTransient<ISftpService, SftpService>();
        services.AddTransient<ICsvProcessingService, CsvProcessingService>();
        services.AddSingleton<FileHeaderService>();
        services.AddSingleton<JobManagerService>();
        
        // Console services
        services.AddSingleton<ICommandLineParser, CommandLineParser>();

    // Health check service - writes heartbeat to logs/health.txt
    services.AddSingleton<IHealthCheckService, FileHealthCheckService>();
        
        // Application
        services.AddSingleton<Application>();
        
        return services;
    }
}
