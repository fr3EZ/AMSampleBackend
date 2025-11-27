namespace AMSample.Domain.Common;

public class PaginatedEntity<TEntity> where TEntity : class
{
    public IEnumerable<TEntity>? Entities { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public IEnumerable<GroupResult>? GroupedResults { get; set; }
}