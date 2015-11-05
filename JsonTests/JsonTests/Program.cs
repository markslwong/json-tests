using System;
using System.Collections.Generic;
using System.Threading;


namespace JsonTests
{
    public class Tile
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int RowSpan { get; set; }
        public int ColumnSpan { get; set; }
    }

    public class Grid
    {
        public string Name { get; set; }

        public int Rows { get; set; }
        public int Columns { get; set; }

        public IEnumerable<Tile> Tiles { get; set; }
    }

    public class InvokeTarget
    {
        public void ToInvoke(string value)
        {
            Invokes++;
            Length += value.Length;
        }

        public int Invokes { get; private set; }
        public int Length { get; private set; }
    }

    public static class Program
    {
        static void Main(string[] args)
        {
            const int randomSeed = 0;
            const int numIterations = 60;
            const int numCpuCores = 4;

            var random1 = new Random(randomSeed);
            var random2 = new Random(randomSeed);
            var random3 = new Random(randomSeed);

            Console.WriteLine("Forcing JIT Compile");
            BinaryTest.SerializeTest(1, numCpuCores, random1);
            JsonTest.SerializeTest(1, numCpuCores, random2);
            BsonTest.SerializeTest(1, numCpuCores, random3);
            BinaryTest.MemoryTest(1, random1);
            JsonTest.MemoryTest(1, random2);
            BsonTest.MemoryTest(1, random3);
            
            Console.Clear();

            Thread.Sleep(2000);
            
            Console.WriteLine("Running Serialize Tests: {0} iterations ", numIterations);
            BinaryTest.SerializeTest(numIterations, numCpuCores, random1);
            JsonTest.SerializeTest(numIterations, numCpuCores, random2);
            BsonTest.SerializeTest(numIterations, numCpuCores, random3);

            Console.WriteLine();
            Console.WriteLine("Running Memory Tests: {0} iterations ", numIterations);
            BinaryTest.MemoryTest(numIterations, random1);
            JsonTest.MemoryTest(numIterations, random2);
            BsonTest.MemoryTest(numIterations, random3);

            Console.WriteLine();
            Console.WriteLine("Running Reflection Tests: {0} iterations ", numIterations);
            NativeTest.PerformanceTest(random1, numIterations, numCpuCores);
            ReflectionTest.PerformanceTest(random2, numIterations, numCpuCores);
            
            Console.ReadLine();
        }

        public static Grid CreateRandomGrid(Random random)
        {
            const int maxTiles = 16;

            var tiles = new Tile[random.Next(maxTiles)];

            for (var i = 0; i < tiles.Length; ++i)
            {
                tiles[i] = new Tile
                {
                    Row = random.Next(),
                    Column = random.Next(),
                    RowSpan = random.Next(),
                    ColumnSpan = random.Next()
                };
            }

            return new Grid
            {
                Name = "MyGrid" + random.Next(),
                Rows = random.Next(),
                Columns = random.Next(),
                Tiles = tiles
            };
        }
    }
}
