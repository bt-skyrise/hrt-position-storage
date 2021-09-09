using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MQTTnet;

namespace HelsinkiRegionTransportPositionStorage.Recorder
{
    public static class PositionMessageMapper
    {
        public static PositionDocument TryMapToDocument(this MqttApplicationMessage message)
        {
            try
            {
                var payload = Encoding.UTF8.GetString(message.Payload);

                var positionMessage = JsonSerializer.Deserialize<PositionMessage>(payload);

                var timestampOrNull = positionMessage?.VehiclePosition?.Timestamp;

                if (timestampOrNull is null)
                {
                    Console.WriteLine("Timestamp is missing from the message! Skipping.");
                    
                    return null;
                }

                return new PositionDocument(
                    Timestamp: timestampOrNull.Value,
                    Topic: message.Topic,
                    Payload: Encoding.ASCII.GetString(message.Payload)
                );
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
        
        private class PositionMessage
        {
            [JsonPropertyName("VP")]
            public VehiclePosition VehiclePosition { get; init; }
        }

        private class VehiclePosition
        {
            [JsonPropertyName("tst")]
            public DateTime? Timestamp { get; init; }
        }
    }
}