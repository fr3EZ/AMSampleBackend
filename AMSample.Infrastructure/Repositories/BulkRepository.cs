namespace AMSample.Infrastructure.Repositories;

public class BulkRepository<TEntity>(DbContext context) : BaseRepository<TEntity>(context), IBulkRepository<TEntity>
    where TEntity : Entity
{
    public async Task BulkInsertAsync(IEnumerable<TEntity> entities)
    {
        await Context.AddRangeAsync(entities);
    }

    public void BulkUpdate(IEnumerable<TEntity> entities)
    {
        Context.UpdateRange(entities);
    }

    public async Task BulkDeleteAsync(IEnumerable<int> ids)
    {
        var entitiesToDelete = DbSet.Where(e => ids.Contains(e.Id));

        await entitiesToDelete.ExecuteDeleteAsync();
    }
}