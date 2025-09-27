using Microsoft.Extensions.Configuration;
using CSVTransferApp.Core.Interfaces;
using CSVTransferApp.Services;
using CSVTransferApp.Core.Tests.TestData;

namespace CSVTransferApp.Data.Tests.TestFixtures;

public class DatabaseTestFixture : IDisposable
{
    public IConfigurationService ConfigurationService { get; }
    public ILoggerService LoggerService { get; }
    public MockDatabaseService MockDatabaseService { get; }

    public DatabaseTestFixture()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string?>("DatabaseConnections:Test:Provider", "Mock"),
                new KeyValuePair<string, string?>("DatabaseConnections:Test:ConnectionString", "mock://test")
            })
            .Build();

        ConfigurationService = new ConfigurationService(configuration);
        LoggerService = new MockLoggerService();
        MockDatabaseService = new MockDatabaseService();
    }

    public void Dispose()
    {
        MockDatabaseService?.Dispose();
        GC.SuppressFinalize(this);
    }
}
