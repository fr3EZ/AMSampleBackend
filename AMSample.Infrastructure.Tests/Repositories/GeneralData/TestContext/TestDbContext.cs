namespace AMSample.Infrastructure.Tests.Repositories.GeneralData.TestContext;

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public DbSet<TestEntity> TestEntities { get; set; } = null!;
    public DbSet<TestCategory> TestCategories { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TestEntity>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.Property(e => e.Price)
                .HasPrecision(18, 2);

            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.CategoryId);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.TestEntities)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<TestCategory>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(e => e.Name)
                .IsUnique(); // Уникальное имя для категории
        });

        SeedTestData(modelBuilder);
    }

    private void SeedTestData(ModelBuilder modelBuilder)
    {
        var categories = new[]
        {
            new TestCategory {Id = 1, Name = "Category 1"},
            new TestCategory {Id = 2, Name = "Category 2"},
            new TestCategory {Id = 3, Name = "Category 3"}
        };

        var testEntities = new[]
        {
            new TestEntity
            {
                Id = 1, Name = "Test Entity 1", Description = "Description 1", CategoryId = 1, Price = 10.50m,
                Quantity = 5, IsActive = true
            },
            new TestEntity
            {
                Id = 2, Name = "Test Entity 2", Description = "Description 2", CategoryId = 1, Price = 20.00m,
                Quantity = 3, IsActive = true
            },
            new TestEntity
            {
                Id = 3, Name = "Test Entity 3", Description = "Description 3", CategoryId = 2, Price = 15.75m,
                Quantity = 8, IsActive = false
            },
            new TestEntity
            {
                Id = 4, Name = "Test Entity 4", Description = "Description 4", CategoryId = 2, Price = 30.25m,
                Quantity = 2, IsActive = true
            },
            new TestEntity
            {
                Id = 5, Name = "Test Entity 5", Description = "Description 5", CategoryId = 3, Price = 5.99m,
                Quantity = 15, IsActive = true
            }
        };

        modelBuilder.Entity<TestCategory>().HasData(categories);
        modelBuilder.Entity<TestEntity>().HasData(testEntities);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}