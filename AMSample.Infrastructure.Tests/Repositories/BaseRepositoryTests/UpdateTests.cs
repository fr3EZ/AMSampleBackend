using AMSample.Infrastructure.Tests.Repositories.BaseRepositoryTests.Initializer;
using AMSample.Infrastructure.Tests.Repositories.GeneralData.TestEntities;
using Microsoft.EntityFrameworkCore;

namespace AMSample.Infrastructure.Tests.Repositories.BaseRepositoryTests;

public class UpdateTests : TestBaseRepositoryInitializer
{
    [Fact]
    public async Task Update_WithExistingEntity_UpdatesEntity()
    {
        // Arrange
        SeedTestData(3);

        // Получаем сущность без отслеживания
        var entityToUpdate = await Context.TestEntities
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == 2);

        Assert.NotNull(entityToUpdate);

        entityToUpdate.Name = "Updated Name";
        entityToUpdate.Description = "Updated Description";
        entityToUpdate.Price = 999.99m;
        entityToUpdate.Quantity = 100;
        entityToUpdate.IsActive = false;

        // Act
        Repository.Update(entityToUpdate);
        await Repository.SaveChangesAsync();

        // Assert
        var updatedEntity = await Context.TestEntities
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == 2);

        Assert.NotNull(updatedEntity);
        Assert.Equal("Updated Name", updatedEntity.Name);
        Assert.Equal("Updated Description", updatedEntity.Description);
        Assert.Equal(999.99m, updatedEntity.Price);
        Assert.Equal(100, updatedEntity.Quantity);
        Assert.False(updatedEntity.IsActive);
    }

    [Fact]
    public async Task Update_WithTrackedEntity_UpdatesSuccessfully()
    {
        // Arrange
        SeedTestData(3);

        // Получаем сущность с отслеживанием
        var trackedEntity = await Context.TestEntities.FindAsync(2);
        Assert.NotNull(trackedEntity);

        trackedEntity.Name = "Updated Name";
        trackedEntity.Price = 50.50m;

        // Act
        Repository.Update(trackedEntity);
        await Repository.SaveChangesAsync();

        // Assert
        Context.ChangeTracker.Clear(); // Очищаем трекер для проверки
        var updatedEntity = await Context.TestEntities.FindAsync(2);
        Assert.NotNull(updatedEntity);
        Assert.Equal("Updated Name", updatedEntity.Name);
        Assert.Equal(50.50m, updatedEntity.Price);
    }

    [Fact]
    public async Task Update_WithPartialUpdate_UpdatesOnlyChangedProperties()
    {
        // Arrange
        SeedTestData(3);

        var entityToUpdate = await Context.TestEntities
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == 1);

        Assert.NotNull(entityToUpdate);

        var originalDescription = entityToUpdate.Description;
        var originalPrice = entityToUpdate.Price;

        entityToUpdate.Name = "Only Name Updated";

        // Act
        Repository.Update(entityToUpdate);
        await Repository.SaveChangesAsync();

        // Assert
        var updatedEntity = await Context.TestEntities
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == 1);

        Assert.NotNull(updatedEntity);
        Assert.Equal("Only Name Updated", updatedEntity.Name);
        Assert.Equal(originalDescription, updatedEntity.Description); // Не изменилось
        Assert.Equal(originalPrice, updatedEntity.Price); // Не изменилось
    }

    [Fact]
    public async Task Update_PreservesOtherEntities()
    {
        // Arrange
        SeedTestData(3);

        var entityToUpdate = await Context.TestEntities
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == 2);

        Assert.NotNull(entityToUpdate);

        entityToUpdate.Name = "Updated";

        // Act
        Repository.Update(entityToUpdate);
        await Repository.SaveChangesAsync();

        // Assert
        var otherEntity = await Context.TestEntities
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == 1);

        Assert.NotNull(otherEntity);
        Assert.Equal("Test Entity 1", otherEntity.Name); // Не изменилось
    }

    [Fact]
    public async Task Update_WithNullProperties_UpdatesToNull()
    {
        // Arrange
        SeedTestData(3);

        var entityToUpdate = await Context.TestEntities
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == 2);

        Assert.NotNull(entityToUpdate);

        entityToUpdate.Description = null;
        entityToUpdate.CategoryId = null;

        // Act
        Repository.Update(entityToUpdate);
        await Repository.SaveChangesAsync();

        // Assert
        var updatedEntity = await Context.TestEntities
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == 2);

        Assert.NotNull(updatedEntity);
        Assert.Null(updatedEntity.Description);
        Assert.Null(updatedEntity.CategoryId);
    }
}