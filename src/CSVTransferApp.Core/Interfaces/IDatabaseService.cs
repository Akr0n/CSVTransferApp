using System.Data;
using CSVTransferApp.Core.Models;

namespace CSVTransferApp.Core.Interfaces;

public interface IDatabaseService
{
    Task<DataTable> ExecuteQueryAsync(string connectionName, string query);
    Task<bool> TestConnectionAsync(string connectionName);
    Task<List<string>> GetTablesAsync(string connectionName);
    Task<List<string>> GetColumnsAsync(string connectionName, string tableName);
    void Dispose();
}
