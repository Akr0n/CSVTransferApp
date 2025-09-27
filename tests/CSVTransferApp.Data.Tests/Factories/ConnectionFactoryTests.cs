using Xunit;
using FluentAssertions;
using CSVTransferApp.Data.Factories;
using CSVTransferApp.Core.Models;
using CSVTransferApp.Core.Constants;

namespace CSVTransferApp.Data.Tests.Factories;

public class ConnectionFactoryTests
{
    [Theory]
    [InlineData(DatabaseProviders.Oracle)]
    [InlineData(DatabaseProviders.SqlServer)]
    [InlineData(DatabaseProviders.PostgreSQL)]
    public void CreateConnection_WithSupportedProvider_ShouldReturnConnection(string provider)
    {
        // Arrange
        var factory = new ConnectionFactory();
        var config = new DatabaseConnectionConfig
        {
            Provider = provider,
            ConnectionString = GetTestConnectionString(provider)
        };

        // Act
        var connection = factory.CreateConnection(config);

        // Assert
        connection.Should().NotBeNull();
        connection.ConnectionString.Should().Be(config.ConnectionString);
    }

    [Fact]
    public void CreateConnection_WithUnsupportedProvider_ShouldThrowException()
    {
        // Arrange
        var factory = new ConnectionFactory();
        var config = new DatabaseConnectionConfig
        {
            Provider = "UnsupportedProvider",
            ConnectionString = "test"
        };

        // Act & Assert
        factory.Invoking(f => f.CreateConnection(config))
            .Should().Throw<NotSupportedException>()
            .WithMessage("Database provider 'UnsupportedProvider' is not supported");
    }

    [Fact]
    public void GetProvider_WithValidProvider_ShouldReturnProvider()
    {
        // Arrange
        var factory = new ConnectionFactory();

        // Act
        var provider = factory.GetProvider(DatabaseProviders.Oracle);

        // Assert
        provider.Should().NotBeNull();
        provider.ProviderName.Should().Be("Oracle");
    }

    private static string GetTestConnectionString(string provider) => provider switch
    {
        DatabaseProviders.Oracle => "Data Source=localhost:1521/XE;User Id=test;Password=test;",
        DatabaseProviders.SqlServer => "Server=localhost;Database=test;Integrated Security=true;",
        DatabaseProviders.PostgreSQL => "Host=localhost;Database=test;Username=postgres;Password=test",
        _ => "test"
    };
}
