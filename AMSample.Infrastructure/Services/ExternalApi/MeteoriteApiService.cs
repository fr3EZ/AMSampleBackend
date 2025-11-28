namespace AMSample.Infrastructure.Services.ExternalApi;

public class MeteoriteApiService(
    HttpClient httpClient,
    IOptions<SyncDataConfig> syncDataConfig,
    ILogger<MeteoriteApiService> logger) : IMeteoriteApiService
{
    public async Task<HttpResponseMessage> GetMeteoritesResponse()
    {
        try
        {
            logger.LogInformation("Starting to fetch meteorites from {Source}", syncDataConfig.Value.Meteorites?.Source);

            var response = await httpClient.GetAsync(
                syncDataConfig.Value.Meteorites?.Source,
                HttpCompletionOption.ResponseHeadersRead);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                logger.LogWarning(
                    "HTTP request failed with status {StatusCode}. Response: {ErrorContent}",
                    response.StatusCode, errorContent);

                throw new HttpRequestException(
                    $"HTTP request failed with status code {response.StatusCode}. Response: {errorContent}",
                    null, response.StatusCode);
            }

            logger.LogInformation("Successfully received response with status {StatusCode}", response.StatusCode);

            return response;
        }
        catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
        {
            logger.LogError(ex, "HTTP error occurred while fetching meteorites. Status: {StatusCode}", ex.StatusCode);
            throw new HttpRequestException($"Failed to fetch meteorites: HTTP {(int) ex.StatusCode}", ex);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Network error occurred while fetching meteorites");
            throw new HttpRequestException("Network error while fetching meteorites", ex);
        }
    }
}