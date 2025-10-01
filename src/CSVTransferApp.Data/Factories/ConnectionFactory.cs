using System.Data;
using System.Data.SqlClient;
using CSVTransferApp.Core.Models;
using CSVTransferApp.Core.Constants;
using CSVTransferApp.Core.Interfaces;
using CSVTransferApp.Data.Services;

namespace CSVTransferApp.Data.Factories;

public class ConnectionFactory : IConnectionFactory
{
    private readonly Dictionary<string, IDatabaseProvider> _providers;

    public ConnectionFactory()
    {
        _providers = new Dictionary<string, IDatabaseProvider>
        {
            { DatabaseProviders.Oracle, new OracleDatabaseProvider() },
            { DatabaseProviders.SqlServer, new SqlServerDatabaseProvider() },
            { DatabaseProviders.PostgreSQL, new PostgreSqlDatabaseProvider() }
        };
    }

    public IDbConnection CreateConnection(DatabaseConnectionConfig config)
    {
        if (!_providers.TryGetValue(config.Provider, out var provider))
        {
            throw new NotSupportedException($"Database provider '{config.Provider}' is not supported");
        }

        return provider.CreateConnection(config);
    }

    public IDbConnection CreateConnection(string connectionString, string providerName)
    {
        if (!_providers.TryGetValue(providerName, out var provider))
        {
            throw new NotSupportedException($"Database provider '{providerName}' is not supported");
        }

        return provider.CreateConnection(new DatabaseConnectionConfig
        {
            ConnectionString = connectionString,
            Provider = providerName
        });
    }

    public bool IsProviderSupported(string providerName)
    {
        return !string.IsNullOrEmpty(providerName) && _providers.ContainsKey(providerName);
    }

    public IDatabaseProvider GetProvider(string providerName)
    {
        if (!_providers.TryGetValue(providerName, out var provider))
        {
            throw new NotSupportedException($"Database provider '{providerName}' is not supported");
        }
        
        return provider;
    }
}
