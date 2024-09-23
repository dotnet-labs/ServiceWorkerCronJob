using Cronos;

namespace ServiceWorkerCronJobDemo.Services
{
    public abstract class CronJobService(string cronExpression, TimeZoneInfo timeZoneInfo, ILogger logger) : IHostedService, IDisposable
    {
        private System.Timers.Timer? _timer;
        private readonly CronExpression _expression = CronExpression.Parse(cronExpression);

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("{jobName}: started with expression [{expression}].", GetType().Name, cronExpression);
            await ScheduleJob(cancellationToken);
        }

        protected virtual async Task ScheduleJob(CancellationToken cancellationToken)
        {
            var next = _expression.GetNextOccurrence(DateTimeOffset.Now, timeZoneInfo);
            if (next.HasValue)
            {
                logger.LogInformation("{jobName}: scheduled next run at {nextRun}", GetType().Name, next.ToString());
                var delay = next.Value - DateTimeOffset.Now;
                if (delay.TotalMilliseconds <= 0)   // prevent non-positive values from being passed into Timer
                {
                    await ScheduleJob(cancellationToken);
                    return;
                }
                _timer = new System.Timers.Timer(delay.TotalMilliseconds);
                _timer.Elapsed += async (_, _) =>
                {
                    try
                    {
                        _timer.Dispose(); // reset and dispose timer
                        _timer = null;

                        if (!cancellationToken.IsCancellationRequested)
                        {
                            await DoWork(cancellationToken);
                        }

                        if (!cancellationToken.IsCancellationRequested)
                        {
                            await ScheduleJob(cancellationToken); // reschedule next
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        logger.LogInformation("{LoggerName}: job received cancellation signal, stopping...", GetType().Name);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "{LoggerName}: an error happened during execution of the job", GetType().Name);
                    }
                };
                _timer.Start();
            }
            await Task.CompletedTask;
        }

        public virtual async Task DoWork(CancellationToken cancellationToken)
        {
            await Task.Delay(5000, cancellationToken);  // do the work
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("{jobName}: stopping...", GetType().Name);
            _timer?.Stop();
            await Task.CompletedTask;
            logger.LogInformation("{jobName}: stopped.", GetType().Name);
        }

        public virtual void Dispose()
        {
            _timer?.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    // ReSharper disable once UnusedTypeParameter
    public interface IScheduleConfig<T>
    {
        string CronExpression { get; set; }
        TimeZoneInfo TimeZoneInfo { get; set; }
    }

    public class ScheduleConfig<T> : IScheduleConfig<T>
    {
        public string CronExpression { get; set; } = string.Empty;
        public TimeZoneInfo TimeZoneInfo { get; set; } = TimeZoneInfo.Local;
    }

    public static class ScheduledServiceExtensions
    {
        public static IServiceCollection AddCronJob<T>(this IServiceCollection services, Action<IScheduleConfig<T>> options) where T : CronJobService
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), "Please provide Schedule Configurations.");
            }
            var config = new ScheduleConfig<T>();
            options.Invoke(config);
            if (string.IsNullOrWhiteSpace(config.CronExpression))
            {
                throw new ArgumentNullException(nameof(options), "Empty Cron Expression is not allowed.");
            }

            services.AddSingleton<IScheduleConfig<T>>(config);
            services.AddHostedService<T>();
            return services;
        }
    }
}
