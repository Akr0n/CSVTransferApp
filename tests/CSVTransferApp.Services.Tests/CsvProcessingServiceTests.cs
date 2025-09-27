using Xunit;
using FluentAssertions;
using Moq;
using CSVTransferApp.Services;
using CSVTransferApp.Core.Models;
using CSVTransferApp.Core.Interfaces;
using CSVTransferApp.Core.Tests.TestData;

namespace CSVTransferApp.Services.Tests;

public class CsvProcessingServiceTests
{
    private readonly Mock<IDatabaseService> _databaseServiceMock;
    private readonly Mock<ISftpService> _sftpServiceMock;
    private readonly Mock<ILoggerService> _loggerServiceMock;
    private readonly Mock<IConfigurationService> _configurationServiceMock;
    private readonly CsvProcessingService _service;

    public CsvProcessingServiceTests()
    {
        _databaseServiceMock = new Mock<IDatabaseService>();
        _sftpServiceMock = new Mock<ISftpService>();
        _loggerServiceMock = new Mock<ILoggerService>();
        _configurationServiceMock = new Mock<IConfigurationService>();

        // Setup default configuration values
        _configurationServiceMock.Setup(x => x.GetValue("Processing:MaxConcurrentConnections", It.IsAny<int>()))
            .Returns(5);
        _configurationServiceMock.Setup(x => x.GetValue("Processing:MaxConcurrentFiles", It.IsAny<int>()))
            .Returns(10);

        _service = new CsvProcessingService(
            _databaseServiceMock.Object,
            _sftpServiceMock.Object,
            _loggerServiceMock.Object,
            _configurationServiceMock.Object);
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
