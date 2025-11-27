namespace AMSample.Infrastructure.UoW;

public class UnitOfWork(MeteoriteDbContext context) : IUnitOfWork
{
    public IMeteoriteRepository Meteorites { get; } = new MeteoriteRepository(context);
}