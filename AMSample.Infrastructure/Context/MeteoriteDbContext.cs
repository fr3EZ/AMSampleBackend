using AMSample.Domain.Enums;

namespace AMSample.Infrastructure.Context;

public class MeteoriteDbContext : DbContext
{
    public MeteoriteDbContext(DbContextOptions<MeteoriteDbContext> options)
        : base(options)
    {
    }

    public DbSet<Meteorite> Meteorites { get; set; }
    public DbSet<Geolocation> Geolocations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        ConfigureMeteorite(modelBuilder);
        ConfigureGeolocation(modelBuilder);
    }

    private static void ConfigureMeteorite(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Meteorite>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(startValue: 1, incrementBy: 1)
                .HasMaxLength(50);

            entity.Property(e => e.ExternalId)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.NameType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(e => e.RecClass)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Mass)
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.Fall)
                .IsRequired()
                .HasConversion<int>()
                .HasMaxLength(20);

            entity.Property(e => e.Year)
                .HasColumnType("timestamp");

            entity.Property(e => e.RecLat)
                .HasColumnType("decimal(10,6)");

            entity.Property(e => e.RecLong)
                .HasColumnType("decimal(10,6)");

            entity.HasOne(e => e.Geolocation)
                .WithOne()
                .HasForeignKey<Meteorite>("GeolocationId")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ExternalId).IsUnique();
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.RecClass);
            entity.HasIndex(e => e.Fall);
            entity.HasIndex(e => e.Year);
            entity.HasIndex(e => new {e.RecLat, e.RecLong});

            entity.HasIndex(e => e.Name)
                .HasDatabaseName("IX_Meteorite_Name");
        });
    }

    private static void ConfigureGeolocation(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Geolocation>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Type)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(GeometryType.Point);

            entity.Property(e => e.Coordinates)
                .IsRequired()
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(decimal.Parse)
                        .ToArray()
                )
                .HasMaxLength(100);

            entity.HasIndex(e => e.Coordinates);
        });
    }
}