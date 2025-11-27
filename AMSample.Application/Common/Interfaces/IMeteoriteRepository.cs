namespace AMSample.Application.Common.Interfaces;

public interface IMeteoriteRepository : IBulkRepository<Meteorite>
{
    Task<IDictionary<string, Meteorite>> GetMeteoritesDictionaryByExternalIdsAsync(IEnumerable<string> externalIds);
    Task<IEnumerable<string>> GetMeteoriteExternalIdsAsync();
    Task<PaginatedEntity<Meteorite>> GetPaginatedMeteorites(int pageNumber, int pageSize,MeteoriteFilters filters);
    Task<Meteorite?> GetByExternalIdAsync(string externalId);
    Task DeleteByExternalId(string externalId);
    Task BulkDeleteByExternalIdAsync(IEnumerable<string> externalIds);
}