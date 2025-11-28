namespace AMSample.Infrastructure.Services;

public class RedisCacheService(IDistributedCache cache, IConnectionMultiplexer redis, IOptions<RedisConfig> redisConfig, ILogger<RedisCacheService> logger)
    : ICacheService
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cachedData = await cache.GetStringAsync(key, cancellationToken);
            if (string.IsNullOrEmpty(cachedData))
                return default;

            return JsonSerializer.Deserialize<T>(cachedData);
        }
        catch (Exception ex)
        {
            return default;
        }
    }
    
    private List<string> GetKeysByPatternAsync(string pattern)
    {
        var keys = new List<string>();
        
        try
        {
            var server = redis.GetServer(redis.GetEndPoints().First());
            keys = server.Keys(pattern: pattern).Select(k => k.ToString()).ToList();
            
            return keys;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting keys by pattern '{Pattern}'", pattern);
            return keys;
        }
    }

    public async Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var serializedData = JsonSerializer.Serialize(value);
            await cache.SetStringAsync(key, serializedData, options, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogInformation(ex, "Error setting data to Redis cache for key {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await cache.RemoveAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogInformation(ex, "Error removing data from Redis cache for key {Key}", key);
        }
    }
    
    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken)
    {
        try
        {
            var pattern = $"{redisConfig.Value.InstanceName}{prefix}:*";
            var keys = GetKeysByPatternAsync(pattern);
            
            var removedCount = 0;
            foreach (var key in keys)
            {
                await cache.RemoveAsync(key, cancellationToken);
                removedCount++;
            }
            
            logger.LogInformation("Removed {Count} cache keys with prefix '{Prefix}'", removedCount, prefix);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing cache keys with prefix '{Prefix}'", prefix);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var data = await cache.GetStringAsync(key, cancellationToken);
            return !string.IsNullOrEmpty(data);
        }
        catch
        {
            return false;
        }
    }

    public string GenerateKey<T>(string prefix, T parameters)
    {
        var parametersJson = JsonSerializer.Serialize(parameters, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(parametersJson));
        var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

        return $"{prefix}:{hashString}";
    }
}