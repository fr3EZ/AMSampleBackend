using AMSample.Application.Common.Enums;
using AMSample.Application.Meteorites.Models;
using AMSample.Application.Meteorites.Queries;
using AMSample.WebAPI.Tests.Contollers.MeteoritesControllerTests.Initializer;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AMSample.WebAPI.Tests.Contollers.MeteoritesControllerTests;

public class GetMeteoritesTests : TestMeteoritesControllerInitializer
{
    [Fact]
    public async Task Get_WithValidParameters_ReturnsOkResultWithData()
    {
        // Arrange
        var expectedResult = CreatePaginatedMeteoritesDto();
        MediatorMock
            .Setup(m => m.Send(It.IsAny<GetMeteoritesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var pageNumber = 1;
        var pageSize = 10;

        // Act
        var result = await Controller.Get(pageNumber, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<PaginatedMeteoritesDto>(okResult.Value);

        Assert.Equal(expectedResult.TotalCount, returnValue.TotalCount);
        Assert.Equal(expectedResult.TotalPages, returnValue.TotalPages);
        Assert.Equal(expectedResult.Meteorites.Length, returnValue.Meteorites.Length);

        MediatorMock.Verify(m => m.Send(
                It.Is<GetMeteoritesQuery>(q =>
                    q.PageNumber == pageNumber &&
                    q.PageSize == pageSize),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Get_WithAllFilters_ReturnsOkResult()
    {
        // Arrange
        var expectedResult = CreatePaginatedMeteoritesDto();
        MediatorMock
            .Setup(m => m.Send(It.IsAny<GetMeteoritesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var pageNumber = 1;
        var pageSize = 10;
        var yearFrom = new DateTime(1900, 1, 1);
        var yearTo = new DateTime(2000, 1, 1);
        var recClass = "L5";
        var nameContains = "Aach";
        var groupBy = GroupByType.Year;
        var sortBy = SortByType.Mass;
        var sortDirection = SortDirection.Ascending;

        // Act
        var result = await Controller.Get(pageNumber, pageSize, yearFrom, yearTo, recClass, nameContains, groupBy,
            sortBy, sortDirection);

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);

        MediatorMock.Verify(m => m.Send(
                It.Is<GetMeteoritesQuery>(q =>
                    q.Filters.YearFrom == yearFrom &&
                    q.Filters.YearTo == yearTo &&
                    q.Filters.RecClass == recClass &&
                    q.Filters.NameContains == nameContains &&
                    q.Filters.GroupBy == groupBy &&
                    q.Filters.SortBy == sortBy &&
                    q.Filters.SortDirection == sortDirection),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Get_WithGroupBy_ReturnsGroupedResults()
    {
        // Arrange
        var expectedResult = CreateGroupedMeteoritesDto();
        MediatorMock
            .Setup(m => m.Send(It.IsAny<GetMeteoritesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var pageNumber = 1;
        var pageSize = 10;
        var groupBy = GroupByType.Year;

        // Act
        var result = await Controller.Get(pageNumber, pageSize, groupBy: groupBy);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<PaginatedMeteoritesDto>(okResult.Value);

        Assert.NotNull(returnValue.GroupedResults);
        Assert.Equal(3, returnValue.GroupedResults.Length);
        Assert.Equal("2020", returnValue.GroupedResults[0].GroupKey);
    }

    [Fact]
    public async Task Get_WithNullOptionalParameters_UsesDefaultValues()
    {
        // Arrange
        var expectedResult = CreatePaginatedMeteoritesDto();
        MediatorMock
            .Setup(m => m.Send(It.IsAny<GetMeteoritesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var pageNumber = 1;
        var pageSize = 10;

        // Act
        var result = await Controller.Get(pageNumber, pageSize);

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);

        MediatorMock.Verify(m => m.Send(
                It.Is<GetMeteoritesQuery>(q =>
                    q.Filters.YearFrom == null &&
                    q.Filters.YearTo == null &&
                    q.Filters.RecClass == null &&
                    q.Filters.NameContains == null &&
                    q.Filters.GroupBy == GroupByType.None && // Default value
                    q.Filters.SortBy == SortByType.Year && // Default value
                    q.Filters.SortDirection == SortDirection.Descending), // Default value
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Get_WithEmptyStringParameters_TreatsAsNull()
    {
        // Arrange
        var expectedResult = CreatePaginatedMeteoritesDto();
        MediatorMock
            .Setup(m => m.Send(It.IsAny<GetMeteoritesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var pageNumber = 1;
        var pageSize = 10;
        var recClass = "";
        var nameContains = "";

        // Act
        var result = await Controller.Get(pageNumber, pageSize, recClass: recClass, nameContains: nameContains);

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);

        MediatorMock.Verify(m => m.Send(
                It.Is<GetMeteoritesQuery>(q =>
                    q.Filters.RecClass == "" &&
                    q.Filters.NameContains == ""),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Get_WithDifferentSortingOptions_ReturnsCorrectly()
    {
        // Arrange
        var expectedResult = CreatePaginatedMeteoritesDto();
        MediatorMock
            .Setup(m => m.Send(It.IsAny<GetMeteoritesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var testCases = new[]
        {
            new {SortBy = SortByType.Name, SortDirection = SortDirection.Ascending},
            new {SortBy = SortByType.Mass, SortDirection = SortDirection.Descending},
            new {SortBy = SortByType.Year, SortDirection = SortDirection.Ascending}
        };

        foreach (var testCase in testCases)
        {
            // Act
            var result = await Controller.Get(1, 10, sortBy: testCase.SortBy, sortDirection: testCase.SortDirection);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);

            MediatorMock.Verify(m => m.Send(
                    It.Is<GetMeteoritesQuery>(q =>
                        q.Filters.SortBy == testCase.SortBy &&
                        q.Filters.SortDirection == testCase.SortDirection),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}