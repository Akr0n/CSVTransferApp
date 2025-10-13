using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using CSVTransferApp.Services;
using CSVTransferApp.Core.Interfaces;

namespace CSVTransferApp.Services.Tests;

public class LoggingServiceTests : IDisposable
{
    private readonly Mock<ILogger<LoggingService>> _loggerMock;
    private readonly Mock<IConfigurationService> _configurationServiceMock;
    private readonly LoggingService _service;
    private readonly string _testLogPath = Path.Combine(Path.GetTempPath(), "test-logs");

    public LoggingServiceTests()
    {
        _loggerMock = new Mock<ILogger<LoggingService>>();
        _configurationServiceMock = new Mock<IConfigurationService>();
        
        // Ensure test directory exists
        if (Directory.Exists(_testLogPath))
        {
            Directory.Delete(_testLogPath, true);
        }
        Directory.CreateDirectory(_testLogPath);
        
        _configurationServiceMock.Setup(x => x.GetValue<string>("Processing:LogPath", "./logs"))
            .Returns(_testLogPath);

        _service = new LoggingService(_loggerMock.Object, _configurationServiceMock.Object);

        // Cleanup test directory
        if (Directory.Exists(_testLogPath))
        {
            Directory.Delete(_testLogPath, true);
        }
    }

    [Fact]
    public void LogInformation_ShouldCallUnderlyingLogger()
    {
        // Arrange
        var message = "Test information message";
        var args = new object[] { "arg1", "arg2" };

        // Act
        _service.LogInformation(message, args);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Test information message")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogError_WithException_ShouldCallUnderlyingLogger()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var message = "Test error message";

        // Act
        _service.LogError(exception, message);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task LogToFileAsync_ShouldCreateLogFile()
    {
        // Arrange
        var fileName = "test.log";
        var message = "Test log message";

        // Act
        await _service.LogToFileAsync(fileName, message);

        // Assert
        var logFilePath = Path.Combine(_testLogPath, fileName);
        File.Exists(logFilePath).Should().BeTrue();
        
        var content = await File.ReadAllTextAsync(logFilePath);
        content.Should().Contain(message);
    }

    [Fact]
    public async Task LogToFileAsync_WithMultipleMessages_ShouldAppendToFile()
    {
        // Arrange
        var fileName = "append-test.log";
        var message1 = "First message";
        var message2 = "Second message";

        // Act
        await _service.LogToFileAsync(fileName, message1);
        await _service.LogToFileAsync(fileName, message2);

        // Assert
        var logFilePath = Path.Combine(_testLogPath, fileName);
        var content = await File.ReadAllTextAsync(logFilePath);
        
        content.Should().Contain(message1);
        content.Should().Contain(message2);
        content.Split('\n', StringSplitOptions.RemoveEmptyEntries).Should().HaveCountGreaterOrEqualTo(2);
    }
    
    public void Dispose()
    {
        // Clean up test directory
        if (Directory.Exists(_testLogPath))
        {
            Directory.Delete(_testLogPath, true);
        }
        GC.SuppressFinalize(this);
    }
}
