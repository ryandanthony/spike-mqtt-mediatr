using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddHostedService(provider => new MetricsHost(1200)); 
                    //services.AddHostedService(provider => new DistributedTracingHost(provider.GetService<ILoggerFactory>(), provider.GetService<IConfiguration>())); 
                    services.AddHostedService(provider => new MqttProcessorHost(typeof(Program).Assembly));
                });
    }
}