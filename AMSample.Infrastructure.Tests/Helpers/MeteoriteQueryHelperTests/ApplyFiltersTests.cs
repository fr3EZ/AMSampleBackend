using AMSample.Application.Meteorites.Models;
using AMSample.Infrastructure.Helpers;
using AMSample.Infrastructure.Tests.Helpers.MeteoriteQueryHelperTests.TestData;

namespace AMSample.Infrastructure.Tests.Helpers.MeteoriteQueryHelperTests;

public class ApplyFiltersTests : MeteoriteQueryHelperTestData
{
    [Fact]
    public void ApplyFilters_WhenFiltersNull_ReturnsOriginalQuery()
    {
        // Arrange
        var query = GetTestQueryable();
        MeteoriteFilters? filters = null;

        // Act
        var result = MeteoriteQueryHelper.ApplyFilters(query, filters);

        // Assert
        Assert.Equal(query.Count(), result.Count());
    }

    [Theory]
    [InlineData(1900, 1950, 5)] // Year range 1900-1950
    [InlineData(1950, 2000, 3)] // Year range 1950-2000
    [InlineData(1800, 1850, 1)] // Year range 1800-1850 (only Agen)
    public void ApplyFilters_WithYearRange_ReturnsFilteredResults(int yearFrom, int yearTo, int expectedCount)
    {
        // Arrange
        var query = GetTestQueryable();
        var filters = new MeteoriteFilters(
            YearFrom: new DateTime(yearFrom, 1, 1),
            YearTo: new DateTime(yearTo, 1, 1),
            RecClass: null,
            NameContains: null,
            GroupBy: null,
            SortBy: null,
            SortDirection: null
        );

        // Act
        var result = MeteoriteQueryHelper.ApplyFilters(query, filters);

        // Assert
        Assert.Equal(expectedCount, result.Count());
        Assert.All(result, m =>
        {
            Assert.True(m.Year >= filters.YearFrom);
            Assert.True(m.Year <= filters.YearTo);
        });
    }

    [Fact]
    public void ApplyFilters_WithYearFromOnly_ReturnsResultsFromYear()
    {
        // Arrange
        var query = GetTestQueryable();
        var filters = new MeteoriteFilters(
            YearFrom: new DateTime(1950, 1, 1),
            YearTo: null,
            RecClass: null,
            NameContains: null,
            GroupBy: null,
            SortBy: null,
            SortDirection: null
        );

        // Act
        var result = MeteoriteQueryHelper.ApplyFilters(query, filters);

        // Assert
        Assert.Equal(3, result.Count()); // 1951, 1952, 1976
        Assert.All(result, m => Assert.True(m.Year >= new DateTime(1950, 1, 1)));
    }

    [Fact]
    public void ApplyFilters_WithYearToOnly_ReturnsResultsUpToYear()
    {
        // Arrange
        var query = GetTestQueryable();
        var filters = new MeteoriteFilters(
            YearFrom: null,
            YearTo: new DateTime(1900, 1, 1),
            RecClass: null,
            NameContains: null,
            GroupBy: null,
            SortBy: null,
            SortDirection: null
        );

        // Act
        var result = MeteoriteQueryHelper.ApplyFilters(query, filters);

        // Assert
        Assert.Equal(2, result.Count()); // 1880, 1814
        Assert.All(result, m => Assert.True(m.Year <= new DateTime(1900, 1, 1)));
    }

    [Theory]
    [InlineData("H5", 2)] // Aarhus, Agen
    [InlineData("L6", 2)] // Achiras, Aguada
    [InlineData("EH4", 2)] // Abee, Adhi Kot
    [InlineData("Unknown", 1)] // No Year meteorite
    public void ApplyFilters_WithRecClass_ReturnsFilteredResults(string recClass, int expectedCount)
    {
        // Arrange
        var query = GetTestQueryable();
        var filters = new MeteoriteFilters(
            YearFrom: null,
            YearTo: null,
            RecClass: recClass,
            NameContains: null,
            GroupBy: null,
            SortBy: null,
            SortDirection: null
        );

        // Act
        var result = MeteoriteQueryHelper.ApplyFilters(query, filters);

        // Assert
        Assert.Equal(expectedCount, result.Count());
        Assert.All(result, m => Assert.Contains(recClass, m.RecClass));
    }

    [Theory]
    [InlineData("Aachen", 1)]
    [InlineData("Agu", 2)] // Aguada, Aguila Blanca
    [InlineData("Ad", 2)] // Adhi Kot, Adzhi-Bogdo
    [InlineData("Nonexistent", 0)]
    public void ApplyFilters_WithNameContains_ReturnsFilteredResults(string nameContains, int expectedCount)
    {
        // Arrange
        var query = GetTestQueryable();
        var filters = new MeteoriteFilters(
            YearFrom: null,
            YearTo: null,
            RecClass: null,
            NameContains: nameContains,
            GroupBy: null,
            SortBy: null,
            SortDirection: null
        );

        // Act
        var result = MeteoriteQueryHelper.ApplyFilters(query, filters);

        // Assert
        Assert.Equal(expectedCount, result.Count());
        Assert.All(result, m => Assert.Contains(nameContains, m.Name));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void ApplyFilters_WithEmptyRecClass_DoesNotFilter(string recClass)
    {
        // Arrange
        var query = GetTestQueryable();
        var filters = new MeteoriteFilters(
            YearFrom: null,
            YearTo: null,
            RecClass: recClass,
            NameContains: null,
            GroupBy: null,
            SortBy: null,
            SortDirection: null
        );

        // Act
        var result = MeteoriteQueryHelper.ApplyFilters(query, filters);

        // Assert
        Assert.Equal(query.Count(), result.Count());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void ApplyFilters_WithEmptyNameContains_DoesNotFilter(string nameContains)
    {
        // Arrange
        var query = GetTestQueryable();
        var filters = new MeteoriteFilters(
            YearFrom: null,
            YearTo: null,
            RecClass: null,
            NameContains: nameContains,
            GroupBy: null,
            SortBy: null,
            SortDirection: null
        );

        // Act
        var result = MeteoriteQueryHelper.ApplyFilters(query, filters);

        // Assert
        Assert.Equal(query.Count(), result.Count());
    }
}