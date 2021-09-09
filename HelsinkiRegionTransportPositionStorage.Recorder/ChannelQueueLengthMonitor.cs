using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace HelsinkiRegionTransportPositionStorage.Recorder
{
    public static class ChannelQueueLengthMonitor
    {
        public static void Start<T>(Channel<T> channel)
        {
            // ReSharper disable once FunctionNeverReturns - it runs indefinitely by design
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    Console.WriteLine($"Current queue length: {channel.Reader.Count}");
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            });
        }
    }
}