namespace AMSample.Application.Common.Interfaces;

public interface IMeteoriteRepository : IBulkRepository<Meteorite>
{
    Task<Dictionary<string, Meteorite>> GetMeteoritesDictionaryAsync();
    Task<IEnumerable<string>> GetMeteoriteExternalIdsAsync();
    Task<PaginatedEntity<Meteorite>> GetPaginatedMeteorites(int pageNumber, int pageSize,MeteoriteFilters filters);
    Task<Meteorite?> GetByExternalIdAsync(string externalId);
    Task DeleteByExternalId(string externalId);
    Task BulkDeleteByExternalIdAsync(IEnumerable<string> externalIds);
}