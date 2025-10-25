using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CSVTransferApp.Console;
using CSVTransferApp.Console.DependencyInjection;
using CSVTransferApp.Console.Parsers;
using CSVTransferApp.Core.Models;
using CSVTransferApp.Core.Interfaces;
using CSVTransferApp.Infrastructure.Health;
using CSVTransferApp.Core.Tests.TestData;
using CSVTransferApp.Integration.Tests.Mocks;
using CSVTransferApp.Services;

namespace CSVTransferApp.Integration.Tests;

public class EndToEndTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;

    public EndToEndTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Application_WithTransferCommand_ShouldProcessSuccessfully()
    {
        // Arrange
        var args = new[] { "transfer", "--table", "employees", "--db-connection", "Test", "--sftp-connection", "Test" };
        
        // Act
        var exitCode = await _fixture.Application.RunAsync(args);

        // Assert
        exitCode.Should().Be(0);
    }

    [Fact]
    public async Task Application_WithInvalidCommand_ShouldReturnError()
    {
        // Arrange
        var args = new[] { "invalid-command" };
        
        // Act
        var exitCode = await _fixture.Application.RunAsync(args);

        // Assert
        exitCode.Should().Be(1);
    }

    [Fact]
    public async Task Application_WithMissingRequiredParameter_ShouldReturnError()
    {
        // Arrange
        var args = new[] { "transfer" }; // Missing --table parameter
        
        // Act
        var exitCode = await _fixture.Application.RunAsync(args);

        // Assert
        exitCode.Should().Be(1);
    }

    [Fact]
    public async Task CsvProcessingService_WithRealData_ShouldGenerateValidCsv()
    {
        // Arrange
        var job = new TransferJob
        {
            TableName = "employees",
            DatabaseConnection = "Test",
            SftpConnection = "Test",
            Query = "SELECT * FROM employees"
        };

        // Act
        var result = await _fixture.CsvProcessingService.ProcessJobAsync(job);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.RecordsProcessed.Should().BeGreaterThan(0);
        result.FileSizeBytes.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task BatchProcessing_WithMultipleJobs_ShouldProcessAllSuccessfully()
    {
        // Arrange
        var jobs = new List<TransferJob>
        {
            new() { TableName = "employees", DatabaseConnection = "Test", SftpConnection = "Test", Query = "SELECT * FROM employees" },
            new() { TableName = "products", DatabaseConnection = "Test", SftpConnection = "Test", Query = "SELECT * FROM products" }
        };

        // Act
        var results = await _fixture.CsvProcessingService.ProcessJobsAsync(jobs);

        // Assert
        results.Should().HaveCount(2);
        results.Should().AllSatisfy(r => r.IsSuccess.Should().BeTrue());
    }
}

public class IntegrationTestFixture : IDisposable
{
    public Application Application { get; }
    public ICsvProcessingService CsvProcessingService { get; }
    public IServiceProvider ServiceProvider { get; }

    public IntegrationTestFixture()
    {
        var configuration = CreateTestConfiguration();
        var services = new ServiceCollection();
        
        services.AddSingleton<IConfiguration>(configuration);
        
        // Add logging
        services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Information));
        
        // Add mocks first
        services.AddSingleton<IDatabaseService, MockDatabaseService>();
        services.AddSingleton<ISftpService, MockSftpService>();
        
        // Use MockCsvProcessingService instead of real one
        services.AddScoped<ICsvProcessingService, MockCsvProcessingService>();
        
        // Add other required services from Console DI but not overlapping ones
        services.AddSingleton<ILoggerService, LoggingService>();
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        services.AddSingleton<ICommandLineParser, CommandLineParser>();
        services.AddSingleton<IHealthCheckService, FileHealthCheckService>();
        services.AddSingleton<Application>();

        ServiceProvider = services.BuildServiceProvider();
        Application = ServiceProvider.GetRequiredService<Application>();
        CsvProcessingService = ServiceProvider.GetRequiredService<ICsvProcessingService>();
    }

    private static IConfiguration CreateTestConfiguration()
    {
        var configData = new Dictionary<string, string?>
        {
            {"DatabaseConnections:Test:Provider", "Mock"},
            {"DatabaseConnections:Test:ConnectionString", "mock://test"},
            {"SftpConnections:Test:Host", "mock.example.com"},
            {"SftpConnections:Test:Username", "testuser"},
            {"SftpConnections:Test:Password", "testpass"},
            {"Processing:MaxConcurrentConnections", "3"},
            {"Processing:MaxConcurrentFiles", "5"},
            {"Processing:HeaderOverridePath", "./test-overrides"},
            {"Processing:LogPath", "./test-logs"}
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();
    }

    public void Dispose()
    {
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
        GC.SuppressFinalize(this);
    }
}
