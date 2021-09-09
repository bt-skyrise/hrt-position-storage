using System.Linq;
using System.Threading.Channels;
using HelsinkiRegionTransportPositionStorage.Recorder;
using MQTTnet;

// unbounded, as we don't want to miss any messages in case of transient errors
var channel = Channel.CreateUnbounded<MqttApplicationMessage>();

await PositionsMqttClient.Connect(
    messageHandler: async message => await channel.Writer.WriteAsync(message)
);

var positionsMongoDbStorage = new PositionsMongoDbStorage();

await channel.Reader
    .ReadAllAsync()
    .Select(PositionMessageMapper.TryMapToDocument)
    .Where(document => document is not null)
    .Buffer(1000)
    .ForEachAwaitAsync(positionsMongoDbStorage.StoreBatch);
