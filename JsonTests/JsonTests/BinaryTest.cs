using System;
using System.Diagnostics;
using System.Linq;
using System.Text;


namespace JsonTests
{
    public class BinaryTest
    {
        public static void SerializeTest(int numIterations, int numCpuCores, Random random)
        {
            long verifier = 0;

            var startTime = Stopwatch.GetTimestamp();

            for (var i = 0; i < numIterations; ++i)
            {
                var grid = Program.CreateRandomGrid(random);
                var serialized = BinarySerializeGrid(grid);
                var deserialized = BinaryDeserializeGrid(serialized);

                verifier += deserialized.Rows;
            }
            var endTime = Stopwatch.GetTimestamp();
            var deltaTime = (endTime - startTime) * 1000.0 / Stopwatch.Frequency;

            Console.WriteLine(
                "Binary. Total {0:F}ms. Average {1:F5}ms. Percentage {2:P}",
                deltaTime, deltaTime / numIterations, deltaTime / 1000.0 / numCpuCores);
        }

        public static void MemoryTest(int numIterations, Random random)
        {
            long size = 0;

            for (var i = 0; i < numIterations; ++i)
            {
                var grid = Program.CreateRandomGrid(random);
                var serialized = BinarySerializeGrid(grid);

                size += serialized.Length;
            }

            Console.WriteLine(
                "Binary. Total size {0}. Average size {1:F1}.",
                size, (float)size / numIterations);
        }

        static byte[] BinarySerializeGrid(Grid grid)
        {
            var nameLength = grid.Name.Length;
            var tilesCount = grid.Tiles.Count();

            const int sizeOfGrid = 16; // Without variable length string
            const int sizeOfTile = 16;

            var nameUnicode = Encoding.UTF8.GetBytes(grid.Name);
            
            var outputBytes = new byte[sizeOfGrid + nameUnicode.Length + (sizeOfTile * tilesCount)];

            var index = 0;

            var nameLengthBytes = BitConverter.GetBytes(nameLength);
            Array.Copy(nameLengthBytes, 0, outputBytes, 0, 4);
            index += 4;

            Array.Copy(nameUnicode, 0, outputBytes, 4, nameUnicode.Length);
            index += nameUnicode.Length;

            var rowsBytes = BitConverter.GetBytes(grid.Rows);
            Array.Copy(rowsBytes, 0, outputBytes, index, 4);
            index += 4;
            
            var columnsBytes = BitConverter.GetBytes(grid.Columns);
            Array.Copy(columnsBytes, 0, outputBytes, index, 4);
            index += 4;

            var tilesCountBytes = BitConverter.GetBytes(tilesCount);
            Array.Copy(tilesCountBytes, 0, outputBytes, index, 4);
            index += 4;

            foreach (var tile in grid.Tiles)
            {
                var rowBytes = BitConverter.GetBytes(tile.Row);
                Array.Copy(rowBytes, 0, outputBytes, index, 4);
                index += 4;

                var columnBytes = BitConverter.GetBytes(tile.Column);
                Array.Copy(columnBytes, 0, outputBytes, index, 4);
                index += 4;

                var rowSpanBytes = BitConverter.GetBytes(tile.RowSpan);
                Array.Copy(rowSpanBytes, 0, outputBytes, index, 4);
                index += 4;

                var columnSpanBytes = BitConverter.GetBytes(tile.ColumnSpan);
                Array.Copy(columnSpanBytes, 0, outputBytes, index, 4);
                index += 4;
            }

            return outputBytes;
        }

        static Grid BinaryDeserializeGrid(byte[] bytes)
        {
            var grid = new Grid();

            var index = 0;

            var nameLength = BitConverter.ToInt32(bytes, index);
            index += 4;

            grid.Name = Encoding.UTF8.GetString(bytes, index, nameLength);
            index += nameLength;

            grid.Rows = BitConverter.ToInt32(bytes, index);
            index += 4;

            grid.Columns = BitConverter.ToInt32(bytes, index);
            index += 4;

            var numTiles = BitConverter.ToInt32(bytes, index);
            index += 4;

            var tiles = new Tile[numTiles];

            for (var i = 0; i < numTiles; ++i)
            {
                var tile = new Tile();

                tile.Row = BitConverter.ToInt32(bytes, index);
                index += 4;

                tile.Column = BitConverter.ToInt32(bytes, index);
                index += 4;

                tile.RowSpan = BitConverter.ToInt32(bytes, index);
                index += 4;

                tile.ColumnSpan = BitConverter.ToInt32(bytes, index);
                index += 4;

                tiles[i] = tile;
            }

            grid.Tiles = tiles;

            return grid;
        }
    }
}
