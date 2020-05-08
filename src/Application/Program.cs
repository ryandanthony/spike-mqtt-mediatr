using Application.Observability;
using Bct.Common.Workflow.Aggregates;
using Bct.Common.Workflow.Aggregates.Implementation;
using BCT.Common.Logging.Extensions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Formatter;
using NetCore.AutoRegisterDi;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Application
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            BaseAggregate.SetLogger(BCTLoggerService.GetLogger<BaseAggregate>());
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(config =>
                {
                    config.AddInMemoryCollection(new KeyValuePair<string, string>[] 
                    { 
                        new KeyValuePair<string, string>("LOGGING_LEVEL", "Debug")
                    });
                })
                .ConfigureBCTLogging()
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

                    // add all known messages from assemblyies using known message type
                    services.RegisterAssemblyPublicNonGenericClasses(new[] { Assembly.GetAssembly(typeof(DeviceStatus) )})
                        .Where(c => c.BaseType == typeof(BaseAggregate))
                        .AsPublicImplementedInterfaces();

                    services.AddScoped(typeof(IPipelineBehavior<,>), typeof(MetricsPipelineBehavior<,>));
                    //services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DistributedTracingBehavior<,>));

                    services.AddHostedService(provider => new MetricsHost("localhost",1200));
                    //services.AddHostedService(provider => new DistributedTracingHost(provider.GetService<ILoggerFactory>(), provider.GetService<IConfiguration>())); 
                    services.AddHostedService<MqttProcessorHost>();
                    services.AddHostedService<StatusSender>();
                });
    }
}