using Xunit;
using FluentAssertions;
using Moq;
using CSVTransferApp.Services;
using CSVTransferApp.Core.Interfaces;
using CSVTransferApp.Core.Models;

namespace CSVTransferApp.Services.Tests;

public class SftpServiceTests
{
    private readonly Mock<IConfigurationService> _configurationServiceMock;
    private readonly Mock<ILoggerService> _loggerServiceMock;

    public SftpServiceTests()
    {
        _configurationServiceMock = new Mock<IConfigurationService>();
        _loggerServiceMock = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task TestConnectionAsync_WithValidConnection_ShouldReturnTrue()
    {
        // Arrange
        var mockSftp = new MockSftpService();

        // Act
        var result = await mockSftp.TestConnectionAsync("ValidConnection");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task TestConnectionAsync_WithInvalidConnection_ShouldReturnFalse()
    {
        // Arrange
        var mockSftp = new MockSftpService();

        // Act
        var result = await mockSftp.TestConnectionAsync("Invalid");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UploadFileAsync_WithValidParameters_ShouldUploadFile()
    {
        // Arrange
        var mockSftp = new MockSftpService();
        var fileName = "test.csv";
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("test,data\n1,value"));

        // Act
        await mockSftp.UploadFileAsync("TestConnection", stream, fileName);

        // Assert
        mockSftp.UploadedFiles.Should().Contain(fileName);
    }

    [Fact]
    public async Task UploadFileAsync_WithSftpError_ShouldThrowException()
    {
        // Arrange
        var mockSftp = new MockSftpService { ShouldThrowException = true };
        var fileName = "test.csv";
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("test,data\n1,value"));

        // Act & Assert
        await mockSftp.Invoking(s => s.UploadFileAsync("TestConnection", stream, fileName))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Mock SFTP error");
    }

    [Fact]
    public async Task FileExistsAsync_WithExistingFile_ShouldReturnTrue()
    {
        // Arrange
        var mockSftp = new MockSftpService();
        var fileName = "existing.csv";
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("test"));
        await mockSftp.UploadFileAsync("TestConnection", stream, fileName);

        // Act
        var result = await mockSftp.FileExistsAsync("TestConnection", fileName);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteFileAsync_WithExistingFile_ShouldRemoveFile()
    {
        // Arrange
        var mockSftp = new MockSftpService();
        var fileName = "toDelete.csv";
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("test"));
        await mockSftp.UploadFileAsync("TestConnection", stream, fileName);

        // Act
        await mockSftp.DeleteFileAsync("TestConnection", fileName);

        // Assert
        mockSftp.UploadedFiles.Should().NotContain(fileName);
    }
}
