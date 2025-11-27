using AMSample.Application.Helpers;

namespace AMSample.Application.Tests.Helpers.TypeHelperTests;

public class ParseDateTimeTests
{
    [Theory]
    [InlineData("2023-12-31", 2023, 12, 31)]
    [InlineData("2023-12-31 14:30:25", 2023, 12, 31, 14, 30, 25)]
    [InlineData("31.12.2023", 2023, 12, 31)]
    public void ParseDateTime_ValidString_ReturnsExpectedValue(string input, int year, int month, int day, int hour = 0,
        int minute = 0, int second = 0)
    {
        // Arrange
        var expected = new DateTime(year, month, day, hour, minute, second);

        // Act
        var result = TypeHelper.ParseDateTime(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ParseDateTime_NullOrEmptyString_ReturnsNull(string input)
    {
        // Act
        var result = TypeHelper.ParseDateTime(input);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("2023-13-45")]
    [InlineData("31.31.2023")]
    [InlineData("2023-02-30")]
    [InlineData("not-a-date")]
    [InlineData("1234567890")]
    public void ParseDateTime_InvalidString_ReturnsNull(string input)
    {
        // Act
        var result = TypeHelper.ParseDateTime(input);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ParseDateTime_MinValue_ReturnsMinValue()
    {
        // Arrange
        var minDateString = DateTime.MinValue.ToString();

        // Act
        var result = TypeHelper.ParseDateTime(minDateString);

        // Assert
        Assert.Equal(DateTime.MinValue, result);
    }

    [Theory]
    [InlineData("2023-12-31 23:59:59.999")]
    [InlineData("2023-12-31 00:00:00.000")]
    public void ParseDateTime_WithMilliseconds_ReturnsExpectedValue(string input)
    {
        // Act
        var result = TypeHelper.ParseDateTime(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2023, result.Value.Year);
        Assert.Equal(12, result.Value.Month);
        Assert.Equal(31, result.Value.Day);
    }
}