using Microsoft.Extensions.DependencyInjection;

namespace CSVTransferApp.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        // Registrazione dei servizi di accesso ai dati
        // TODO: Aggiungere DatabaseService, Repository, etc.
        return services;
    }
}