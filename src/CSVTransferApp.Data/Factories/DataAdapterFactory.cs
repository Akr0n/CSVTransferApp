using System.Data;
using CSVTransferApp.Core.Constants;
using CSVTransferApp.Core.Interfaces;
using CSVTransferApp.Data.Services;

namespace CSVTransferApp.Data.Factories;

public class DataAdapterFactory : IDataAdapterFactory
{
    private readonly Dictionary<string, IDatabaseProvider> _providers;
    private readonly ILogger<DataAdapterFactory> _logger;

    public DataAdapterFactory(ILogger<DataAdapterFactory> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _providers = new Dictionary<string, IDatabaseProvider>(StringComparer.OrdinalIgnoreCase)
        {
            { DatabaseProviders.Oracle, new OracleDatabaseProvider() },
            { DatabaseProviders.SqlServer, new SqlServerDatabaseProvider() },
            { DatabaseProviders.PostgreSQL, new PostgreSqlDatabaseProvider() }
        };

        _logger.LogInformation("DataAdapterFactory initialized with providers: {Providers}", 
            string.Join(", ", _providers.Keys));
    }

    public IDbDataAdapter CreateDataAdapter(string providerName, IDbCommand command)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        if (string.IsNullOrEmpty(providerName))
            throw new ArgumentException("Provider name cannot be null or empty", nameof(providerName));

        if (!_providers.TryGetValue(providerName, out var provider))
        {
            var error = $"Database provider '{providerName}' is not supported. Supported providers: {string.Join(", ", _providers.Keys)}";
            _logger.LogError(error);
            throw new NotSupportedException(error);
        }

        try
        {
            var adapter = provider.CreateDataAdapter(command);
            _logger.LogInformation("Created data adapter for provider {Provider}", providerName);
            return adapter;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create data adapter for provider {Provider}", providerName);
            throw new InvalidOperationException($"Failed to create data adapter for provider {providerName}", ex);
        }
    }

    public bool IsProviderSupported(string providerName)
    {
        return !string.IsNullOrEmpty(providerName) && _providers.ContainsKey(providerName);
    }


}
