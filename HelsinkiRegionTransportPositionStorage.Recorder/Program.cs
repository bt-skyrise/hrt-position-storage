using System.Linq;
using System.Threading.Channels;
using HelsinkiRegionTransportPositionStorage.Recorder;
using MQTTnet;

// unbounded, as we don't want to miss any messages in case of transient errors
var channel = Channel.CreateUnbounded<MqttApplicationMessage>();

ChannelQueueLengthMonitor.Start(channel);

await PositionsMqttClient.Connect(
    messageHandler: async message => await channel.Writer.WriteAsync(message)
);

var positionsFileStorage = new PositionsFileStorage();

await channel.Reader
    .ReadAllAsync()
    .Select(PositionJsonLineMapper.TryMapToJsonLine)
    .Where(document => document is not null)
    .Buffer(1000)
    .ForEachAwaitAsync(positionsFileStorage.StoreBatch);
