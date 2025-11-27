namespace AMSample.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IMeteoriteRepository Meteorites { get; }
}