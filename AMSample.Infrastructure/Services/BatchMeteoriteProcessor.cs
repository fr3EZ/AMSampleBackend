using Microsoft.Extensions.Options;

namespace AMSample.Infrastructure.Services;

public class BatchMeteoriteProcessor(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<BatchMeteoriteProcessor> logger,
    IOptions<SyncDataConfig> syncDataConfig) : IBatchMeteoriteProcessor
{
    public async Task ProcessMeteoriteBatchesByStreamAsync(Stream stream)
    {
        var batchSize = syncDataConfig.Value.BatchSize;
        var totalProcessed = 0;

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        var meteorites = JsonSerializer.DeserializeAsyncEnumerable<MeteoriteJsonDto>(
            stream, options);

        var existingMeteorites = await unitOfWork.Meteorites.GetMeteoritesDictionaryAsync();

        var processedMeteoriteExternalIds = new HashSet<string>();

        var batchIndex = 0;
        var batch = new MeteoriteJsonDto[batchSize];

        await foreach (var meteorite in meteorites)
        {
            if (meteorite is not null)
            {
                batch[batchIndex++] = meteorite;
            }

            if (batchIndex >= batchSize)
            {
                await ProcessMeteoriteBatchAsync(batch, batchIndex, existingMeteorites);
                totalProcessed += batchIndex;
                logger.LogInformation("Processed {Count} records, totally processed: {Total}",
                    batchIndex, totalProcessed);

                foreach (var item in batch)
                {
                    if(item is not null) processedMeteoriteExternalIds.Add(item.ExternalId);
                }

                batchIndex = 0;

                await Task.Delay(10);
            }
        }

        if (batchIndex > 0)
        {
            await ProcessMeteoriteBatchAsync(batch, batchIndex, existingMeteorites);
            totalProcessed += batchIndex;

            foreach (var item in batch)
            {
                if(item is not null) processedMeteoriteExternalIds.Add(item.ExternalId);
            }

            logger.LogInformation("Processed last batch: {Count}, totally processed: {Total}",
                batchIndex, totalProcessed);
        }

        var toDelete = existingMeteorites.Keys.Except(processedMeteoriteExternalIds).ToList();
        if (toDelete.Any())
        {
            await unitOfWork.Meteorites.BulkDeleteByExternalIdAsync(toDelete);
            logger.LogInformation("Deleted {Count} records", toDelete.Count);
        }

        await unitOfWork.Meteorites.SaveChangesAsync();
    }

    private async Task ProcessMeteoriteBatchAsync(MeteoriteJsonDto[] batch, int batchIndex,
        Dictionary<string, Meteorite> existingMeteorites)
    {
        var toInsert = new List<Meteorite>(batchIndex);
        var toUpdate = new List<Meteorite>(batchIndex);

        foreach (var dto in batch)
        {
            var meteorite = mapper.Map<Meteorite>(dto);

            if (meteorite is not null)
            {
                if (existingMeteorites.TryGetValue(meteorite.ExternalId, out var existing))
                {
                    meteorite.Id = existing.Id;
                    
                    if (meteorite.Geolocation is not null && existing.Geolocation is not null)
                    {
                        meteorite.Geolocation.Id = existing.Geolocation.Id;
                    }
                    
                    if (HasChanges(existing, meteorite))
                    {
                        toUpdate.Add(meteorite);
                        // UpdateMeteoriteFromDto(existing, dto);
                        // toUpdate.Add(existing);
                    }
                }
                else
                {
                    toInsert.Add(meteorite);
                }
            }
        }

        if (toInsert.Any())
        {
            await unitOfWork.Meteorites.BulkInsertAsync(toInsert);
            logger.LogInformation("Added {Count} records", toInsert.Count);
        }

        if (toUpdate.Any())
        {
            unitOfWork.Meteorites.BulkUpdate(toUpdate);
            logger.LogInformation("Updated {Count} records", toUpdate.Count);
        }
    }

    private bool HasChanges(Meteorite existing, Meteorite updated)
    {
        return !string.Equals(existing.Name, updated.Name) ||
               !string.Equals(existing.NameType, updated.NameType) ||
               !string.Equals(existing.RecClass, updated.RecClass) ||
               existing.Mass != updated.Mass ||
               existing.Fall != updated.Fall ||
               existing.Year != updated.Year ||
               existing.RecLat != updated.RecLat ||
               existing.RecLong != updated.RecLong ||
               !GeolocationEquals(existing.Geolocation, updated.Geolocation);
    }

    private bool GeolocationEquals(Geolocation? geo1, Geolocation? geo2)
    {
        if (geo1 is null && geo2 is null) return true;
        if (geo1 is null || geo2 is null) return false;

        return geo1.Type == geo2.Type &&
               geo1.Coordinates.SequenceEqual(geo2.Coordinates);
    }
}