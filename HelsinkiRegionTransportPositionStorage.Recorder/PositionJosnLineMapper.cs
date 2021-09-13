using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MQTTnet;

namespace HelsinkiRegionTransportPositionStorage.Recorder
{
    public static class PositionJsonLineMapper
    {
        public static string TryMapToJsonLine(this MqttApplicationMessage message)
        {
            try
            {
                var payload = Encoding.UTF8.GetString(message.Payload);

                var payloadJsonDocument = JsonDocument.Parse(payload);

                var timestamp = payloadJsonDocument.RootElement
                    .GetProperty("VP")
                    .GetProperty("tst")
                    .GetDateTime();

                var jsonLine = new PositionJsonLine
                {
                    Timestamp = timestamp,
                    Topic = message.Topic,
                    Payload = payloadJsonDocument
                };

                return JsonSerializer.Serialize(jsonLine, new JsonSerializerOptions
                {
                    WriteIndented = false
                });
            }
            catch (Exception exception)
            {
                Console.WriteLine("Failed to map message:");
                Console.WriteLine(exception);

                return null;
            }
        }
        
        // ReSharper disable ClassNeverInstantiated.Local - they are via deserialization
        // ReSharper disable UnusedAutoPropertyAccessor.Local - they are via deserialization

        private class PositionJsonLine
        {
            public DateTime Timestamp { get; set; }
            public string Topic { get; set; }
            public JsonDocument Payload { get; set; }
        }
    }
}