namespace AMSample.Application.Common.Entities;

public class PaginatedEntity<TEntity> where TEntity : class
{
    public IEnumerable<TEntity>? Entities { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public IEnumerable<EntityGroup>? EntityGroups { get; set; }
}