namespace AMSample.Infrastructure.Services.BatchProcessors.Meteorites;

public class BatchMeteoriteProcessor(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<BatchMeteoriteProcessor> logger,
    IOptions<BatchProcessorConfig> batchProcessorConfig) : IBatchMeteoriteProcessor
{
    public async Task ProcessMeteoriteBatchesByStreamAsync(Stream stream)
    {
        var batchIndex = 0;
        var batchSize = batchProcessorConfig.Value.BatchSize;
        var batch = new MeteoriteJsonDto[batchSize];
        var processedMeteoriteExternalIds = new HashSet<string>();

        var meteorites = JsonSerializer.DeserializeAsyncEnumerable<MeteoriteJsonDto>(
            stream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            });

        await foreach (var meteorite in meteorites)
        {
            if (meteorite is not null)
            {
                batch[batchIndex++] = meteorite;
            }

            if (batchIndex >= batchSize)
            {
                await ProcessBatchAsync(batch, batchIndex, processedMeteoriteExternalIds);

                batchIndex = 0;

                await Task.Delay(10);
            }
        }

        if (IsLastBatch(batchIndex))
        {
            await ProcessBatchAsync(batch, batchIndex, processedMeteoriteExternalIds);

            logger.LogInformation("Processed last batch");
        }
        
        await ProcessDeleteBeforeBatchesAsync(processedMeteoriteExternalIds);

        if (batchProcessorConfig.Value.SaveAllPerOneTransaction)
        {
            await unitOfWork.Meteorites.SaveChangesAsync();
        }
    }

    private async Task ProcessBatchAsync(MeteoriteJsonDto[] batch, int batchIndex, HashSet<string> processedMeteoriteExternalIds)
    {
        var (toInsert, toUpdate) = await SplitBatchAsync(batch, batchIndex);
            
        await ProcessChangesForBatchAsync(toInsert, toUpdate);

        foreach (var item in batch)
        {
            if (item is not null) processedMeteoriteExternalIds.Add(item.ExternalId);
        }
    }

    private async Task<(List<Meteorite> toInsert, List<Meteorite> toUpdate)> SplitBatchAsync(MeteoriteJsonDto[] batch,
        int batchIndex)
    {
        var existingMeteorites =
            await unitOfWork.Meteorites.GetMeteoritesDictionaryByExternalIdsAsync(batch.Select(m => m.ExternalId));

        var toInsert = new List<Meteorite>(batchIndex);
        var toUpdate = new List<Meteorite>(batchIndex);

        foreach (var dto in batch)
        {
            var meteorite = mapper.Map<Meteorite>(dto);

            if (meteorite is not null)
            {
                if (existingMeteorites.TryGetValue(meteorite.ExternalId, out var existing))
                {
                    if (HasChanges(existing, meteorite))
                    {
                        UpdateMeteorite(existing, meteorite);
                        toUpdate.Add(existing);
                    }
                }
                else
                {
                    toInsert.Add(meteorite);
                }
            }
        }

        return (toInsert, toUpdate);
    }

    private async Task ProcessChangesForBatchAsync(List<Meteorite> toInsert, List<Meteorite> toUpdate)
    {
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
        
        if (!batchProcessorConfig.Value.SaveAllPerOneTransaction)
        {
            await unitOfWork.Meteorites.SaveChangesAsync();
        }
    }

    private async Task ProcessDeleteBeforeBatchesAsync(HashSet<string> processedMeteoriteExternalIds)
    {
        var existingMeteorites = await unitOfWork.Meteorites.GetMeteoriteExternalIdsAsync();

        var toDelete = existingMeteorites.Except(processedMeteoriteExternalIds).ToList();
        if (toDelete.Any())
        {
            await unitOfWork.Meteorites.BulkDeleteByExternalIdAsync(toDelete);
            logger.LogInformation("Deleted {Count} records", toDelete.Count);
        }
        
        if (!batchProcessorConfig.Value.SaveAllPerOneTransaction)
        {
            await unitOfWork.Meteorites.SaveChangesAsync();
        }
    }

    private void UpdateMeteorite(Meteorite existing, Meteorite updated)
    {
        existing.Name = updated.Name;
        existing.NameType = updated.NameType;
        existing.RecClass = updated.RecClass;
        existing.Mass = updated.Mass;
        existing.Fall = updated.Fall;
        existing.Year = updated.Year;
        existing.RecLat = updated.RecLat;
        existing.RecLong = updated.RecLong;
        if (updated.Geolocation != null)
        {
            if (existing.Geolocation == null)
            {
                existing.Geolocation = updated.Geolocation;
            }
            else
            {
                existing.Geolocation.Type = updated.Geolocation.Type;
                existing.Geolocation.Coordinates = updated.Geolocation.Coordinates;
            }
        }
        else
        {
            existing.Geolocation = null;
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
    
    private bool IsLastBatch(int batchIndex) => batchIndex > 0;
}