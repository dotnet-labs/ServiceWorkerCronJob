namespace ServiceWorkerCronJobDemo.Services
{
    public class MyCronJob1(IScheduleConfig<MyCronJob1> config, ILogger<MyCronJob1> logger)
        : CronJobService(config.CronExpression, config.TimeZoneInfo)
    {
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("CronJob 1 starts.");
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            logger.LogInformation("{now} CronJob 1 is working.", DateTime.Now.ToString("T"));
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("CronJob 1 is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}
