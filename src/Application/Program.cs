using Application.Messages;
using Application.Observability;
using Bct.Barcode;
using Bct.Common.NServiceBus;
using Bct.Common.NServiceBus.Metrics;
using Bct.Common.NServiceBus.Tracing;
using Bct.Common.Workflow.Aggregates;
using Bct.Common.Workflow.Aggregates.Implementation;
using BCT.Common.Logging.Extensions;
using Jaeger;
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.Extensions.Hosting.Host;

namespace Application
{
    class Program
    {
        public static async Task Main(string[] args)
        {

            Console.Title = $"Application: {Thread.CurrentThread.ManagedThreadId}";
            var configLog = new StringBuilder();
            await CreateHostBuilder(args, configLog)
               .Build()
               .LogHostConfiguration(configLog)
               .Do(_ => configLog.Clear())
               .RunAsync()
               .ConfigureAwait(false);

        }

        public static IHostBuilder CreateHostBuilder(string[] args, StringBuilder configLog) =>
           CreateDefaultBuilder(args)

                .ConfigureHostConfiguration(configBuilder =>
                {
                    new ConfigurationBuilder()
                    .AddEnvironmentVariables()
                    .AddAndWriteToConsole(configBuilder, redactFields: false, configurationLogging: configLog);
                })

                .ConfigureBCTLogging()
                //.UseJaegerTracing()
                //.UseBctMetrics()
                //.UseBctNServiceBus((configuration) => new HostingConfiguration()
                //{
                //    RabbitMqConnectionString = configuration["RABBITMQ_CONNECTION"],
                //    EndpointName = "BarcodeService",
                //    EnablePersistence = true,
                //    PersistenceType = configuration["PERSISTENCE_TYPE"],
                //    EnableOutbox = true,
                //    EnableMetrics = configuration["METRICS_ENABLED"] == "true",
                //    LimitMessageProcessingConcurrencyNumProcessors = null,
                //    ConnectionString = configuration["PERSISTENCE_CONNECTION"],
                //    SubscriptionPersisterCachingTimePeriodMinutes = 1,
                //    LoggingMinLevel = configuration["LOGGING_LEVEL"],
                //})

                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;
                    services.AddTransient<IBarcodeServiceFactory, BarcodeServiceFactory>(serviceProvider =>
                     new BarcodeServiceFactory(
                         serviceProvider.GetService<ILoggerFactory>(),
                         configuration["PERSISTENCE_TYPE"]));

                    // *** APP Component ***
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

                    // *** Mediator Component ***
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

                    // *** Mediator Component ***
                    services.AddMediatR(new[] { typeof(Program).Assembly });

                    // *** Messages Component *** 
                    // add all known messages from assemblyies using known message type
                    services.RegisterAssemblyPublicNonGenericClasses(new[] { Assembly.GetAssembly(typeof(DeviceStatus)) })
                        .Where(c => c.BaseType == typeof(BaseAggregate))
                        .AsPublicImplementedInterfaces();

                    services.AddSingleton<IMessageAdapter, MessageAdapter>();

                    // *** Mediator Component ***
                    services.AddScoped(typeof(IPipelineBehavior<,>), typeof(MetricsPipelineBehavior<,>));
                    //services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DistributedTracingBehavior<,>));

                    // *** Metrics Component *** (already exists)
                    services.AddHostedService(provider => new MetricsHost("localhost", 1200));
                    // *** Traceing componetnt *** (already exists)
                    //services.AddHostedService(provider => new DistributedTracingHost(provider.GetService<ILoggerFactory>(), provider.GetService<IConfiguration>())); 

                    // *** MQTT Component ***
                    services.AddHostedService<MqttProcessorHost>();

                    // *** APP Component ***
                    services.AddHostedService<StatusSender>();
                });
    }
}
