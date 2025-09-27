using Xunit;
using FluentAssertions;
using Moq;
using CSVTransferApp.Data.Services;
using CSVTransferApp.Core.Interfaces;
using CSVTransferApp.Core.Tests.TestData;

namespace CSVTransferApp.Data.Tests.Services;

public class DatabaseServiceTests
{
    private readonly Mock<IConfigurationService> _configurationServiceMock;
    private readonly Mock<ILoggerService> _loggerServiceMock;

    public DatabaseServiceTests()
    {
        _configurationServiceMock = new Mock<IConfigurationService>();
        _loggerServiceMock = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task ExecuteQueryAsync_WithValidConnection_ShouldReturnData()
    {
        // Arrange
        var mockDatabase = new MockDatabaseService();
        var query = "SELECT * FROM employees";

        // Act
        var result = await mockDatabase.ExecuteQueryAsync("TestConnection", query);

        // Assert
        result.Should().NotBeNull();
        result.Rows.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task TestConnectionAsync_WithValidConnection_ShouldReturnTrue()
    {
        // Arrange
        var mockDatabase = new MockDatabaseService();

        // Act
        var result = await mockDatabase.TestConnectionAsync("ValidConnection");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task TestConnectionAsync_WithInvalidConnection_ShouldReturnFalse()
    {
        // Arrange
        var mockDatabase = new MockDatabaseService();

        // Act
        var result = await mockDatabase.TestConnectionAsync("Invalid");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetTablesAsync_ShouldReturnTableList()
    {
        // Arrange
        var mockDatabase = new MockDatabaseService();

        // Act
        var result = await mockDatabase.GetTablesAsync("TestConnection");

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain("employees");
        result.Should().Contain("products");
    }

    [Fact]
    public async Task GetColumnsAsync_WithValidTable_ShouldReturnColumns()
    {
        // Arrange
        var mockDatabase = new MockDatabaseService();

        // Act
        var result = await mockDatabase.GetColumnsAsync("TestConnection", "employees");

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain("emp_id");
        result.Should().Contain("first_name");
        result.Should().Contain("last_name");
    }
}
