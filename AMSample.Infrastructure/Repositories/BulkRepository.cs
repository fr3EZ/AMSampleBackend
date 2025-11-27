namespace AMSample.Infrastructure.Repositories;

public class BulkRepository<TEntity>(DbContext context) : BaseRepository<TEntity>(context), IBulkRepository<TEntity>
    where TEntity : Entity
{
    public async Task BulkInsertAsync(IEnumerable<TEntity> entities)
    {
        var entitiesList = entities.ToList();

        await Context.AddRangeAsync(entitiesList);
    }

    public void BulkUpdate(IEnumerable<TEntity> entities)
    {
        var entitiesList = entities.ToList();

        Context.UpdateRange(entitiesList);
    }

    public async Task BulkDeleteAsync(IEnumerable<int> ids)
    {
        var entitiesToDelete = DbSet.Where(e => ids.Contains(e.Id));

        await entitiesToDelete.ExecuteDeleteAsync();
    }
}