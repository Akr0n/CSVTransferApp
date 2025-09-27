using System.Data;
using Xunit;
using FluentAssertions;
using CSVTransferApp.Core.Extensions;

namespace CSVTransferApp.Core.Tests.Extensions;

public class DataTableExtensionsTests
{
    [Fact]
    public void ToCsvStream_WithSimpleData_ShouldGenerateCorrectCsv()
    {
        // Arrange
        var dataTable = CreateTestDataTable();
        
        // Act
        using var stream = dataTable.ToCsvStream();
        using var reader = new StreamReader(stream);
        var csvContent = reader.ReadToEnd();

        // Assert
        var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        lines.Should().HaveCount(3); // Header + 2 data rows
        lines[0].Should().Be("ID,Name,Email");
        lines[1].Should().Be("1,John Doe,john@example.com");
        lines[2].Should().Be("2,Jane Smith,jane@example.com");
    }

    [Fact]
    public void ToCsvStream_WithCustomHeaders_ShouldUseProvidedHeaders()
    {
        // Arrange
        var dataTable = CreateTestDataTable();
        var customHeaders = new[] { "Identifier", "Full Name", "Email Address" };
        
        // Act
        using var stream = dataTable.ToCsvStream(customHeaders);
        using var reader = new StreamReader(stream);
        var csvContent = reader.ReadToEnd();

        // Assert
        var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        lines[0].Should().Be("Identifier,Full Name,Email Address");
    }

    [Fact]
    public void ToCsvStream_WithSpecialCharacters_ShouldEscapeProperly()
    {
        // Arrange
        var dataTable = new DataTable();
        dataTable.Columns.Add("Name", typeof(string));
        dataTable.Columns.Add("Description", typeof(string));
        
        var row = dataTable.NewRow();
        row["Name"] = "Test, Inc.";
        row["Description"] = "Company with \"quotes\" and\nnewlines";
        dataTable.Rows.Add(row);
        
        // Act
        using var stream = dataTable.ToCsvStream();
        using var reader = new StreamReader(stream);
        var csvContent = reader.ReadToEnd();

        // Assert
        csvContent.Should().Contain("\"Test, Inc.\"");
        csvContent.Should().Contain("\"Company with \"\"quotes\"\" and\nnewlines\"");
    }

    [Fact]
    public void GetColumnTypes_ShouldReturnCorrectTypes()
    {
        // Arrange
        var dataTable = CreateTestDataTable();
        
        // Act
        var columnTypes = dataTable.GetColumnTypes();

        // Assert
        columnTypes.Should().HaveCount(3);
        columnTypes["ID"].Should().Be(typeof(int));
        columnTypes["Name"].Should().Be(typeof(string));
        columnTypes["Email"].Should().Be(typeof(string));
    }

    private static DataTable CreateTestDataTable()
    {
        var dataTable = new DataTable();
        dataTable.Columns.Add("ID", typeof(int));
        dataTable.Columns.Add("Name", typeof(string));
        dataTable.Columns.Add("Email", typeof(string));

        var row1 = dataTable.NewRow();
        row1["ID"] = 1;
        row1["Name"] = "John Doe";
        row1["Email"] = "john@example.com";
        dataTable.Rows.Add(row1);

        var row2 = dataTable.NewRow();
        row2["ID"] = 2;
        row2["Name"] = "Jane Smith";
        row2["Email"] = "jane@example.com";
        dataTable.Rows.Add(row2);

        return dataTable;
    }
}
