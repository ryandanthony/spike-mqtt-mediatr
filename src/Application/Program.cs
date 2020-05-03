using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Formatter;
using Newtonsoft.Json;
using Spike.Common;
using Spike.Messages;

namespace Application
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var are = new AutoResetEvent(false);
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                are.Set();
            };

            var appName = "application";
            var clientId = $"{appName}-{Guid.NewGuid()}";
            Console.Title = $"Application: {clientId}";
            var options = new ManagedMqttClientOptionsBuilder()
                
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId(clientId)
                    .WithProtocolVersion(MqttProtocolVersion.V500)
                    .WithTcpServer("localhost", 1883)
                    //.WithTls()
                    .Build()
                )
                
                .Build();

            using (var cancellationTokenSource = new CancellationTokenSource())
            using (var mqttClient = new MqttFactory().CreateManagedMqttClient())
            {
                var deviceType = "+";
                var deviceId = "+";
                //var deviceType = "myDevice";
                //var deviceId = "myDevice-a";
                var topic = $"$share/{appName}/bct/{deviceType}/{deviceId}/status";
                //var topic = $"bct/{deviceType}/{deviceId}/status";
                await mqttClient.SubscribeAsync(new TopicFilterBuilder()
                    .WithTopic(topic).Build());
                mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(OnConnected);
                mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnDisconnected);
                mqttClient.ConnectingFailedHandler = new ConnectingFailedHandlerDelegate(OnConnectingFailed);
                
                mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(OnMessageReceived);
                mqttClient.SynchronizingSubscriptionsFailedHandler = new SynchronizingSubscriptionsFailedHandlerDelegate(OnSynchronizingSubscriptionsFailed);
                mqttClient.ApplicationMessageSkippedHandler = new ApplicationMessageSkippedHandlerDelegate(OnMessageSkipped);

                await mqttClient.StartAsync(options);
                are.WaitOne();
                await mqttClient.StopAsync();
            }
        }

        private static Task OnSynchronizingSubscriptionsFailed(ManagedProcessFailedEventArgs e)
        {
            Console.WriteLine($"OnSynchronizingSubscriptionsFailed [Exception]:       {e.Exception}");
            return Task.CompletedTask;
        }

        private static Task OnMessageSkipped(ApplicationMessageSkippedEventArgs e)
        {
            Console.WriteLine($"OnMessageSkipped");
            return Task.CompletedTask;
        }

        private static Task OnConnected(MqttClientConnectedEventArgs e)
        {
            Console.WriteLine($"OnConnected [AuthenticateResult]:       {e.AuthenticateResult}");
            return Task.CompletedTask;
        }

        private static Task OnDisconnected(MqttClientDisconnectedEventArgs e)
        {
            Console.WriteLine($"OnDisconnected [ClientWasConnected]:       {e.ClientWasConnected}");
            Console.WriteLine($"OnDisconnected [AuthenticateResult]:       {e.AuthenticateResult}");
            Console.WriteLine($"OnDisconnected [Exception]:       {e.Exception}");
            return Task.CompletedTask;
        }

        private static Task OnConnectingFailed(ManagedProcessFailedEventArgs e)
        {
            Console.WriteLine($"OnConnectingFailed [Exception]:       {e.Exception}");
            return Task.CompletedTask;
        }

        private static Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs arg)
        {
            Console.WriteLine($"FROM:   {arg.ApplicationMessage.Topic}");
            Console.WriteLine($"        processingFailed: {arg.ProcessingFailed}");
            Console.WriteLine($"        messageType: {arg.ApplicationMessage.UserProperties?.FirstOrDefault(p => p.Name == "messageType")?.Value}");
            Console.WriteLine($"        serialization: {arg.ApplicationMessage.UserProperties?.FirstOrDefault(p => p.Name == "serialization")?.Value}");
            Console.WriteLine($"        encoding: {arg.ApplicationMessage.UserProperties?.FirstOrDefault(p => p.Name == "encoding")?.Value}");
            Console.WriteLine($"        bytes: {arg.ApplicationMessage?.Payload?.LongLength}");

            return Task.CompletedTask;
        }
    }
}
