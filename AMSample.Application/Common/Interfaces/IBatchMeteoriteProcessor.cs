namespace AMSample.Application.Common.Interfaces;

public interface IBatchMeteoriteProcessor
{
    Task ProcessMeteoriteBatchesByStreamAsync(Stream stream);
}