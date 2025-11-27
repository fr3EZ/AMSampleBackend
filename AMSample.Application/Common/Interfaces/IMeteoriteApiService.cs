namespace AMSample.Application.Common.Interfaces;

public interface IMeteoriteApiService
{
    Task<HttpResponseMessage> GetMeteoritesResponse();
}