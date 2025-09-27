using Xunit;
using FluentAssertions;
using CSVTransferApp.Data.Services;
using CSVTransferApp.Core.Models;

namespace CSVTransferApp.Data.Tests.Services;

public class SqlServerDatabaseProviderTests
{
    [Fact]
    public void ProviderName_ShouldReturnSqlServer()
    {
        // Arrange
        var provider = new SqlServerDatabaseProvider();

        // Act
        var name = provider.ProviderName;

        // Assert
        name.Should().Be("SqlServer");
    }

    [Fact]
    public void CreateConnection_WithValidConfig_ShouldReturnConnection()
    {
        // Arrange
        var provider = new SqlServerDatabaseProvider();
        var config = new DatabaseConnectionConfig
        {
            ConnectionString = "Server=localhost;Database=test;Integrated Security=true;"
        };

        // Act
        var connection = provider.CreateConnection(config);

        // Assert
        connection.Should().NotBeNull();
        connection.ConnectionString.Should().Be(config.ConnectionString);
    }
}
