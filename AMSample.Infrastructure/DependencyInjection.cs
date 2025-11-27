using StackExchange.Redis;

namespace AMSample.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SyncDataConfig>(configuration.GetSection(nameof(SyncDataConfig)));

        var databaseConfig = configuration.GetSection(nameof(DatabaseConfig)).Get<DatabaseConfig>();
        var redisConfig = configuration.GetSection(nameof(RedisConfig)).Get<RedisConfig>();

        if (databaseConfig is not null)
        {
            services.AddDbContext<MeteoriteDbContext>(options =>
            {
                options.UseNpgsql(databaseConfig.ConnectionString, npgsqlOptions =>
                {
                    npgsqlOptions.CommandTimeout(databaseConfig.CommandTimeout);

                    if (databaseConfig.EnableRetryOnFailure)
                    {
                        npgsqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(5),
                            errorCodesToAdd: null);
                    }
                });

                if (databaseConfig.EnableDetailedErrors)
                    options.EnableDetailedErrors();

                if (databaseConfig.EnableSensitiveDataLogging)
                    options.EnableSensitiveDataLogging();
            });
        }

        if (redisConfig is not null)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConfig.ConnectionString;
                options.InstanceName = redisConfig.InstanceName;
            });
            
            services.AddSingleton<IConnectionMultiplexer>(sp => 
                ConnectionMultiplexer.Connect(redisConfig.ConnectionString));
        }

        services.AddHttpClient<IMeteoriteApiService, MeteoriteApiService>()
            .AddPolicyHandler(HttpClientPolicies.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicies.GetCircuitBreakerPolicy())
            .AddPolicyHandler(HttpClientPolicies.GetTimeoutPolicy());

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IRedisCacheService, RedisCacheService>();

        services.AddScoped<IMeteoriteApiService, MeteoriteApiService>();

        services.AddScoped<IBatchMeteoriteProcessor, BatchMeteoriteProcessor>();

        return services;
    }
}