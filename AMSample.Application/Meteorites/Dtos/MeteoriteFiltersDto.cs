namespace AMSample.Application.Meteorites.Dtos;

public record MeteoriteFiltersDto(
    DateTime? YearFrom,
    DateTime? YearTo,
    string? RecClass,
    string? NameContains,
    GroupByType? GroupBy,
    SortByType? SortBy,
    SortDirection? SortDirection);