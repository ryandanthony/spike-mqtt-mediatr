using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Options;
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
        static async Task Main(string[] args)
        {
            var running = true;
            var are = new AutoResetEvent(false);
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                running = false;
                eventArgs.Cancel = true;
                are.Set();
            };

            var deviceType = "myDevice";
            //var deviceUniqueId = Guid.NewGuid();
            var deviceUniqueId = "a";
            var deviceId = $"{deviceType}-{deviceUniqueId}";
            Console.Title = $"Device: {deviceId}";

            var options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithPendingMessagesOverflowStrategy(MqttPendingMessagesOverflowStrategy.DropOldestQueuedMessage)
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithCommunicationTimeout(TimeSpan.FromSeconds(15))
                    .WithProtocolVersion(MqttProtocolVersion.V500)
                    .WithClientId(deviceId)
                    .WithTcpServer("localhost", 1883)
                    //.WithTls()
                    .Build()
                )
                .Build();


            using (var cancellationTokenSource = new CancellationTokenSource())
            using (var mqttClient = new MqttFactory().CreateManagedMqttClient())
            {
                //await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic("bct/topic").Build());
                await mqttClient.StartAsync(options);
                await Task.Run(async () =>
                {
                    while (running)
                    {
                        var status = new Status()
                        {
                            Value = "",
                            When = DateTimeOffset.Now,
                            MessageId = Guid.NewGuid(),
                        };
                        await mqttClient.PublishAsync(builder =>
                                builder
                                    .WithTopic($"bct/{deviceType}/{deviceId}/status")
                                    .WithUserProperty("messageType", nameof(Status))
                                    .WithUserProperty("serialization", "json")
                                    .WithUserProperty("encoding", "utf8")
                                    .WithMessage(status)
                            , cancellationTokenSource.Token);


                        Console.WriteLine($"Publishing Status Message [{status.MessageId}]");
                        await Task.Delay(1000, cancellationTokenSource.Token);
                    }
                }, cancellationTokenSource.Token);
                are.WaitOne();
                await mqttClient.StopAsync();
            }
        }
    }
}