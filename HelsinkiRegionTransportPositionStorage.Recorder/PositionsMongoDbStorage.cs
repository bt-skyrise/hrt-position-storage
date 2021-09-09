using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace HelsinkiRegionTransportPositionStorage.Recorder
{
    public record PositionDocument(DateTime Timestamp, string Topic, string Payload);

    public class PositionsMongoDbStorage
    {
        private readonly MongoClient _client;

        public PositionsMongoDbStorage()
        {
            _client = new MongoClient("mongodb://localhost:27017");
        }

        public async Task StoreBatch(IList<PositionDocument> positionMessages)
        {
            Console.WriteLine($"Storing {positionMessages.Count} messages");
            
            await _client
                .GetDatabase("positions")
                .GetCollection<PositionDocument>("positions")
                .InsertManyAsync(positionMessages);
        }
    }
}