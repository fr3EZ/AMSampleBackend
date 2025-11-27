namespace AMSample.WebAPI.BackgroundJobs;

public class MeteoritesSyncJob(IMediator mediator, ILogger<MeteoritesSyncJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await mediator.Send(new SyncMeteoritesCommand());
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
        }
    }
}