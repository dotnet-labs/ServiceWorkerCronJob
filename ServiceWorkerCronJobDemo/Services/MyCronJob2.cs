namespace ServiceWorkerCronJobDemo.Services
{
    public class MyCronJob2 : CronJobService
    {
        private readonly ILogger<MyCronJob2> _logger;
        private readonly IServiceProvider _serviceProvider;

        public MyCronJob2(IScheduleConfig<MyCronJob2> config, ILogger<MyCronJob2> logger, IServiceProvider serviceProvider)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            if (config.GetType().GenericTypeArguments[0].Name != GetType().Name)
            {
                throw new ArgumentException("Incorrect JobType name for IScheduleConfig.");
            }
            if (logger.GetType().GenericTypeArguments[0].Name != GetType().Name)
            {
                throw new ArgumentException("Incorrect JobType name for ILogger.");
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CronJob 2 starts.");
            return base.StartAsync(cancellationToken);
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{now} CronJob 2 is working.", DateTime.Now.ToString("T"));
            using var scope = _serviceProvider.CreateScope();
            var svc = scope.ServiceProvider.GetRequiredService<IMyScopedService>();
            await svc.DoWork(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CronJob 2 is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}
