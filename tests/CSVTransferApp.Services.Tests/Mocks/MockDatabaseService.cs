using System.Data;
using CSVTransferApp.Core.Interfaces;
using CSVTransferApp.Core.Tests.TestData;

namespace CSVTransferApp.Services.Tests.Mocks;

public class MockDatabaseService : IDatabaseService
{
    private readonly Dictionary<string, DataTable> _mockTables;
    public bool ShouldThrowException { get; set; }
    public string? ExceptionMessage { get; set; }

    public MockDatabaseService()
    {
        _mockTables = new Dictionary<string, DataTable>
        {
            { "employees", SampleData.CreateEmployeesDataTable() },
            { "products", SampleData.CreateProductsDataTable() }
        };
    }

    public Task<DataTable> ExecuteQueryAsync(string connectionName, string query)
    {
        if (ShouldThrowException)
            throw new InvalidOperationException(ExceptionMessage ?? "Mock database error");

        // Simple query parsing for mock
        if (query.ToLower().Contains("employees"))
            return Task.FromResult(_mockTables["employees"].Copy());
        
        if (query.ToLower().Contains("products"))
            return Task.FromResult(_mockTables["products"].Copy());

        // Return empty table for unknown queries
        return Task.FromResult(new DataTable());
    }

    public Task<bool> TestConnectionAsync(string connectionName)
    {
        if (ShouldThrowException)
            throw new InvalidOperationException(ExceptionMessage ?? "Mock connection test error");
            
        return Task.FromResult(connectionName != "Invalid");
    }

    public Task<List<string>> GetTablesAsync(string connectionName)
    {
        if (ShouldThrowException)
            throw new InvalidOperationException(ExceptionMessage ?? "Mock get tables error");
            
        return Task.FromResult(_mockTables.Keys.ToList());
    }

    public Task<List<string>> GetColumnsAsync(string connectionName, string tableName)
    {
        if (ShouldThrowException)
            throw new InvalidOperationException(ExceptionMessage ?? "Mock get columns error");

        if (_mockTables.TryGetValue(tableName, out var table))
        {
            return Task.FromResult(table.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList());
        }
        
        return Task.FromResult(new List<string>());
    }

    public void AddMockTable(string name, DataTable table)
    {
        _mockTables[name] = table;
    }

    public void Dispose()
    {
        _mockTables.Clear();
        GC.SuppressFinalize(this);
    }
}
