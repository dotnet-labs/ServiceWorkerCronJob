using ServiceWorkerCronJobDemo.Services;

namespace ServiceWorkerCronJobDemo
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddScoped<IMyScopedService, MyScopedService>();

            services.AddCronJob<MyCronJob1>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = "*/5 * * * *";
            });
            // MyCronJob2 calls the scoped service MyScopedService
            services.AddCronJob<MyCronJob2>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = "* * * * *";
            });
            services.AddCronJob<MyCronJob3>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = "50 12 * * *";
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
