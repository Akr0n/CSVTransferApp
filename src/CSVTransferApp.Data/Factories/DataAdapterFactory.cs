using System.Data;
using CSVTransferApp.Core.Constants;
using CSVTransferApp.Data.Services;

namespace CSVTransferApp.Data.Factories;

public class DataAdapterFactory
{
    private readonly Dictionary<string, IDatabaseProvider> _providers;

    public DataAdapterFactory()
    {
        _providers = new Dictionary<string, IDatabaseProvider>
        {
            { DatabaseProviders.Oracle, new OracleDatabaseProvider() },
            { DatabaseProviders.SqlServer, new SqlServerDatabaseProvider() },
            { DatabaseProviders.PostgreSQL, new PostgreSqlDatabaseProvider() }
        };
    }

    public IDbDataAdapter CreateAdapter(IDbCommand command, string providerName)
    {
        if (!_providers.TryGetValue(providerName, out var provider))
        {
            throw new NotSupportedException($"Database provider '{providerName}' is not supported");
        }

        return provider.CreateDataAdapter(command);
    }
}
