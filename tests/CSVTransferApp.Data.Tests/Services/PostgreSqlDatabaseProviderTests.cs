using Xunit;
using FluentAssertions;
using CSVTransferApp.Data.Services;
using CSVTransferApp.Core.Models;

namespace CSVTransferApp.Data.Tests.Services;

public class PostgreSqlDatabaseProviderTests
{
    [Fact]
    public void ProviderName_ShouldReturnPostgreSQL()
    {
        // Arrange
        var provider = new PostgreSqlDatabaseProvider();

        // Act
        var name = provider.ProviderName;

        // Assert
        name.Should().Be("PostgreSQL");
    }

    [Fact]
    public void CreateConnection_WithValidConfig_ShouldReturnConnection()
    {
        // Arrange
        var provider = new PostgreSqlDatabaseProvider();
        var config = new DatabaseConnectionConfig
        {
            ConnectionString = "Host=localhost;Database=test;Username=postgres;Password=test"
        };

        // Act
        var connection = provider.CreateConnection(config);

        // Assert
        connection.Should().NotBeNull();
        connection.ConnectionString.Should().Be(config.ConnectionString);
    }
}
