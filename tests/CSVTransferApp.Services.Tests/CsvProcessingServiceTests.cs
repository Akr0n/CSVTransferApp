using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CSVTransferApp.Services;
using CSVTransferApp.Core.Models;
using CSVTransferApp.Core.Interfaces;
using CSVTransferApp.Core.Tests.TestData;

namespace CSVTransferApp.Services.Tests;

public class CsvProcessingServiceTests
{
    private readonly Mock<IDatabaseService> _databaseServiceMock;
    private readonly Mock<ISftpService> _sftpServiceMock;
    private readonly Mock<ILogger<CsvProcessingService>> _loggerMock;
    private readonly CsvProcessingService _service;

    public CsvProcessingServiceTests()
    {
        _databaseServiceMock = new Mock<IDatabaseService>();
        _sftpServiceMock = new Mock<ISftpService>();
        _loggerMock = new Mock<ILogger<CsvProcessingService>>();
        
        // Create real configuration instead of mocking extension methods
        var configData = new Dictionary<string, string>
        {
            ["Processing:MaxConcurrentConnections"] = "5",
            ["Processing:MaxConcurrentFiles"] = "10"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        _service = new CsvProcessingService(
            _databaseServiceMock.Object,
            _sftpServiceMock.Object,
            _loggerMock.Object,
            configuration);
    }

    [Fact]
    public async Task ProcessJobAsync_WithValidJob_ShouldReturnSuccessResult()
    {
        // Arrange
        var job = new TransferJob
        {
            TableName = "test_table",
            DatabaseConnection = "TestDB",
            SftpConnection = "TestSFTP",
            Query = "SELECT * FROM test_table"
        };

        var testData = SampleData.CreateEmployeesDataTable();
        _databaseServiceMock.Setup(x => x.ExecuteQueryAsync(job.DatabaseConnection, job.Query))
            .ReturnsAsync(testData);

        _sftpServiceMock.Setup(x => x.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.ProcessJobAsync(job);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.TableName.Should().Be(job.TableName);
        result.RecordsProcessed.Should().Be(testData.Rows.Count);
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task ProcessJobAsync_WithDatabaseError_ShouldReturnFailureResult()
    {
        // Arrange
        var job = new TransferJob
        {
            TableName = "test_table",
            DatabaseConnection = "TestDB",
            SftpConnection = "TestSFTP",
            Query = "SELECT * FROM test_table"
        };

        _databaseServiceMock.Setup(x => x.ExecuteQueryAsync(job.DatabaseConnection, job.Query))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act
        var result = await _service.ProcessJobAsync(job);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Database connection failed");
    }

    [Fact]
    public async Task ProcessJobAsync_WithSftpError_ShouldReturnFailureResult()
    {
        // Arrange
        var job = new TransferJob
        {
            TableName = "test_table",
            DatabaseConnection = "TestDB",
            SftpConnection = "TestSFTP",
            Query = "SELECT * FROM test_table"
        };

        var testData = SampleData.CreateEmployeesDataTable();
        _databaseServiceMock.Setup(x => x.ExecuteQueryAsync(job.DatabaseConnection, job.Query))
            .ReturnsAsync(testData);

        _sftpServiceMock.Setup(x => x.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("SFTP upload failed"));

        // Act
        var result = await _service.ProcessJobAsync(job);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("SFTP upload failed");
    }

    [Fact]
    public async Task ProcessJobsAsync_WithMultipleJobs_ShouldProcessAllJobs()
    {
        // Arrange
        var jobs = SampleData.CreateTransferJobs();
        var testData = SampleData.CreateEmployeesDataTable();

        _databaseServiceMock.Setup(x => x.ExecuteQueryAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(testData);

        _sftpServiceMock.Setup(x => x.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var results = await _service.ProcessJobsAsync(jobs);

        // Assert
        results.Should().HaveCount(jobs.Count);
        results.Should().AllSatisfy(r => r.IsSuccess.Should().BeTrue());
    }
}
