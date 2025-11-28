namespace AMSample.Infrastructure.Tests.Repositories.BaseRepositoryTests.Initializer;

public abstract class TestBaseRepositoryInitializer : IDisposable
{
    protected readonly TestDbContext Context;
    protected readonly BaseRepository<TestEntity> Repository;

    protected TestBaseRepositoryInitializer()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new TestDbContext(options);

        SeedTestData();

        Repository = new BaseRepository<TestEntity>(Context);
    }

    protected void SeedTestData(int count = 10)
    {
        Context.TestEntities.RemoveRange(Context.TestEntities);
        Context.TestCategories.RemoveRange(Context.TestCategories);
        Context.SaveChanges();

        var categories = new[]
        {
            new TestCategory {Id = 1, Name = "Category 1"},
            new TestCategory {Id = 2, Name = "Category 2"},
            new TestCategory {Id = 3, Name = "Category 3"}
        };
        Context.TestCategories.AddRange(categories);

        var testEntities = new List<TestEntity>();
        for (int i = 1; i <= count; i++)
        {
            testEntities.Add(new TestEntity
            {
                Id = i,
                Name = $"Test Entity {i}",
                Description = $"Description {i}",
                CategoryId = (i % 3) + 1, // Распределяем по категориям
                Price = i * 10.0m,
                Quantity = i * 2,
                IsActive = i % 2 == 0 // Четные активны, нечетные нет
            });
        }

        Context.TestEntities.AddRange(testEntities);

        Context.SaveChanges();

        Context.ChangeTracker.Clear();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}