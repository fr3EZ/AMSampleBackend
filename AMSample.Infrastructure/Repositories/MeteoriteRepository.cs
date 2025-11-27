namespace AMSample.Infrastructure.Repositories;

public sealed class MeteoriteRepository(MeteoriteDbContext context)
    : BulkRepository<Meteorite>(context), IMeteoriteRepository
{
    public async Task<IDictionary<string, Meteorite>> GetMeteoritesDictionaryByExternalIdsAsync(
        IEnumerable<string> externalIds)
    {
        var externalIdsList = externalIds.ToList();

        var meteorites = await context.Meteorites
            .AsNoTracking()
            .Where(m => externalIdsList.Contains(m.ExternalId))
            .ToListAsync();

        return meteorites
            .GroupBy(m => m.ExternalId)
            .ToDictionary(g => g.Key, g => g.First());
    }

    public async Task<IEnumerable<string>> GetMeteoriteExternalIdsAsync()
    {
        return await context.Meteorites
            .AsNoTracking()
            .Select(m => m.ExternalId)
            .ToArrayAsync();
    }

    public async Task<PaginatedEntity<Meteorite>> GetPaginatedMeteorites(
        int pageNumber,
        int pageSize,
        MeteoriteFilters filters)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        IQueryable<Meteorite> query = DbSet.Include(m => m.Geolocation);

        query = MeteoriteQueryHelper.ApplyFilters(query, filters);

        if (filters?.GroupBy != GroupByType.None)
        {
            return await MeteoriteQueryHelper.GetGroupedResults(query, pageNumber, pageSize, filters);
        }

        query = MeteoriteQueryHelper.ApplySorting(query, filters);

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