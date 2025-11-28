namespace AMSample.Infrastructure.Data.Repositories;

public sealed class MeteoriteRepository(MeteoriteDbContext context)
    : BulkRepository<Meteorite>(context), IMeteoriteRepository
{
    public async Task<Dictionary<string, Meteorite>> GetMeteoritesDictionaryByExternalIdsAsync(IEnumerable<string> externalIds)
    {
        return await context.Meteorites
            .AsNoTracking()
            .Where(m => externalIds.Contains(m.ExternalId))
            .Include(m => m.Geolocation)
            .ToDictionaryAsync(g => g.ExternalId);
    }

    public async Task<IEnumerable<string>> GetMeteoriteExternalIdsAsync()
    {
        return await context.Meteorites
            .AsNoTracking()
            .Select(m => m.ExternalId)
            .ToHashSetAsync();
    }

    public async Task<PaginatedEntity<Meteorite>> GetPaginatedMeteorites(
        int pageNumber,
        int pageSize,
        MeteoriteFiltersDto filtersDto)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        IQueryable<Meteorite> query = DbSet.Include(m => m.Geolocation);

        query = MeteoriteQueryHelper.ApplyFilters(query, filtersDto);

        if (filtersDto.GroupBy != GroupByType.None)
        {
            return await MeteoriteQueryHelper.GetGroupedResults(query, pageNumber, pageSize, filtersDto);
        }

        query = MeteoriteQueryHelper.ApplySorting(query, filtersDto);

        var totalCount = await query.CountAsync();
        var totalPages = (int) Math.Ceiling(totalCount / (double) pageSize);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return new PaginatedEntity<Meteorite>()
        {
            Entities = items,
            TotalCount = totalCount,
            TotalPages = totalPages,
        };
    }

    public async Task<Meteorite?> GetByExternalIdAsync(string externalId)
    {
        return await DbSet
            .FirstOrDefaultAsync(e => e.ExternalId == externalId);
    }

    public async Task DeleteByExternalId(string externalId)
    {
        var entity = await GetByExternalIdAsync(externalId);
        if (entity is not null)
        {
            DbSet.Remove(entity);
        }
    }

    public async Task BulkDeleteByExternalIdAsync(IEnumerable<string> externalIds)
    {
        var meteoritesToDelete = DbSet.Where(e => externalIds.Contains(e.ExternalId));

        await meteoritesToDelete.ExecuteDeleteAsync();
    }
}