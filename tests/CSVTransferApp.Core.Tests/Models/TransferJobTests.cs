using Xunit;
using FluentAssertions;
using CSVTransferApp.Core.Models;

namespace CSVTransferApp.Core.Tests.Models;

public class TransferJobTests
{
    [Fact]
    public void TransferJob_WhenCreated_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var job = new TransferJob();

        // Assert
        job.RequestTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        job.TableName.Should().BeEmpty();
        job.DatabaseConnection.Should().BeEmpty();
        job.SftpConnection.Should().BeEmpty();
        job.Query.Should().BeEmpty();
    }

    [Fact]
    public void TransferJob_WhenPropertiesSet_ShouldReturnCorrectValues()
    {
        // Arrange
        var tableName = "test_table";
        var dbConnection = "Oracle";
        var sftpConnection = "MainServer";
        var query = "SELECT * FROM test_table";
        var requestTime = DateTime.UtcNow;

        // Act
        var job = new TransferJob
        {
            TableName = tableName,
            DatabaseConnection = dbConnection,
            SftpConnection = sftpConnection,
            Query = query,
            RequestTime = requestTime
        };

        // Assert
        job.TableName.Should().Be(tableName);
        job.DatabaseConnection.Should().Be(dbConnection);
        job.SftpConnection.Should().Be(sftpConnection);
        job.Query.Should().Be(query);
        job.RequestTime.Should().Be(requestTime);
    }
}
