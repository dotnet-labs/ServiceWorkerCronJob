namespace ServiceWorkerCronJobDemo.Services
{
    public class MyCronJob3 : CronJobService
    {
        private readonly ILogger<MyCronJob3> _logger;

        public MyCronJob3(IScheduleConfig<MyCronJob3> config, ILogger<MyCronJob3> logger)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CronJob 3 starts.");
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{now} CronJob 3 is working.", DateTime.Now.ToString("T"));
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CronJob 3 is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}
