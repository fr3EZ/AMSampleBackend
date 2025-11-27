using AMSample.Domain.Entities;
using AMSample.Domain.Enums;

namespace AMSample.Infrastructure.Tests.Helpers.MeteoriteQueryHelperTests.TestData;

public class MeteoriteQueryHelperTestData
{
    private readonly List<Meteorite> _testMeteorites;

    protected MeteoriteQueryHelperTestData()
    {
        _testMeteorites = new List<Meteorite>
        {
            new Meteorite
            {
                Id = 1, Name = "Aachen", Year = new DateTime(1880, 1, 1), RecClass = "L5", Mass = 21m,
                Fall = FallType.Fell
            },
            new Meteorite
            {
                Id = 2, Name = "Aarhus", Year = new DateTime(1951, 1, 1), RecClass = "H5", Mass = 720m,
                Fall = FallType.Found
            },
            new Meteorite
            {
                Id = 3, Name = "Abee", Year = new DateTime(1952, 1, 1), RecClass = "EH4", Mass = 107000m,
                Fall = FallType.Fell
            },
            new Meteorite
            {
                Id = 4, Name = "Acapulco", Year = new DateTime(1976, 1, 1), RecClass = "Acapulcoite", Mass = 1914m,
                Fall = FallType.Fell
            },
            new Meteorite
            {
                Id = 5, Name = "Achiras", Year = new DateTime(1902, 1, 1), RecClass = "L6", Mass = 780m,
                Fall = FallType.Fell
            },
            new Meteorite
            {
                Id = 6, Name = "Adhi Kot", Year = new DateTime(1919, 1, 1), RecClass = "EH4", Mass = 4239m,
                Fall = FallType.Found
            },
            new Meteorite
            {
                Id = 7, Name = "Adzhi-Bogdo", Year = new DateTime(1949, 1, 1), RecClass = "LL3-6", Mass = 910m,
                Fall = FallType.Fell
            },
            new Meteorite
            {
                Id = 8, Name = "Agen", Year = new DateTime(1814, 1, 1), RecClass = "H5", Mass = 30000m,
                Fall = FallType.Fell
            },
            new Meteorite
            {
                Id = 9, Name = "Aguada", Year = new DateTime(1930, 1, 1), RecClass = "L6", Mass = 1620m,
                Fall = FallType.Found
            },
            new Meteorite
            {
                Id = 10, Name = "Aguila Blanca", Year = new DateTime(1920, 1, 1), RecClass = "L", Mass = 1440m,
                Fall = FallType.Found
            },
            new Meteorite
                {Id = 11, Name = "No Year", RecClass = "Unknown", Mass = 100m, Fall = FallType.Found}
        };
    }

    protected IQueryable<Meteorite> GetTestQueryable() => _testMeteorites.AsQueryable();
}