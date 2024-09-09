namespace ULIT.ICAPI
{
    public class Program
    {
        private static IHost _host;
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true);
            var configuration = builder?.Build();

            _host = CreateHostBuilder(args).Build();

            using (var serviceScope = _host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {

                }
                catch (Exception ex)
                {
                }
            }

            _host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                    .UseKestrel(options =>
                    {
                        options.Limits.MaxRequestBodySize = null;
                    });
                }).ConfigureServices((context, services) =>
                {
                    //services.AddMetricsReportingHostedService();
                    //services.AddMetricsEndpoints(context.Configuration);
                    //services.AddMetricsTrackingMiddleware(context.Configuration);
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                });

    }
}