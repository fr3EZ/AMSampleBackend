namespace AMSample.Infrastructure.Configuration;

public class DatabaseConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public bool EnableRetryOnFailure { get; set; } = true;
    public int CommandTimeout { get; set; } = 30;
    public bool EnableDetailedErrors { get; set; }
    public bool EnableSensitiveDataLogging { get; set; }
    public int MaxRetryCount { get; set; } = 3;
    public int MaxRetryDelaySeconds { get; set; } = 5;
}