using System.Data;
using Oracle.ManagedDataAccess.Client;
using CSVTransferApp.Core.Models;

namespace CSVTransferApp.Data.Services;

public class OracleDatabaseProvider : IDatabaseProvider
{
    public string ProviderName => "Oracle";

    public IDbConnection CreateConnection(DatabaseConnectionConfig config)
    {
        return new OracleConnection(config.ConnectionString);
    }

    public IDbDataAdapter CreateDataAdapter(IDbCommand command)
    {
        return new OracleDataAdapter((OracleCommand)command);
    }

    public async Task<List<string>> GetTablesAsync(IDbConnection connection)
    {
        var tables = new List<string>();
        const string query = "SELECT table_name FROM user_tables ORDER BY table_name";
        
        using var command = connection.CreateCommand();
        command.CommandText = query;
        
        using var reader = await ((OracleCommand)command).ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tables.Add(reader.GetString(0));
        }
        
        return tables;
    }

    public async Task<List<string>> GetColumnsAsync(IDbConnection connection, string tableName)
    {
        var columns = new List<string>();
        const string query = "SELECT column_name FROM user_tab_columns WHERE table_name = :tableName ORDER BY column_id";
        
        using var command = (OracleCommand)connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.Add(new OracleParameter("tableName", tableName.ToUpper()));
        
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            columns.Add(reader.GetString(0));
        }
        
        return columns;
    }
}

public interface IDatabaseProvider
{
    string ProviderName { get; }
    IDbConnection CreateConnection(DatabaseConnectionConfig config);
    IDbDataAdapter CreateDataAdapter(IDbCommand command);
    Task<List<string>> GetTablesAsync(IDbConnection connection);
    Task<List<string>> GetColumnsAsync(IDbConnection connection, string tableName);
}
