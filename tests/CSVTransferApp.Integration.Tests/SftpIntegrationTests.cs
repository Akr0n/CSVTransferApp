using Xunit;
using FluentAssertions;
using CSVTransferApp.Services;
using CSVTransferApp.Core.Models;
using CSVTransferApp.Core.Interfaces;
using CSVTransferApp.Core.Tests.TestData;

namespace CSVTransferApp.Integration.Tests;

public class SftpIntegrationTests
{
    [Fact]
    public async Task SftpService_WithMockServer_ShouldUploadFile()
    {
        // Arrange
        var mockSftp = new MockSftpService();
        var fileName = "integration-test.csv";
        var csvContent = "ID,Name,Email\n1,Test User,test@example.com";
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csvContent));

        // Act
        await mockSftp.UploadFileAsync("TestConnection", stream, fileName);

        // Assert
        mockSftp.UploadedFiles.Should().Contain(fileName);
    }

    [Fact]
    public async Task SftpService_TestConnection_ShouldValidateCredentials()
    {
        // Arrange
        var mockSftp = new MockSftpService();

        // Act
        var isValid = await mockSftp.TestConnectionAsync("ValidConnection");
        var isInvalid = await mockSftp.TestConnectionAsync("Invalid");

        // Assert
        isValid.Should().BeTrue();
        isInvalid.Should().BeFalse();
    }

    [Fact]
    public async Task SftpService_FileOperations_ShouldWorkCorrectly()
    {
        // Arrange
        var mockSftp = new MockSftpService();
        var fileName = "test-operations.csv";
        var csvContent = "test,data";
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csvContent));

        // Act & Assert
        // Upload file
        await mockSftp.UploadFileAsync("TestConnection", stream, fileName);
        mockSftp.UploadedFiles.Should().Contain(fileName);

        // Check if file exists
        var exists = await mockSftp.FileExistsAsync("TestConnection", fileName);
        exists.Should().BeTrue();

        // Delete file
        await mockSftp.DeleteFileAsync("TestConnection", fileName);
        mockSftp.UploadedFiles.Should().NotContain(fileName);

        // Check if file no longer exists
        var stillExists = await mockSftp.FileExistsAsync("TestConnection", fileName);
        stillExists.Should().BeFalse();
    }

    // Note: For real SFTP integration tests, you would use TestContainers
    // to spin up an SFTP server or use a dedicated test SFTP service
}
