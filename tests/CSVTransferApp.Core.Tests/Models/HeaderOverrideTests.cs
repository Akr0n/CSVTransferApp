using Xunit;
using FluentAssertions;
using CSVTransferApp.Core.Models;

namespace CSVTransferApp.Core.Tests.Models;

public class HeaderOverrideTests
{
    [Fact]
    public void HeaderOverride_WhenCreated_ShouldHaveEmptyMappings()
    {
        // Arrange & Act
        var headerOverride = new HeaderOverride();

        // Assert
        headerOverride.ColumnMappings.Should().BeEmpty();
    }

    [Fact]
    public void HeaderOverride_WhenMappingsSet_ShouldReturnCorrectMappings()
    {
        // Arrange
        var mappings = new Dictionary<string, string>
        {
            { "emp_id", "Employee ID" },
            { "first_name", "First Name" },
            { "last_name", "Last Name" }
        };

        // Act
        var headerOverride = new HeaderOverride
        {
            ColumnMappings = mappings
        };

        // Assert
        headerOverride.ColumnMappings.Should().BeEquivalentTo(mappings);
        headerOverride.ColumnMappings.Should().HaveCount(3);
        headerOverride.ColumnMappings["emp_id"].Should().Be("Employee ID");
    }
}
