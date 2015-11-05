using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace JsonTests
{
    public static class BsonTest
    {
        public static void SerializeTest(int numIterations, int numCpuCores, Random random)
        {
            long verifier = 0;

            var startTime = Stopwatch.GetTimestamp();

            for (var i = 0; i < numIterations; ++i)
            {
                var grid = Program.CreateRandomGrid(random);
                var serialized = BsonSerializeObject(grid);
                var deserialized = BsonDeserializeObject<Grid>(serialized);

                verifier += deserialized.Rows;
            }

            var endTime = Stopwatch.GetTimestamp();
            var deltaTime = (endTime - startTime) * 1000.0 / Stopwatch.Frequency;

            Console.WriteLine(
                "BSON. Total {0:F}ms. Average {1:F5}ms. Percentage {2:P}",
                deltaTime, deltaTime / numIterations, deltaTime / 1000.0 / numCpuCores); ;
        }

        public static void MemoryTest(int numIterations, Random random)
        {
            long size = 0;

            for (var i = 0; i < numIterations; ++i)
            {
                var grid = Program.CreateRandomGrid(random);
                var serialized = BsonSerializeObject(grid);

                size += serialized.Length;
            }

            Console.WriteLine(
                "BSON. Total size {0}. Average size {1:F1}.",
                size, (float)size / numIterations);
        }

        static readonly JsonSerializer _JsonSerializer = new JsonSerializer();
        
        static byte[] BsonSerializeObject(object target)
        {
            var memoryStream = new MemoryStream();
            var writer = new BsonWriter(memoryStream);
            
            _JsonSerializer.Serialize(writer, target);

            return memoryStream.ToArray();
        }

        static T BsonDeserializeObject<T>(byte[] data)
        {
            var memoryStream = new MemoryStream(data);
            var reader = new BsonReader(memoryStream);

            return _JsonSerializer.Deserialize<T>(reader);
        }
    }
}
