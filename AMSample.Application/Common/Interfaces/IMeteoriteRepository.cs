namespace AMSample.Application.Common.Interfaces;

public interface IMeteoriteRepository : IBulkRepository<Meteorite>
{
    Task<Dictionary<string, Meteorite>> GetMeteoritesDictionaryByExternalIdsAsync(IEnumerable<string> externalIds);
    Task<IEnumerable<string>> GetMeteoriteExternalIdsAsync();
    Task<PaginatedEntity<Meteorite>> GetPaginatedMeteorites(int pageNumber, int pageSize,MeteoriteFiltersDto filtersDto);
    Task<Meteorite?> GetByExternalIdAsync(string externalId);
    Task DeleteByExternalId(string externalId);
    Task BulkDeleteByExternalIdAsync(IEnumerable<string> externalIds);
}