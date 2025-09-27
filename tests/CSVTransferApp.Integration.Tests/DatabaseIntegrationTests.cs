using Xunit;
using FluentAssertions;
using CSVTransferApp.Data.Services;
using CSVTransferApp.Core.Models;
using CSVTransferApp.Core.Constants;

namespace CSVTransferApp.Integration.Tests;

public class DatabaseIntegrationTests
{
    [Fact]
    public void OracleDatabaseProvider_ShouldCreateValidConnection()
    {
        // Arrange
        var provider = new OracleDatabaseProvider();
        var config = new DatabaseConnectionConfig
        {
            Provider = DatabaseProviders.Oracle,
            ConnectionString = "Data Source=localhost:1521/XE;User Id=test;Password=test;"
        };

        // Act
        using var connection = provider.CreateConnection(config);

        // Assert
        connection.Should().NotBeNull();
        connection.ConnectionString.Should().Be(config.ConnectionString);
    }

    [Fact]
    public void SqlServerDatabaseProvider_ShouldCreateValidConnection()
    {
        // Arrange
        var provider = new SqlServerDatabaseProvider();
        var config = new DatabaseConnectionConfig
        {
            Provider = DatabaseProviders.SqlServer,
            ConnectionString = "Server=localhost;Database=test;Integrated Security=true;"
        };

        // Act
        using var connection = provider.CreateConnection(config);

        // Assert
        connection.Should().NotBeNull();
        connection.ConnectionString.Should().Be(config.ConnectionString);
    }

    [Fact]
    public void PostgreSqlDatabaseProvider_ShouldCreateValidConnection()
    {
        // Arrange
        var provider = new PostgreSqlDatabaseProvider();
        var config = new DatabaseConnectionConfig
        {
            Provider = DatabaseProviders.PostgreSQL,
            ConnectionString = "Host=localhost;Database=test;Username=postgres;Password=test"
        };

        // Act
        using var connection = provider.CreateConnection(config);

        // Assert
        connection.Should().NotBeNull();
        connection.ConnectionString.Should().Be(config.ConnectionString);
    }

    // Note: These tests require actual database instances to run
    // In CI/CD, you would use TestContainers or similar to spin up test databases
}
