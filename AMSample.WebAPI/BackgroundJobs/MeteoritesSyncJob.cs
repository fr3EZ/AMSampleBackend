namespace AMSample.WebAPI.BackgroundJobs;

public class MeteoritesSyncJob(IMediator mediator, ILogger<MeteoritesSyncJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            logger.LogInformation("Starting meteorites sync job");
            
            await mediator.Send(new SyncMeteoritesCommand());
            
            logger.LogInformation("Meteorites sync job completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Meteorites sync job failed");
            throw;
        }
    }
}