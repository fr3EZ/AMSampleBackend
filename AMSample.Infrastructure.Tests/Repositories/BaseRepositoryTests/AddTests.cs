namespace AMSample.Infrastructure.Tests.Repositories.BaseRepositoryTests;

public class AddTests : TestBaseRepositoryInitializer
{
    [Fact]
    public async Task Add_WithValidEntity_AddsToDatabase()
    {
        // Arrange
        var newEntity = new TestEntity
        {
            Name = "New Entity",
            Description = "New Description",
            Price = 99.99m,
            Quantity = 10,
            IsActive = true
        };

        // Act
        Repository.Add(newEntity);
        await Repository.SaveChangesAsync();

        // Assert
        var savedEntity = await Context.TestEntities
            .FirstOrDefaultAsync(e => e.Name == "New Entity");

        Assert.NotNull(savedEntity);
        Assert.Equal("New Entity", savedEntity.Name);
        Assert.Equal("New Description", savedEntity.Description);
        Assert.Equal(99.99m, savedEntity.Price);
        Assert.Equal(10, savedEntity.Quantity);
        Assert.True(savedEntity.IsActive);
        Assert.True(savedEntity.Id > 0); // ID должен быть сгенерирован
    }

    [Fact]
    public async Task Add_WithExistingData_PreservesExistingData()
    {
        // Arrange
        SeedTestData(3);
        var initialCount = await Context.TestEntities.CountAsync();
        var newEntity = new TestEntity {Name = "New Entity"};

        // Act
        Repository.Add(newEntity);
        await Repository.SaveChangesAsync();

        // Assert
        var finalCount = await Context.TestEntities.CountAsync();
        Assert.Equal(initialCount + 1, finalCount);

        var existingEntity = await Context.TestEntities.FindAsync(1);
        Assert.NotNull(existingEntity);
        Assert.Equal("Test Entity 1", existingEntity.Name);
    }

    [Fact]
    public async Task Add_WithMultipleEntities_AddsAllToDatabase()
    {
        // Arrange
        var initialCount = await Context.TestEntities.CountAsync();
        var entity1 = new TestEntity {Name = "Entity 1"};
        var entity2 = new TestEntity {Name = "Entity 2"};

        // Act
        Repository.Add(entity1);
        Repository.Add(entity2);
        await Repository.SaveChangesAsync();

        // Assert
        var finalCount = await Context.TestEntities.CountAsync();
        Assert.Equal(initialCount + 2, finalCount);

        var savedEntity1 = await Context.TestEntities
            .FirstOrDefaultAsync(e => e.Name == "Entity 1");
        var savedEntity2 = await Context.TestEntities
            .FirstOrDefaultAsync(e => e.Name == "Entity 2");

        Assert.NotNull(savedEntity1);
        Assert.NotNull(savedEntity2);
    }

    [Fact]
    public async Task Add_EntityIsTrackedAfterAddition()
    {
        // Arrange
        var newEntity = new TestEntity {Name = "Tracked Entity"};

        // Act
        Repository.Add(newEntity);
        await Repository.SaveChangesAsync();

        // Assert
        var entry = Context.ChangeTracker.Entries<TestEntity>()
            .FirstOrDefault(e => e.Entity.Id == newEntity.Id);
        Assert.NotNull(entry);
        Assert.Equal(EntityState.Unchanged, entry.State);
    }

    [Fact]
    public async Task Add_WithMaxLengthProperties_HandlesCorrectly()
    {
        // Arrange
        var longName = new string('A', 100); // Максимальная длина
        var longDescription = new string('B', 500); // Максимальная длина

        var newEntity = new TestEntity
        {
            Name = longName,
            Description = longDescription
        };

        // Act
        Repository.Add(newEntity);
        await Repository.SaveChangesAsync();

        // Assert
        var savedEntity = await Context.TestEntities
            .FirstOrDefaultAsync(e => e.Name == longName);

        Assert.NotNull(savedEntity);
        Assert.Equal(longName, savedEntity.Name);
        Assert.Equal(longDescription, savedEntity.Description);
    }
}