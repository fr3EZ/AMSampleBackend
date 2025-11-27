using AMSample.Application.Helpers;

namespace AMSample.Application.Tests.Helpers.TypeHelperTests;

public class ParseDecimalTests
{
    [Theory]
    [InlineData("123.45", 123.45)]
    [InlineData("123", 123.0)]
    [InlineData("-123.45", -123.45)]
    [InlineData("0", 0.0)]
    [InlineData("0.0", 0.0)]
    [InlineData("1,234.56", 1234.56)]
    public void ParseDecimal_ValidString_ReturnsExpectedValue(string input, decimal expected)
    {
        // Act
        var result = TypeHelper.ParseDecimal(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ParseDecimal_NullOrEmptyString_ReturnsNull(string input)
    {
        // Act
        var result = TypeHelper.ParseDecimal(input);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("12.34.56")]
    [InlineData("--123")]
    [InlineData("12a.34")]
    [InlineData("1.2.3.4")]
    public void ParseDecimal_InvalidString_ReturnsNull(string input)
    {
        // Act
        var result = TypeHelper.ParseDecimal(input);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("79228162514264337593543950335")]
    [InlineData("-79228162514264337593543950335")]
    public void ParseDecimal_EdgeCases_ReturnsExpectedValue(string input)
    {
        // Act
        var result = TypeHelper.ParseDecimal(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(decimal.Parse(input, System.Globalization.CultureInfo.InvariantCulture), result);
    }
}