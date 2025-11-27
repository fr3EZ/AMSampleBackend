using AMSample.Application.Common.Enums;
using AMSample.Application.Meteorites.Models;
using AMSample.Infrastructure.Helpers;
using AMSample.Infrastructure.Tests.Helpers.MeteoriteQueryHelperTests.TestData;

namespace AMSample.Infrastructure.Tests.Helpers.MeteoriteQueryHelperTests;

public class ApplySortingTests : MeteoriteQueryHelperTestData
{
    [Fact]
    public void ApplySorting_WhenFiltersNull_ReturnsQueryOrderedByNameAscending()
    {
        // Arrange
        var query = GetTestQueryable();
        MeteoriteFilters? filters = null;

        // Act
        var result = MeteoriteQueryHelper.ApplySorting(query, filters);

        // Assert
        var sortedList = result.ToList();
        Assert.Equal("Aachen", sortedList[0].Name);
        Assert.Equal("Aarhus", sortedList[1].Name);
    }

    [Fact]
    public void ApplySorting_WhenSortByNull_ReturnsQueryOrderedByNameAscending()
    {
        // Arrange
        var query = GetTestQueryable();
        var filters = new MeteoriteFilters(
            YearFrom: null,
            YearTo: null,
            RecClass: null,
            NameContains: null,
            GroupBy: null,
            SortBy: null,
            SortDirection: null
        );

        // Act
        var result = MeteoriteQueryHelper.ApplySorting(query, filters);

        // Assert
        var sortedList = result.ToList();
        Assert.Equal("Aachen", sortedList[0].Name);
    }

    [Theory]
    [InlineData(SortByType.Year, SortDirection.Descending, "Acapulco")] // Newest first (1976)
    public void ApplySorting_ByYear_ReturnsCorrectlySorted(SortByType sortBy, SortDirection direction,
        string expectedFirstName)
    {
        // Arrange
        var query = GetTestQueryable();
        var filters = new MeteoriteFilters(
            YearFrom: new DateTime(1814, 1, 1),
            YearTo: null,
            RecClass: null,
            NameContains: null,
            GroupBy: null,
            SortBy: sortBy,
            SortDirection: direction
        );

        // Act
        var result = MeteoriteQueryHelper.ApplySorting(query, filters);

        // Assert
        Assert.Equal(expectedFirstName, result.First().Name);
    }

    [Theory]
    [InlineData(SortByType.Mass, SortDirection.Ascending, "Aachen")] // Smallest mass first (21)
    [InlineData(SortByType.Mass, SortDirection.Descending, "Abee")] // Largest mass first (107000)
    public void ApplySorting_ByMass_ReturnsCorrectlySorted(SortByType sortBy, SortDirection direction,
        string expectedFirstName)
    {
        // Arrange
        var query = GetTestQueryable();
        var filters = new MeteoriteFilters(
            YearFrom: null,
            YearTo: null,
            RecClass: null,
            NameContains: null,
            GroupBy: null,
            SortBy: sortBy,
            SortDirection: direction
        );

        // Act
        var result = MeteoriteQueryHelper.ApplySorting(query, filters);

        // Assert
        Assert.Equal(expectedFirstName, result.First().Name);
    }

    [Theory]
    [InlineData(SortByType.Name, SortDirection.Ascending, "Aachen")]
    [InlineData(SortByType.Name, SortDirection.Descending, "No Year")] // "No Year" comes last alphabetically
    public void ApplySorting_ByName_ReturnsCorrectlySorted(SortByType sortBy, SortDirection direction,
        string expectedFirstName)
    {
        // Arrange
        var query = GetTestQueryable();
        var filters = new MeteoriteFilters(
            YearFrom: null,
            YearTo: null,
            RecClass: null,
            NameContains: null,
            GroupBy: null,
            SortBy: sortBy,
            SortDirection: direction
        );

        // Act
        var result = MeteoriteQueryHelper.ApplySorting(query, filters);

        // Assert
        Assert.Equal(expectedFirstName, result.First().Name);
    }

    [Fact]
    public void ApplySorting_WithUnknownSortBy_ReturnsDefaultOrderedByName()
    {
        // Arrange
        var query = GetTestQueryable();
        var filters = new MeteoriteFilters(
            YearFrom: null,
            YearTo: null,
            RecClass: null,
            NameContains: null,
            GroupBy: null,
            SortBy: (SortByType) 999, // Unknown value
            SortDirection: SortDirection.Ascending
        );

        // Act
        var result = MeteoriteQueryHelper.ApplySorting(query, filters);

        // Assert
        var sortedList = result.ToList();
        Assert.Equal("Aachen", sortedList[0].Name);
    }
}