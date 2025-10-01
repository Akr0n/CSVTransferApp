using Microsoft.Extensions.DependencyInjection;

namespace CSVTransferApp.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // Qui registriamo i servizi del layer Core
        return services;
    }
}