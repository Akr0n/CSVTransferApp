using System.Data;
using CSVTransferApp.Core.Models;
using CSVTransferApp.Core.Constants;
using CSVTransferApp.Data.Services;

namespace CSVTransferApp.Data.Factories;

public class ConnectionFactory
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

        //var connection = provider.CreateConnection(config);
        //connection.ConnectionTimeout = config.ConnectionTimeout;
        string connectionString = $"...;Connection Timeout={config.ConnectionTimeout};...";
        var connection = new SqlConnection(connectionString); // oppure NpgsqlConnection, OracleConnection ecc.

        return connection;
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
