using AMSample.Application.Meteorites.Models;
using AMSample.WebAPI.Controllers;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace AMSample.WebAPI.Tests.Contollers.MeteoritesControllerTests.Initializer;

public abstract class TestMeteoritesControllerInitializer
{
    protected readonly Mock<IMediator> MediatorMock;
    protected readonly Mock<ILogger<MeteoritesController>> LoggerMock;
    protected readonly MeteoritesController Controller;

    protected TestMeteoritesControllerInitializer()
    {
        MediatorMock = new Mock<IMediator>();
        LoggerMock = new Mock<ILogger<MeteoritesController>>();
        Controller = new MeteoritesController(MediatorMock.Object, LoggerMock.Object);
    }

    protected PaginatedMeteoritesDto CreatePaginatedMeteoritesDto(int totalCount = 10, int totalPages = 1)
    {
        var meteorites = new MeteoriteDto[]
        {
            new() {Id = "1", Name = "Aachen", Year = new DateTime(1880, 1, 1), RecClass = "L5", Mass = 21m},
            new() {Id = "2", Name = "Aarhus", Year = new DateTime(1951, 1, 1), RecClass = "H5", Mass = 720m},
            new() {Id = "3", Name = "Abee", Year = new DateTime(1952, 1, 1), RecClass = "EH4", Mass = 107000m}
        };

        return new PaginatedMeteoritesDto
        {
            TotalCount = totalCount,
            TotalPages = totalPages,
            Meteorites = meteorites.Take(Math.Min(3, totalCount)).ToArray()
        };
    }

    protected PaginatedMeteoritesDto CreateGroupedMeteoritesDto()
    {
        var groupedResults = new GroupResultDto[]
        {
            new() {GroupKey = "2020", Count = 5, TotalMass = 1000m},
            new() {GroupKey = "2021", Count = 3, TotalMass = 500m},
            new() {GroupKey = "2022", Count = 2, TotalMass = 200m}
        };

        return new PaginatedMeteoritesDto
        {
            TotalCount = 10,
            TotalPages = 1,
            GroupedResults = groupedResults
        };
    }
}