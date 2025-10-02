using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using CSVTransferApp.Core.Interfaces;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CSVTransferApp.Data.Services;

public class DatabaseService : IDatabaseService
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
        if (string.IsNullOrEmpty(connectionName))
            throw new ArgumentException("Connection name cannot be null or empty", nameof(connectionName));
            
        if (string.IsNullOrEmpty(query))
            throw new ArgumentException("Query cannot be null or empty", nameof(query));

        var section = _configuration.GetSection($"DatabaseConnections:{connectionName}");
        string? connectionString = section["ConnectionString"];
        string? providerName = section["Provider"];

        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException($"Connection string not found for {connectionName}");
            
        if (string.IsNullOrEmpty(providerName))
            throw new InvalidOperationException($"Provider not specified for {connectionName}");

        var connection = await GetConnectionAsync(connectionName);
        using var command = connection.CreateCommand();
        command.CommandText = query;

        _logger.LogInformation("Executing query on {Connection}: {Query}",
            connectionName, query);

        DbDataAdapter adapter = providerName switch
        {
            "Oracle.ManagedDataAccess.Client" => new Oracle.ManagedDataAccess.Client.OracleDataAdapter((Oracle.ManagedDataAccess.Client.OracleCommand)command),
            "Microsoft.Data.SqlClient" => new Microsoft.Data.SqlClient.SqlDataAdapter((Microsoft.Data.SqlClient.SqlCommand)command),
            "Npgsql" => new Npgsql.NpgsqlDataAdapter((Npgsql.NpgsqlCommand)command),
            _ => throw new NotSupportedException($"Provider {providerName} not supported for data adapter")
        };
            
        var dataTable = new DataTable();
        adapter.Fill(dataTable);

        return dataTable;
    }

    public async Task<bool> TestConnectionAsync(string connectionName)
    {
        if (string.IsNullOrEmpty(connectionName))
            throw new ArgumentException("Connection name cannot be null or empty", nameof(connectionName));

        try
        {
            var connection = await GetConnectionAsync(connectionName);
            return connection.State == ConnectionState.Open;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to test connection {ConnectionName}", connectionName);
            return false;
        }
    }

    public async Task<List<string>> GetTablesAsync(string connectionName)
    {
        if (string.IsNullOrEmpty(connectionName))
            throw new ArgumentException("Connection name cannot be null or empty", nameof(connectionName));

        var connection = await GetConnectionAsync(connectionName);
        var tables = new List<string>();

        using var command = connection.CreateCommand();
        command.CommandText = GetTableListQuery(connection);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            tables.Add(reader.GetString(0));
        }

        return tables;
    }

    public async Task<List<string>> GetColumnsAsync(string connectionName, string tableName)
    {
        if (string.IsNullOrEmpty(connectionName))
            throw new ArgumentException("Connection name cannot be null or empty", nameof(connectionName));
            
        if (string.IsNullOrEmpty(tableName))
            throw new ArgumentException("Table name cannot be null or empty", nameof(tableName));

        var connection = await GetConnectionAsync(connectionName);
        var columns = new List<string>();

        using var command = connection.CreateCommand();
        command.CommandText = GetColumnListQuery(connection, tableName);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            columns.Add(reader.GetString(0));
        }

        return columns;
    }

    private string GetTableListQuery(IDbConnection connection)
    {
        return connection switch
        {
            SqlConnection => "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'",
            OracleConnection => "SELECT TABLE_NAME FROM USER_TABLES",
            NpgsqlConnection => "SELECT tablename FROM pg_catalog.pg_tables WHERE schemaname NOT IN ('pg_catalog', 'information_schema')",
            _ => throw new NotSupportedException($"Unsupported database type: {connection.GetType().Name}")
        };
    }

    private string GetColumnListQuery(IDbConnection connection, string tableName)
    {
        return connection switch
        {
            SqlConnection => $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'",
            OracleConnection => $"SELECT COLUMN_NAME FROM USER_TAB_COLUMNS WHERE TABLE_NAME = '{tableName.ToUpper()}'",
            NpgsqlConnection => $"SELECT column_name FROM information_schema.columns WHERE table_name = '{tableName.ToLower()}'",
            _ => throw new NotSupportedException($"Unsupported database type: {connection.GetType().Name}")
        };
    }

    public void Dispose()
    {
        foreach (var connection in _connections.Values)
        {
            try
            {
                connection.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing connection");
            }
        }

        _connections.Clear();
        GC.SuppressFinalize(this);
    }

    private Task<IDbConnection> GetConnectionAsync(string connectionName)
    {
        var connection = _connections.GetOrAdd(connectionName, name =>
        {
            var config = _configuration.GetSection($"DatabaseConnections:{name}");
            var connectionString = config["ConnectionString"];
            var provider = config["Provider"];

            return provider switch
            {
                "Oracle.ManagedDataAccess.Client" => new OracleConnection(connectionString),
                "Microsoft.Data.SqlClient" => new SqlConnection(connectionString),
                "Npgsql" => new NpgsqlConnection(connectionString),
                _ => throw new NotSupportedException($"Provider {provider} not supported")
            };
        });

        return Task.FromResult(connection);
    }
}
