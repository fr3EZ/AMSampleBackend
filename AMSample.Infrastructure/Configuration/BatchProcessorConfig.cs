namespace AMSample.Infrastructure.Configuration;

public class BatchProcessorConfig
{
    public int BatchSize { get; set; }
    public bool SaveAllPerOneTransaction { get; set; }
}