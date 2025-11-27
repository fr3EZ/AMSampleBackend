using AMSample.Domain.Common;

namespace AMSample.Application.Common.Interfaces;

public interface IBaseRepository<TEntity> where TEntity : class
{
    Task<PaginatedEntity<TEntity>> GetPaginated(int pageNumber, int pageSize);
    Task<TEntity?> GetByIdAsync(int id);
    void Add(TEntity entity);
    void Update(TEntity entity);
    Task Delete(int id);
    Task SaveChangesAsync();
}