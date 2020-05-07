using System;
using System.Threading.Tasks;
using Application.Infrastructure;
using Application.Observability;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Formatter;

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

                    services.AddSingleton(p =>
                    {

                        var appName = "application";
                        var clientId = $"{appName}-{Guid.NewGuid()}";
                        Console.Title = $"Application: {clientId}";
                        return new BctMqttClientConfiguration()
                        {
                            ClientId = clientId,
                        };
                    });

                    services.AddSingleton<IManagedMqttClient>(p =>
                    {
                        var mqttClientConfiguration = p.GetService<BctMqttClientConfiguration>();
                        var options = new ManagedMqttClientOptionsBuilder()

                            .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                            .WithClientOptions(new MqttClientOptionsBuilder()
                                .WithClientId(mqttClientConfiguration.ClientId)
                                .WithProtocolVersion(MqttProtocolVersion.V500)
                                .WithTcpServer("192.168.1.98", 1883)
                                //.WithTls()
                                .Build()
                            )

                            .Build();
                        var mqttClient = new MqttFactory().CreateManagedMqttClient();
                       
                        mqttClient.StartAsync(options).Wait();
                        return mqttClient;
                    });


                    services.AddMediatR(new[] {typeof(Program).Assembly});
                    //services.AddScoped(typeof(IPipelineBehavior<,>), typeof(MetricsPipelineBehavior<,>));
                    //services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DistributedTracingBehavior<,>));

                    //services.AddHostedService(provider => new MetricsHost("192.168.1.98",1200));
                    //services.AddHostedService(provider => new DistributedTracingHost(provider.GetService<ILoggerFactory>(), provider.GetService<IConfiguration>())); 
                    services.AddHostedService(provider => new MqttProcessorHost(provider));
                    services.AddHostedService(provider => new StatusSender(provider));
                });
    }
}