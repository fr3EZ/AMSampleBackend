using AMSample.Domain.Entities;

namespace AMSample.Application.Interfaces;

public interface IUnitOfWork
{
    IRepository<Meteorite> MeteoritesRepository { get; }
    Task SaveAsync();
}