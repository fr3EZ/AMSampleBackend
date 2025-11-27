namespace AMSample.Application.Meteorites.Models;

public record MeteoriteFilters(
    DateTime? YearFrom,
    DateTime? YearTo,
    string? RecClass,
    string? NameContains,
    GroupByType? GroupBy,
    SortByType? SortBy,
    SortDirection? SortDirection);