namespace ServiceWorkerCronJobDemo.Services
{
    public interface IMyScopedService
    {
        Task DoWork(CancellationToken cancellationToken);
    }

    public class MyScopedService(ILogger<MyScopedService> logger) : IMyScopedService
    {
        public Task DoWork(CancellationToken cancellationToken)
        {
            logger.LogInformation("{now} MyScopedService is working.", DateTime.Now.ToString("T"));
            return Task.Delay(1000 * 20, cancellationToken);
        }
    }
}
