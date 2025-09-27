using System.Data;
using CSVTransferApp.Core.Interfaces;
using CSVTransferApp.Core.Models;

namespace CSVTransferApp.Core.Tests.TestData;

public class MockDatabaseService : IDatabaseService
{
    private readonly Dictionary<string, DataTable> _tables;

    public MockDatabaseService()
    {
        _tables = new Dictionary<string, DataTable>
        {
            { "employees", SampleData.CreateEmployeesDataTable() },
            { "products", SampleData.CreateProductsDataTable() }
        };
    }

    public Task<DataTable> ExecuteQueryAsync(string connectionName, string query)
    {
        // Simple mock implementation
        if (query.Contains("employees"))
            return Task.FromResult(_tables["employees"]);
        if (query.Contains("products"))
            return Task.FromResult(_tables["products"]);
            
        return Task.FromResult(new DataTable());
    }

    public Task<bool> TestConnectionAsync(string connectionName)
    {
        return Task.FromResult(connectionName != "Invalid");
    }

    public Task<List<string>> GetTablesAsync(string connectionName)
    {
        return Task.FromResult(_tables.Keys.ToList());
    }

    public Task<List<string>> GetColumnsAsync(string connectionName, string tableName)
    {
        if (_tables.TryGetValue(tableName, out var table))
        {
            return Task.FromResult(table.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList());
        }
        return Task.FromResult(new List<string>());
    }

    public void Dispose()
    {
        // Mock disposal
    }
}

public class MockSftpService : ISftpService
{
    public List<string> UploadedFiles { get; } = new();
    public bool ShouldThrowException { get; set; }

    public Task UploadFileAsync(string connectionName, Stream fileStream, string fileName)
    {
        if (ShouldThrowException)
            throw new InvalidOperationException("Mock SFTP error");

        UploadedFiles.Add(fileName);
        return Task.CompletedTask;
    }

    public Task<bool> TestConnectionAsync(string connectionName)
    {
        return Task.FromResult(connectionName != "Invalid");
    }

    public Task<bool> FileExistsAsync(string connectionName, string fileName)
    {
        return Task.FromResult(UploadedFiles.Contains(fileName));
    }

    public Task DeleteFileAsync(string connectionName, string fileName)
    {
        UploadedFiles.Remove(fileName);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        // Mock disposal
    }
}

public class MockLoggerService : ILoggerService
{
    public List<string> LoggedMessages { get; } = new();
    public List<Exception> LoggedExceptions { get; } = new();

    public void LogInformation(string message, params object[] args)
    {
        LoggedMessages.Add(string.Format(message, args));
    }

    public void LogWarning(string message, params object[] args)
    {
        LoggedMessages.Add($"WARNING: {string.Format(message, args)}");
    }

    public void LogError(Exception exception, string message, params object[] args)
    {
        LoggedExceptions.Add(exception);
        LoggedMessages.Add($"ERROR: {string.Format(message, args)}");
    }

    public void LogError(string message, params object[] args)
    {
        LoggedMessages.Add($"ERROR: {string.Format(message, args)}");
    }

    public Task LogToFileAsync(string fileName, string message)
    {
        LoggedMessages.Add($"FILE[{fileName}]: {message}");
        return Task.CompletedTask;
    }
}
