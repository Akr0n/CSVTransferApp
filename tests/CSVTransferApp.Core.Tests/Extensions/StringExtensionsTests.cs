using Xunit;
using FluentAssertions;
using CSVTransferApp.Core.Extensions;

namespace CSVTransferApp.Core.Tests.Extensions;

public class StringExtensionsTests
{
    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("   ", true)]
    [InlineData("test", false)]
    public void IsNullOrWhiteSpace_ShouldReturnCorrectResult(string input, bool expected)
    {
        // Act
        var result = input.IsNullOrWhiteSpace();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("valid_filename.txt", "valid_filename.txt")]
    [InlineData("file<>name.txt", "filename.txt")]
    [InlineData("file|name?.txt", "filename.txt")]
    [InlineData("file\\name/test.txt", "filenametest.txt")]
    public void ToSafeFileName_ShouldRemoveInvalidCharacters(string input, string expected)
    {
        // Act
        var result = input.ToSafeFileName();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ReplaceTokens_ShouldReplaceAllTokens()
    {
        // Arrange
        var template = "Hello {name}, your order {orderId} is ready!";
        var tokens = new Dictionary<string, string>
        {
            { "name", "John" },
            { "orderId", "12345" }
        };

        // Act
        var result = template.ReplaceTokens(tokens);

        // Assert
        result.Should().Be("Hello John, your order 12345 is ready!");
    }

    [Theory]
    [InlineData("", 10, "")]
    [InlineData("short", 10, "short")]
    [InlineData("this is a long string", 10, "this is...")]
    [InlineData("exactly10c", 10, "exactly10c")]
    public void TruncateWithEllipsis_ShouldTruncateCorrectly(string input, int maxLength, string expected)
    {
        // Act
        var result = input.TruncateWithEllipsis(maxLength);

        // Assert
        result.Should().Be(expected);
    }
}
