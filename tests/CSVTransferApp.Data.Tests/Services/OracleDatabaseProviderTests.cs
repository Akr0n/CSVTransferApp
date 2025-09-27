using Xunit;
using FluentAssertions;
using CSVTransferApp.Data.Services;
using CSVTransferApp.Core.Models;

namespace CSVTransferApp.Data.Tests.Services;

public class OracleDatabaseProviderTests
{
    [Fact]
    public void ProviderName_ShouldReturnOracle()
    {
        // Arrange
        var provider = new OracleDatabaseProvider();

        // Act
        var name = provider.ProviderName;

        // Assert
        name.Should().Be("Oracle");
    }

    [Fact]
    public void CreateConnection_WithValidConfig_ShouldReturnConnection()
    {
        // Arrange
        var provider = new OracleDatabaseProvider();
        var config = new DatabaseConnectionConfig
        {
            ConnectionString = "Data Source=localhost:1521/XE;User Id=test;Password=test;"
        };

        // Act
        var connection = provider.CreateConnection(config);

        // Assert
        connection.Should().NotBeNull();
        connection.ConnectionString.Should().Be(config.ConnectionString);
    }

    // Note: Integration tests for actual database operations would require
    // a test Oracle database or test containers
}
