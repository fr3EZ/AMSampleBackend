using System.Linq.Expressions;

namespace AMSample.Application.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
    Task Add(TEntity obj);   
    Task Update(TEntity obj);
    Task Remove(string id);
    Task<TEntity?> GetById(string id);
    Task<IEnumerable<TEntity>> GetFiltered(Expression<Func<TEntity, bool>>? specification = null);
    Task<IEnumerable<TEntity>> GetPaginated(int  pageNumber, int pageSize);
}