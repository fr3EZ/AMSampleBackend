using AMSample.Application.Common.Enums;
using AMSample.Application.Common.Interfaces;
using AMSample.Application.Meteorites.Models;
using AMSample.Application.Meteorites.Queries;
using AMSample.Domain.Common;
using AMSample.Domain.Entities;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;

namespace AMSample.Application.Tests.Meteorites.Queries;

public class GetMeteoritesQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IRedisCacheService> _mockCacheService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetMeteoritesQueryHandler _handler;

    public GetMeteoritesQueryHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCacheService = new Mock<IRedisCacheService>();
        _mockMapper = new Mock<IMapper>();

        _handler = new GetMeteoritesQueryHandler(
            _mockUnitOfWork.Object,
            _mockCacheService.Object,
            _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_WithCacheHit_ReturnsCachedResult()
    {
        // Arrange
        var query = new GetMeteoritesQuery(1, 20, new MeteoriteFilters(
            YearFrom: new DateTime(1970, 1, 1),
            YearTo: new DateTime(1980, 1, 1),
            RecClass: "L5",
            NameContains: "Aachen",
            GroupBy: GroupByType.None,
            SortBy: SortByType.Year,
            SortDirection: SortDirection.Descending
        ));

        var cachedResult = new PaginatedMeteoritesDto
        {
            Meteorites =
            [
                new() {Id = "1", Name = "Aachen", RecClass = "L5"}
            ],
            TotalCount = 1,
            TotalPages = 1,
        };

        var cacheKey = "meteorites:test123";

        _mockCacheService.Setup(c => c.GenerateKey("meteorites", It.IsAny<object>()))
            .Returns(cacheKey);
        _mockCacheService.Setup(c => c.GetAsync<PaginatedMeteoritesDto>(cacheKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(cachedResult);
        _mockUnitOfWork.Verify(
            u => u.Meteorites.GetPaginatedMeteorites(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<MeteoriteFilters>()),
            Times.Never);
        _mockCacheService.Verify(
            c => c.SetAsync(It.IsAny<string>(), It.IsAny<PaginatedMeteoritesDto>(),
                It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithCacheMiss_ReturnsDatabaseResultAndCachesIt()
    {
        // Arrange
        var query = new GetMeteoritesQuery(1, 20, null);

        var paginatedMeteorites = new PaginatedEntity<Meteorite>
        {
            Entities = new List<Meteorite>
            {
                new() {Id = 1, Name = "Aachen", RecClass = "L5"},
                new() {Id = 2, Name = "Abee", RecClass = "EH4"}
            },
            TotalCount = 2,
            TotalPages = 1
        };

        var expectedDto = new PaginatedMeteoritesDto
        {
            Meteorites =
            [
                new() {Id = "1", Name = "Aachen", RecClass = "L5"},
                new() {Id = "2", Name = "Abee", RecClass = "EH4"}
            ],
            TotalCount = 2,
            TotalPages = 1,
        };

        var cacheKey = "meteorites:test456";

        _mockCacheService.Setup(c => c.GenerateKey("meteorites", It.IsAny<object>()))
            .Returns(cacheKey);
        _mockCacheService.Setup(c => c.GetAsync<PaginatedMeteoritesDto>(cacheKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PaginatedMeteoritesDto) null);
        _mockUnitOfWork.Setup(u => u.Meteorites.GetPaginatedMeteorites(1, 20, null))
            .ReturnsAsync(paginatedMeteorites);
        _mockMapper.Setup(m => m.Map<PaginatedMeteoritesDto>(paginatedMeteorites))
            .Returns(expectedDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
        _mockUnitOfWork.Verify(u => u.Meteorites.GetPaginatedMeteorites(1, 20, null), Times.Once);
        _mockCacheService.Verify(c => c.SetAsync(
                cacheKey,
                expectedDto,
                It.Is<DistributedCacheEntryOptions>(opt =>
                    opt.AbsoluteExpirationRelativeToNow == TimeSpan.FromMinutes(10)),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNullFilters_WorksCorrectly()
    {
        // Arrange
        var query = new GetMeteoritesQuery(1, 10, null);

        var paginatedResult = new PaginatedEntity<Meteorite>
        {
            Entities = new List<Meteorite>(),
            TotalCount = 0,
            TotalPages = 0
        };

        _mockCacheService.Setup(c => c.GenerateKey("meteorites", It.IsAny<object>()))
            .Returns("meteorites:key123");
        _mockCacheService.Setup(c =>
                c.GetAsync<PaginatedMeteoritesDto>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PaginatedMeteoritesDto) null);
        _mockUnitOfWork.Setup(u => u.Meteorites.GetPaginatedMeteorites(1, 10, null))
            .ReturnsAsync(paginatedResult);
        _mockMapper.Setup(m => m.Map<PaginatedMeteoritesDto>(paginatedResult))
            .Returns(new PaginatedMeteoritesDto());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        _mockUnitOfWork.Verify(u => u.Meteorites.GetPaginatedMeteorites(1, 10, null), Times.Once);
    }

    [Fact]
    public async Task Handle_WithEmptyResult_CachesEmptyResult()
    {
        // Arrange
        var query = new GetMeteoritesQuery(1, 20, null);

        var emptyPaginatedResult = new PaginatedEntity<Meteorite>
        {
            Entities = new List<Meteorite>(),
            TotalCount = 0,
            TotalPages = 0
        };

        var emptyDto = new PaginatedMeteoritesDto
        {
            Meteorites = Array.Empty<MeteoriteDto>(),
            TotalCount = 0,
            TotalPages = 0,
        };

        _mockCacheService.Setup(c => c.GenerateKey("meteorites", It.IsAny<object>()))
            .Returns("meteorites:empty123");
        _mockCacheService.Setup(c =>
                c.GetAsync<PaginatedMeteoritesDto>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PaginatedMeteoritesDto) null);
        _mockUnitOfWork.Setup(u => u.Meteorites.GetPaginatedMeteorites(1, 20, null))
            .ReturnsAsync(emptyPaginatedResult);
        _mockMapper.Setup(m => m.Map<PaginatedMeteoritesDto>(emptyPaginatedResult))
            .Returns(emptyDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(emptyDto);
        _mockCacheService.Verify(c => c.SetAsync(
                It.IsAny<string>(),
                It.Is<PaginatedMeteoritesDto>(dto => dto.TotalCount == 0 && dto.Meteorites.Length == 0),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}