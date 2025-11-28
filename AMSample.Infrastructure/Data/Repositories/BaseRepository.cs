namespace AMSample.Infrastructure.Data.Repositories;

public class BaseRepository<TEntity>(DbContext context) : IBaseRepository<TEntity>
    where TEntity : Entity
{
    protected readonly DbContext Context = context;
    protected readonly DbSet<TEntity> DbSet = context.Set<TEntity>();

    public async Task<PaginatedEntity<TEntity>> GetPaginated(int pageNumber, int pageSize)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var totalCount = await DbSet.CountAsync();
        var totalPages = (int) Math.Ceiling(totalCount / (double) pageSize);

        var items = await DbSet
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return new PaginatedEntity<TEntity>()
        {
            Entities = items,
            TotalCount = totalCount,
            TotalPages = totalPages,
        };
    }

    public async Task<TEntity?> GetByIdAsync(int id)
    {
        return await DbSet
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public void Add(TEntity entity)
    {
        DbSet.AddAsync(entity);
    }

    public void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    public async Task Delete(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity is not null)
        {
            DbSet.Remove(entity);
        }
    }

    public async Task SaveChangesAsync()
    {
        await Context.SaveChangesAsync();
    }
}