namespace AMSample.Infrastructure.Tests.Repositories.BaseRepositoryTests;

public class DeleteTests : TestBaseRepositoryInitializer
{
    [Fact]
    public async Task Delete_WhenEntityExists_RemovesEntity()
    {
        // Arrange
        SeedTestData(5);
        var idToDelete = 3;
        var initialCount = await Context.TestEntities.CountAsync();

        // Act
        await Repository.Delete(idToDelete);
        await Repository.SaveChangesAsync();

        // Assert
        var finalCount = await Context.TestEntities.CountAsync();
        Assert.Equal(initialCount - 1, finalCount);

        var deletedEntity = await Context.TestEntities.FindAsync(idToDelete);
        Assert.Null(deletedEntity);
    }

    [Fact]
    public async Task Delete_WhenEntityDoesNotExist_DoesNothing()
    {
        // Arrange
        SeedTestData(3);
        var nonExistentId = 999;
        var initialCount = await Context.TestEntities.CountAsync();

        // Act
        await Repository.Delete(nonExistentId);
        await Repository.SaveChangesAsync();

        // Assert
        var finalCount = await Context.TestEntities.CountAsync();
        Assert.Equal(initialCount, finalCount); // Count remains the same
    }

    [Fact]
    public async Task Delete_PreservesOtherEntities()
    {
        // Arrange
        SeedTestData(3);
        var idToDelete = 2;

        // Act
        await Repository.Delete(idToDelete);
        await Repository.SaveChangesAsync();

        // Assert
        var remainingEntity1 = await Context.TestEntities.FindAsync(1);
        var remainingEntity3 = await Context.TestEntities.FindAsync(3);

        Assert.NotNull(remainingEntity1);
        Assert.NotNull(remainingEntity3);
        Assert.Equal("Test Entity 1", remainingEntity1.Name);
        Assert.Equal("Test Entity 3", remainingEntity3.Name);
    }

    [Fact]
    public async Task Delete_WithTrackedEntity_RemovesSuccessfully()
    {
        // Arrange
        SeedTestData(3);
        var entityToDelete = await Context.TestEntities.FindAsync(2);
        Assert.NotNull(entityToDelete);

        // Act
        await Repository.Delete(2);
        await Repository.SaveChangesAsync();

        // Assert
        var deletedEntity = await Context.TestEntities.FindAsync(2);
        Assert.Null(deletedEntity);
    }

    [Fact]
    public async Task Delete_EntityWithRelationships_HandlesCascadeCorrectly()
    {
        // Arrange
        SeedTestData(3);

        // Убедимся что у сущности есть CategoryId
        var entityWithCategory = await Context.TestEntities
            .FirstOrDefaultAsync(e => e.CategoryId.HasValue);
        Assert.NotNull(entityWithCategory);

        var entityId = entityWithCategory.Id;

        // Act
        await Repository.Delete(entityId);
        await Repository.SaveChangesAsync();

        // Assert
        var deletedEntity = await Context.TestEntities.FindAsync(entityId);
        Assert.Null(deletedEntity);

        // Категория должна остаться (DeleteBehavior.SetNull)
        var category = await Context.TestCategories
            .FindAsync(entityWithCategory.CategoryId);
        Assert.NotNull(category);
    }

    [Fact]
    public async Task Delete_WithZeroId_DoesNothing()
    {
        // Arrange
        SeedTestData(3);
        var initialCount = await Context.TestEntities.CountAsync();

        // Act
        await Repository.Delete(0);
        await Repository.SaveChangesAsync();

        // Assert
        var finalCount = await Context.TestEntities.CountAsync();
        Assert.Equal(initialCount, finalCount);
    }

    [Fact]
    public async Task Delete_WithNegativeId_DoesNothing()
    {
        // Arrange
        SeedTestData(3);
        var initialCount = await Context.TestEntities.CountAsync();

        // Act
        await Repository.Delete(-1);
        await Repository.SaveChangesAsync();

        // Assert
        var finalCount = await Context.TestEntities.CountAsync();
        Assert.Equal(initialCount, finalCount);
    }
}