using System.Data;
using Microsoft.Data.SqlClient;
using CSVTransferApp.Core.Models;

namespace CSVTransferApp.Data.Services;

public class SqlServerDatabaseProvider : IDatabaseProvider
{
    public string ProviderName => "SqlServer";

    public IDbConnection CreateConnection(DatabaseConnectionConfig config)
    {
        return new SqlConnection(config.ConnectionString);
    }

    public IDbDataAdapter CreateDataAdapter(IDbCommand command)
    {
        return new SqlDataAdapter((SqlCommand)command);
    }

    public async Task<List<string>> GetTablesAsync(IDbConnection connection)
    {
        var tables = new List<string>();
        const string query = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME";
        
        using var command = connection.CreateCommand();
        command.CommandText = query;
        
        using var reader = await ((SqlCommand)command).ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tables.Add(reader.GetString(0));
        }
        
        return tables;
    }

    public async Task<List<string>> GetColumnsAsync(IDbConnection connection, string tableName)
    {
        var columns = new List<string>();
        const string query = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName ORDER BY ORDINAL_POSITION";
        
        using var command = (SqlCommand)connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@tableName", tableName);
        
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            columns.Add(reader.GetString(0));
        }
        
        return columns;
    }
}
