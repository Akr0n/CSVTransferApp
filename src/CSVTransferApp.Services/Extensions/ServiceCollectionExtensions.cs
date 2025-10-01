using Microsoft.Extensions.DependencyInjection;
using CSVTransferApp.Core.Interfaces;

namespace CSVTransferApp.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Registrazione dei servizi applicativi
        services.AddScoped<ICsvProcessingService, CsvProcessingService>();
        services.AddScoped<IFileHeaderService, FileHeaderService>();
        services.AddScoped<IJobManagerService, JobManagerService>();
        services.AddSingleton<ILoggingService, LoggingService>();
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        services.AddScoped<ISftpService, SftpService>();
        
        return services;
    }
}