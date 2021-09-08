using System;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using HelsinkiRegionTransportPositionStorage.Recorder;
using MQTTnet;

var channel = Channel.CreateBounded<MqttApplicationMessage>(
    new BoundedChannelOptions(capacity: 100)
);

await PositionsMqttClient.Connect(
    messageHandler: async message => await channel.Writer.WriteAsync(message)
);

var positionsMongoDbStorage = new PositionsMongoDbStorage();

await channel.Reader
    .ReadAllAsync()
    .Select(message => new PositionDocument(
        Timestamp: DateTime.UtcNow,
        Topic: message.Topic,
        AsciiPayload: Encoding.ASCII.GetString(message.Payload)
    ))
    .Buffer(1000)
    .ForEachAwaitAsync(positionsMongoDbStorage.StoreBatch);
