using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Polly;
using Polly.Retry;

namespace HelsinkiRegionTransportPositionStorage.Recorder
{
    public record PositionDocument(DateTime Timestamp, string Topic, string Payload);
    
    public class PositionsMongoDbStorage
    {
        private readonly MongoClient _client = new("mongodb://localhost:27017");
        
        private readonly AsyncRetryPolicy _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryForeverAsync(
                sleepDurationProvider: _ => TimeSpan.FromSeconds(1),
                onRetry: (exception, _) =>
                {
                    Console.WriteLine("Storing batch failed; retrying");
                    Console.WriteLine(exception);
                }
            );

        public async Task StoreBatch(IList<PositionDocument> positionMessages)
        {
            Console.WriteLine($"Storing {positionMessages.Count} messages");
            
            await _retryPolicy.ExecuteAsync(async () =>
            {
                await _client
                    .GetDatabase("positions")
                    .GetCollection<PositionDocument>("positions")
                    .InsertManyAsync(positionMessages);
            });
        }
    }
}