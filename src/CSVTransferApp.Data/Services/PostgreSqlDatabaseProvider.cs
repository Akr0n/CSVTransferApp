using System.Data;
using Npgsql;
using CSVTransferApp.Core.Models;

namespace CSVTransferApp.Data.Services;

public class PostgreSqlDatabaseProvider : IDatabaseProvider
{
    public string ProviderName => "PostgreSQL";

    public IDbConnection CreateConnection(DatabaseConnectionConfig config)
    {
        return new NpgsqlConnection(config.ConnectionString);
    }

    public IDbDataAdapter CreateDataAdapter(IDbCommand command)
    {
        return new NpgsqlDataAdapter((NpgsqlCommand)command);
    }

    public async Task<List<string>> GetTablesAsync(IDbConnection connection)
    {
        var tables = new List<string>();
        const string query = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' AND table_type = 'BASE TABLE' ORDER BY table_name";
        
        using var command = connection.CreateCommand();
        command.CommandText = query;
        
        using var reader = await ((NpgsqlCommand)command).ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tables.Add(reader.GetString(0));
        }
        
        return tables;
    }

    public async Task<List<string>> GetColumnsAsync(IDbConnection connection, string tableName)
    {
        var columns = new List<string>();
        const string query = "SELECT column_name FROM information_schema.columns WHERE table_name = @tableName ORDER BY ordinal_position";
        
        using var command = (NpgsqlCommand)connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("tableName", tableName.ToLower());
        
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            columns.Add(reader.GetString(0));
        }
        
        return columns;
    }
}
