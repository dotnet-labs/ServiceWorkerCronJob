namespace ServiceWorkerCronJobDemo.Services
{
    public class MyCronJob1 : CronJobService
    {
        private readonly ILogger<MyCronJob1> _logger;

        public MyCronJob1(IScheduleConfig<MyCronJob2> config, ILogger<MyCronJob1> logger)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;

        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CronJob 1 starts.");
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{now} CronJob 1 is working.", DateTime.Now.ToString("T"));
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CronJob 1 is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}
