using System.Collections.Concurrent;

// Services/DatabaseService.cs
public class DatabaseService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DatabaseService> _logger;
    private readonly ConcurrentDictionary<string, IDbConnection> _connections;

    public DatabaseService(IConfiguration configuration, ILogger<DatabaseService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _connections = new ConcurrentDictionary<string, IDbConnection>();
    }

    public async Task<DataTable> ExecuteQueryAsync(string connectionName, string query)
    {
        var connection = await GetConnectionAsync(connectionName);
        using var command = connection.CreateCommand();
        command.CommandText = query;

        _logger.LogInformation("Executing query on {Connection}: {Query}",
            connectionName, query);

        var adapter = CreateDataAdapter(connection, command);
        var dataTable = new DataTable();
        adapter.Fill(dataTable);

        return dataTable;
    }

    private async Task<IDbConnection> GetConnectionAsync(string connectionName)
    {
        return _connections.GetOrAdd(connectionName, name =>
        {
            var config = _configuration.GetSection($"DatabaseConnections:{name}");
            var connectionString = config["ConnectionString"];
            var provider = config["Provider"];

            return provider switch
            {
                "Oracle.EntityFrameworkCore" => new OracleConnection(connectionString),
                "Microsoft.EntityFrameworkCore.SqlServer" => new SqlConnection(connectionString),
                "Npgsql.EntityFrameworkCore.PostgreSQL" => new NpgsqlConnection(connectionString),
                _ => throw new NotSupportedException($"Provider {provider} not supported")
            };
        });
    }
}
