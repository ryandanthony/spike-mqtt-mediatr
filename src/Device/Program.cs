using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Bct.Common.Workflow.Aggregates;
using Bct.Common.Workflow.Aggregates.Implementation;
using BCT.Common.Logging.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Formatter;
using MQTTnet.Server;
using Newtonsoft.Json;
using Spike.Common;
using Spike.Messages;

namespace Device
{
    class Program
    {
        static byte[] _correlation;

        static async Task Main(string[] args)
        {
            // TODO refactor into proper app with DI
            var settingsConfiguration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            settingsConfiguration.ConfigureBCTLogging();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(configure => configure.AddBCTLogging());
            serviceCollection.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Trace);
            IServiceProvider m_serviceProvider = serviceCollection.BuildServiceProvider();
            var logger = m_serviceProvider.GetService<ILogger<BaseAggregate>>();
            BaseAggregate.SetLogger(logger);
            
  
            logger.WithInformation("Logging initialized").Log();
            logger.WithError("error").Log();

            var running = true;
            var are = new AutoResetEvent(false);
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                running = false;
                eventArgs.Cancel = true;
                are.Set();
            };

            var deviceType = "device-type-1";
            var deviceUniqueId = Guid.NewGuid();
            //var deviceUniqueId = "a";
            var deviceId = $"{deviceType}-{deviceUniqueId}";
            Console.Title = $"Device: {deviceId}";

            var options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithPendingMessagesOverflowStrategy(MqttPendingMessagesOverflowStrategy.DropOldestQueuedMessage)
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithCommunicationTimeout(TimeSpan.FromSeconds(15))
                    .WithProtocolVersion(MqttProtocolVersion.V500)
                    .WithClientId(deviceId)
                    .WithTcpServer("192.168.1.98", 1883)
                    //.WithTls()
                    .Build()
                )
                .Build();


            using (var cancellationTokenSource = new CancellationTokenSource())
            using (var mqttClient = new MqttFactory().CreateManagedMqttClient())
            {
                await mqttClient.SubscribeAsync(
                    new TopicFilterBuilder().WithTopic("bct/app/status").Build(),
                    new TopicFilterBuilder().WithTopic($"bct/{deviceType}/{deviceId}/response").Build(),
                    new TopicFilterBuilder().WithTopic($"bct/{deviceType}/{deviceId}/in").Build());
                mqttClient.ApplicationMessageReceivedHandler =
                    new MqttApplicationMessageReceivedHandlerDelegate(OnMessageReceived);
                await mqttClient.StartAsync(options);

                var initializeMessage = new InitializeConnection();
                initializeMessage.DeviceId.Value = deviceId;
                var payload = Encoding.UTF8.GetBytes(initializeMessage.Serialize());
                _correlation = Guid.NewGuid().ToByteArray();

                await mqttClient.PublishAsync(builder =>
                        builder
                            .WithTopic($"bct/{deviceType}/{deviceId}/out")
                            .WithResponseTopic($"bct/{deviceType}/{deviceId}/response")
                            .WithCorrelationData(_correlation)
                            .WithUserProperty("messageType", nameof(InitializeConnection))
                            .WithUserProperty("responseType", nameof(InitializeConnectionResponse))
                            .WithPayload(payload)
                    , cancellationTokenSource.Token);

                Console.WriteLine($"Publishing InitializeConnection Message");

                await Task.Run(async () =>
                {
                    while (running)
                    {
                        var messageId = Guid.NewGuid();
                        var devStatus = new DeviceStatus();
                        devStatus.Condition.Value = StatusEnum.Connected;
                        devStatus.DeviceId.Value = deviceId;
                        devStatus.MessageId.Value = messageId.ToString();
                        devStatus.When.Value = Convert.ToDouble(DateTime.UtcNow.Ticks);
                        var payload = Encoding.UTF8.GetBytes(devStatus.Serialize());
                        await mqttClient.PublishAsync(builder =>
                                builder
                                    .WithTopic($"bct/{deviceType}/{deviceId}/status")
                                    .WithUserProperty("messageType", nameof(DeviceStatus))
                                    .WithPayload(payload)
                                    .WithContentType("application/json")
                            , cancellationTokenSource.Token);

                        await Task.Delay(5000, cancellationTokenSource.Token);
                    }
                }, cancellationTokenSource.Token);
                are.WaitOne();
                await mqttClient.StopAsync();
            }
        }

        private static Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs arg)
        {
            var messageTypeString = arg.ApplicationMessage
                .UserProperties?
                .FirstOrDefault(p => p.Name == "messageType")?
                .Value;

            Console.WriteLine(messageTypeString);

            switch (messageTypeString)
            {
                case "InitializeConnectionResponse":
                    var json = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
                    var agg = BaseAggregate.Deserialize<InitializeConnectionResponse>(json);
                    if (arg.ApplicationMessage.CorrelationData != null && _correlation.SequenceEqual(arg.ApplicationMessage.CorrelationData))
                    {
                        Console.WriteLine($"MessageType:{messageTypeString}  RequestApproved:{agg.RequestApproved.Value}  Correlation matches");
                    }
                    else
                    {
                        Console.WriteLine("Correlation data mismatched");
                    }
                    break;
                default:
                    break;
            }            
            return Task.CompletedTask;
        }
    }
}