using System;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;


namespace JsonTests
{
    static class JsonTest
    {
        public static void SerializeTest(int numIterations, int numCpuCores, Random random)
        {
            long verifier = 0;

            var startTime = Stopwatch.GetTimestamp();

            for (var i = 0; i < numIterations; ++i)
            {
                var grid = Program.CreateRandomGrid(random);
                var serialized = JsonConvert.SerializeObject(grid);
                var deserialized = JsonConvert.DeserializeObject<Grid>(serialized);
                
                verifier += deserialized.Rows;
            }

            var endTime = Stopwatch.GetTimestamp();
            var deltaTime = (endTime - startTime) * 1000.0 / Stopwatch.Frequency;

            Console.WriteLine(
                "JSON. Total {0:F}ms. Average {1:F5}ms. Percentage {2:P}",
                deltaTime, deltaTime / numIterations, deltaTime / 1000.0 / numCpuCores);
        }

        public static void MemoryTest(int numIterations, Random random)
        {
            long size = 0;

            for (var i = 0; i < numIterations; ++i)
            {
                var grid = Program.CreateRandomGrid(random);
                var serialized = JsonConvert.SerializeObject(grid);

                var array = Encoding.UTF8.GetBytes(serialized);

                size += array.Length;
            }

            Console.WriteLine(
                "JSON. Total size {0}. Average size {1:F1}.",
                size, (float)size / numIterations);
        }
    }
}
