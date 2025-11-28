namespace AMSample.Infrastructure.Tests.Repositories.BaseRepositoryTests;

public class GetPaginatedTests : TestBaseRepositoryInitializer
{
    [Fact]
    public async Task GetPaginated_WithValidPagination_ReturnsCorrectResults()
    {
        // Arrange
        SeedTestData(15);
        var pageNumber = 2;
        var pageSize = 5;

        // Act
        var result = await Repository.GetPaginated(pageNumber, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(15, result.TotalCount);
        Assert.Equal(3, result.TotalPages);
        Assert.Equal(5, result.Entities.Count());
    }

    [Fact]
    public async Task GetPaginated_WithFirstPage_ReturnsFirstPageResults()
    {
        // Arrange
        SeedTestData(8);
        var pageNumber = 1;
        var pageSize = 5;

        // Act
        var result = await Repository.GetPaginated(pageNumber, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(8, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
        Assert.Equal(5, result.Entities.Count());
    }

    [Fact]
    public async Task GetPaginated_WithLastPage_ReturnsLastPageResults()
    {
        // Arrange
        SeedTestData(8);
        var pageNumber = 2;
        var pageSize = 5;

        // Act
        var result = await Repository.GetPaginated(pageNumber, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(8, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
        Assert.Equal(3, result.Entities.Count());
    }

    [Fact]
    public async Task GetPaginated_WithPageNumberLessThanOne_UsesPageOne()
    {
        // Arrange
        SeedTestData(5);
        var pageNumber = 0;
        var pageSize = 10;

        // Act
        var result = await Repository.GetPaginated(pageNumber, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalPages);
        Assert.Equal(5, result.Entities.Count());
    }

    [Fact]
    public async Task GetPaginated_WithPageSizeLessThanOne_UsesDefaultPageSize()
    {
        // Arrange
        SeedTestData(15);
        var pageNumber = 1;
        var pageSize = 0;

        // Act
        var result = await Repository.GetPaginated(pageNumber, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(15, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
        Assert.Equal(10, result.Entities.Count());
    }

    [Fact]
    public async Task GetPaginated_EntitiesAreTrackedAsNoTracking()
    {
        // Arrange
        SeedTestData(3); // Убедимся что есть данные
        var pageNumber = 1;
        var pageSize = 10;

        // Act
        var result = await Repository.GetPaginated(pageNumber, pageSize);

        // Assert
        Assert.NotNull(result);
        var entities = result.Entities.ToList();
        Assert.NotEmpty(entities);

        var firstEntity = entities.First();
        Assert.NotNull(firstEntity);

        // Проверяем что сущности не отслеживаются - исправленная строка
        var entry = Context.ChangeTracker.Entries<TestEntity>() // Используем TestEntity вместо Meteorite
            .FirstOrDefault(e => e.Entity.Id == firstEntity.Id);
        Assert.Null(entry); // Entity should not be tracked
    }

    [Fact]
    public async Task GetPaginated_WithLargePageSize_ReturnsAllEntities()
    {
        // Arrange
        SeedTestData(25);
        var pageNumber = 1;
        var pageSize = 100;

        // Act
        var result = await Repository.GetPaginated(pageNumber, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(25, result.TotalCount);
        Assert.Equal(1, result.TotalPages);
        Assert.Equal(25, result.Entities.Count());
    }

    [Fact]
    public async Task GetPaginated_WithVeryLargePageNumber_ReturnsEmptyResults()
    {
        // Arrange
        SeedTestData(5);
        var pageNumber = 1000;
        var pageSize = 10;

        // Act
        var result = await Repository.GetPaginated(pageNumber, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.TotalCount);
        Assert.Equal(1, result.TotalPages);
        Assert.Empty(result.Entities);
    }
}