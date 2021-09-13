using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace HelsinkiRegionTransportPositionStorage.Recorder
{
    public class PositionsFileStorage
    {
        private readonly string _fileName;

        public PositionsFileStorage()
        {
            var createdDate = DateTime.Now.ToString("yyyy-MM-dd_HH-mm", CultureInfo.InvariantCulture);

            _fileName = $"hrt_positions_{createdDate}.jsonl";
        }
        
        public async Task StoreBatch(IList<string> jsonLines)
        {
            Console.WriteLine($"Writing {jsonLines.Count} messages");

            await File.AppendAllLinesAsync(_fileName, jsonLines);
        }
    }
}