using AMSample.Infrastructure.Tests.Repositories.BaseRepositoryTests.Initializer;

namespace AMSample.Infrastructure.Tests.Repositories.BaseRepositoryTests;

public class GetByIdAsyncTests : TestBaseRepositoryInitializer
{
    [Fact]
    public async Task GetByIdAsync_WhenEntityExists_ReturnsEntity()
    {
        // Arrange
        SeedTestData(5);
        var expectedId = 3;

        // Act
        var result = await Repository.GetByIdAsync(expectedId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedId, result.Id);
        Assert.Equal("Test Entity 3", result.Name);
        Assert.Equal("Description 3", result.Description);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityDoesNotExist_ReturnsNull()
    {
        // Arrange
        SeedTestData(5);
        var nonExistentId = 999;

        // Act
        var result = await Repository.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithZeroId_ReturnsNull()
    {
        // Arrange
        SeedTestData(3);
        var zeroId = 0;

        // Act
        var result = await Repository.GetByIdAsync(zeroId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithNegativeId_ReturnsNull()
    {
        // Arrange
        SeedTestData(3);
        var negativeId = -1;

        // Act
        var result = await Repository.GetByIdAsync(negativeId);

        // Assert
        Assert.Null(result);
    }
}