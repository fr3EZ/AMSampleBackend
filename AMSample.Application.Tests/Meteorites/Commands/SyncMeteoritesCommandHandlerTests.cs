using System.Net;
using Moq;
using Moq.Protected;
using System.Text;
using AMSample.Application.Common.Interfaces;
using AMSample.Application.Configuration;
using AMSample.Application.Meteorites.Commands;
using AMSample.Application.Meteorites.Models;
using AMSample.Domain.Entities;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AMSample.Application.Tests.Meteorites.Commands;

public class SyncMeteoritesCommandHandlerTests
{
    private readonly Mock<IMeteoriteApiService> _meteoriteApiServiceMock;
    private readonly Mock<IBatchMeteoriteProcessor> _batchMeteoriteProcessorMock;
    private readonly Mock<IRedisCacheService> _redisCacheServiceMock;
    private readonly Mock<ILogger<SyncMeteoritesCommandHandler>> _loggerMock;
    private readonly SyncMeteoritesCommandHandler _handler;
    private readonly CancellationToken _cancellationToken;

    public SyncMeteoritesCommandHandlerTests()
    {
        _meteoriteApiServiceMock = new Mock<IMeteoriteApiService>();
        _batchMeteoriteProcessorMock = new Mock<IBatchMeteoriteProcessor>();
        _loggerMock = new Mock<ILogger<SyncMeteoritesCommandHandler>>();
        _handler = new SyncMeteoritesCommandHandler(
            _batchMeteoriteProcessorMock.Object,
            _meteoriteApiServiceMock.Object,
            _redisCacheServiceMock.Object,
            _loggerMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    private void VerifyLogInformation(string expectedMessage)
    {
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedMessage)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);
    }

    private void VerifyLogError(string expectedMessage)
    {
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedMessage)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenSyncSucceeds_ReturnsUnitValue()
    {
        // Arrange
        var command = new SyncMeteoritesCommand();
        var response = new HttpResponseMessage(HttpStatusCode.OK);

        _meteoriteApiServiceMock
            .Setup(x => x.GetMeteoritesResponse())
            .ReturnsAsync(response);

        _batchMeteoriteProcessorMock
            .Setup(x => x.ProcessMeteoriteBatchesByStreamAsync(It.IsAny<Stream>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        Assert.Equal(Unit.Value, result);

        _meteoriteApiServiceMock.Verify(x => x.GetMeteoritesResponse(), Times.Once);
        _batchMeteoriteProcessorMock.Verify(x => x.ProcessMeteoriteBatchesByStreamAsync(It.IsAny<Stream>()),
            Times.Once);

        VerifyLogInformation("Meteorites sync started");
        VerifyLogInformation("Meteorites sync finished");
    }

    [Fact]
    public async Task Handle_WithCancellationToken_ProcessesSuccessfully()
    {
        // Arrange
        var command = new SyncMeteoritesCommand();
        var response = new HttpResponseMessage(HttpStatusCode.OK);
        var cancellationToken = new CancellationToken();

        _meteoriteApiServiceMock
            .Setup(x => x.GetMeteoritesResponse())
            .ReturnsAsync(response);

        _batchMeteoriteProcessorMock
            .Setup(x => x.ProcessMeteoriteBatchesByStreamAsync(It.IsAny<Stream>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.Equal(Unit.Value, result);
    }

    [Fact]
    public async Task Handle_WhenApiServiceThrowsException_LogsErrorAndReturnsUnit()
    {
        // Arrange
        var command = new SyncMeteoritesCommand();
        var expectedException = new HttpRequestException("API unavailable");

        _meteoriteApiServiceMock
            .Setup(x => x.GetMeteoritesResponse())
            .ThrowsAsync(expectedException);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        Assert.Equal(Unit.Value, result);

        VerifyLogError("Error while syncing meteorites");
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error while syncing meteorites")),
                expectedException,
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);

        _batchMeteoriteProcessorMock.Verify(
            x => x.ProcessMeteoriteBatchesByStreamAsync(It.IsAny<Stream>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenBatchProcessorThrowsException_LogsErrorAndReturnsUnit()
    {
        // Arrange
        var command = new SyncMeteoritesCommand();
        var response = new HttpResponseMessage(HttpStatusCode.OK);
        var expectedException = new InvalidOperationException("Processing failed");

        _meteoriteApiServiceMock
            .Setup(x => x.GetMeteoritesResponse())
            .ReturnsAsync(response);

        _batchMeteoriteProcessorMock
            .Setup(x => x.ProcessMeteoriteBatchesByStreamAsync(It.IsAny<Stream>()))
            .ThrowsAsync(expectedException);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        Assert.Equal(Unit.Value, result);

        VerifyLogError("Error while syncing meteorites");
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error while syncing meteorites")),
                expectedException,
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);
    }

    [Theory]
    [InlineData(typeof(HttpRequestException))]
    [InlineData(typeof(TimeoutException))]
    [InlineData(typeof(InvalidOperationException))]
    public async Task Handle_WhenAnyExceptionOccurs_LogsErrorAndReturnsUnit(Type exceptionType)
    {
        // Arrange
        var command = new SyncMeteoritesCommand();
        var exception = (Exception) Activator.CreateInstance(exceptionType, "Test exception")!;

        _meteoriteApiServiceMock
            .Setup(x => x.GetMeteoritesResponse())
            .ThrowsAsync(exception);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        Assert.Equal(Unit.Value, result);
        VerifyLogError("Error while syncing meteorites");
    }
}