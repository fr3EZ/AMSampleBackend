namespace AMSample.Application.Common.Interfaces;

public interface IBulkRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    Task BulkInsertAsync(IEnumerable<TEntity> entities);
    void BulkUpdate(IEnumerable<TEntity> entities);
    Task BulkDeleteAsync(IEnumerable<int> entityIds);
}