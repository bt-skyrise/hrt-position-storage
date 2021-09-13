using System;
using System.Security.Authentication;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Diagnostics;

namespace HelsinkiRegionTransportPositionStorage.Recorder
{
    public static class PositionsMqttClient
    {
        public static async Task Connect(Func<MqttApplicationMessage, Task> messageHandler)
        {
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithClientId(null) // do not keep state on the broker
                .WithCleanSession()
                .WithTls(new MqttClientOptionsBuilderTlsParameters
                {
                    UseTls = true,
                    SslProtocol = SslProtocols.Tls12
                })
                .WithTcpServer("mqtt.hsl.fi", 8883)
                .Build();
        
            var logger = new MqttNetLogger();
        
            logger.LogMessagePublished += (sender, args) =>
            {
                if (args.LogMessage.Level >= MqttNetLogLevel.Warning)
                {
                    Console.WriteLine(args.LogMessage.Message);
                }
        
                if (args.LogMessage.Level == MqttNetLogLevel.Error)
                {
                    Console.WriteLine(args.LogMessage.Exception);
                }
            };
        
            var factory = new MqttFactory(logger);
        
            var mqttClient = factory.CreateMqttClient();
        
            mqttClient.UseConnectedHandler(async _ =>
            {
                Console.WriteLine("### CONNECTED WITH MQTT SERVER ###");
        
                await mqttClient.SubscribeAsync("/hfp/v2/journey/ongoing/vp/bus/#");
        
                Console.WriteLine("### SUBSCRIBED ###");
            });
        
            mqttClient.UseDisconnectedHandler(async args =>
            {
                Console.WriteLine("### DISCONNECTED FROM MQTT SERVER ###");
        
                if (args.Exception is not null)
                {
                    Console.WriteLine(args.Exception);
                }
        
                try
                {
                    await mqttClient.ConnectAsync(mqttClientOptions);
                }
                catch
                {
                    Console.WriteLine("### RECONNECTING FAILED ###");
                }
            });
        
            mqttClient.UseApplicationMessageReceivedHandler(async args =>
            {
                try
                {
                    await messageHandler(args.ApplicationMessage);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }
            });
        
            await mqttClient.ConnectAsync(mqttClientOptions);
        }
    }
}