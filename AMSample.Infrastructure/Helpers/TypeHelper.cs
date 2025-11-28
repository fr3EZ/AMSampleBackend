namespace AMSample.Infrastructure.Helpers;

public static class TypeHelper
{
    public static decimal? ParseDecimal(string? value)
    {
        if (string.IsNullOrEmpty(value)) return null;
        return decimal.TryParse(value, NumberStyles.Any,
            CultureInfo.InvariantCulture, out var result)
            ? result
            : null;
    }

    public static DateTime? ParseDateTime(string? value)
    {
        if (string.IsNullOrEmpty(value)) return null;
        return DateTime.TryParse(value, out var result) ? result : null;
    }
}