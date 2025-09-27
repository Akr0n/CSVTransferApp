using Testcontainers.PostgreSql;

namespace CSVTransferApp.Integration.Tests.TestContainers;

public class DatabaseContainer : IAsyncDisposable
{
    private readonly PostgreSqlContainer _container;

    public DatabaseContainer()
    {
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:15")
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpass")
            .WithPortBinding(5432, true)
            .Build();
    }

    public async Task StartAsync()
    {
        await _container.StartAsync();
        await InitializeDatabaseAsync();
    }

    public string GetConnectionString()
    {
        return _container.GetConnectionString();
    }

    public async Task StopAsync()
    {
        await _container.StopAsync();
    }

    private async Task InitializeDatabaseAsync()
    {
        // Create test tables and insert sample data
        var connectionString = GetConnectionString();
        
        // This would typically use a migration tool or execute SQL scripts
        // For now, just a placeholder for database initialization
        await Task.Delay(100); // Simulate initialization
    }

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
