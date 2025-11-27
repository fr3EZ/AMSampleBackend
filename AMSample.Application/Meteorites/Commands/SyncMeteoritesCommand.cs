namespace AMSample.Application.Meteorites.Commands;

public record SyncMeteoritesCommand : IRequest<Unit>;

public class SyncMeteoritesCommandHandler(
    IBatchMeteoriteProcessor batchMeteoriteProcessor,
    IMeteoriteApiService meteoriteApiService,
    IRedisCacheService redisCacheService,
    ILogger<SyncMeteoritesCommandHandler> logger) : IRequestHandler<SyncMeteoritesCommand, Unit>
{
    public async Task<Unit> Handle(SyncMeteoritesCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Meteorites sync started");

            var response = await meteoriteApiService.GetMeteoritesResponse();

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

            await batchMeteoriteProcessor.ProcessMeteoriteBatchesByStreamAsync(stream);

            await redisCacheService.RemoveByPrefixAsync(Constants.MeteoritesCachePrefix,cancellationToken);

            logger.LogInformation("Meteorites sync finished");

            return Unit.Value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while syncing meteorites");

            return Unit.Value;
        }
    }
}