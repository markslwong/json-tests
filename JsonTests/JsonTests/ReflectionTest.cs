using System;
using System.Diagnostics;
using System.Linq;


namespace JsonTests
{
    public static class ReflectionTest
    {
        public static void PerformanceTest(Random random, int numIterations, int numCpuCores)
        {
            var target = new InvokeTarget();
            var methodInfo = target.GetType().GetMethods().First();

            var startTime = Stopwatch.GetTimestamp();

            for (var i = 0; i < numIterations; ++i)
            {
                methodInfo.Invoke(target, new object[] {"Param" + random.Next(10000)});
            }

            var endTime = Stopwatch.GetTimestamp();
            var deltaTime = (endTime - startTime) * 1000.0 / Stopwatch.Frequency;

            Console.WriteLine(
                "Reflection. Total {0:F}ms. Average {1:F5}ms. Percentage {2:P}",
                deltaTime, deltaTime / numIterations, deltaTime / 1000.0 / numCpuCores);
        }
    }
}
