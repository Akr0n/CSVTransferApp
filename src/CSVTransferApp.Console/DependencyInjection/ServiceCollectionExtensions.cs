using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using CSVTransferApp.Core.Interfaces;
using CSVTransferApp.Services;
using CSVTransferApp.Data.Factories;
using CSVTransferApp.Data.Configuration;
using CSVTransferApp.Infrastructure.Security;

namespace CSVTransferApp.Console.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCsvTransferServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Core services
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        services.AddSingleton<ILoggerService, LoggingService>();
        
        // Data services
        services.AddSingleton<ConnectionFactory>();
        services.AddSingleton<DataAdapterFactory>();
        services.AddSingleton<DatabaseConnectionManager>();
        services.AddSingleton<IDatabaseService, DatabaseService>();
        
        // Business services
        services.AddSingleton<ISftpService, SftpService>();
        services.AddSingleton<ICsvProcessingService, CsvProcessingService>();
        services.AddSingleton<FileHeaderService>();
        services.AddSingleton<JobManagerService>();
        
        // Infrastructure services
        services.AddSingleton<EncryptionService>();
        services.AddSingleton<CredentialManager>();
        
        // Application
        services.AddSingleton<Application>();
        
        return services;
    }
}
