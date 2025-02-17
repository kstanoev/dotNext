using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Order;
using System;


namespace DotNext.Benchmarks
{
    [SimpleJob(runStrategy: RunStrategy.Throughput, launchCount: 1)]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    public class ArrayEqualityBenchmark
    {
        private static readonly Guid[] ShortGuidArray1 = new Guid[10];
        private static readonly Guid[] ShortGuidArray2 = ShortGuidArray1.Clone() as Guid[];

        private static readonly Guid[] LongGuidArray1 = new Guid[100];
        private static readonly Guid[] LongGuidArray2 = LongGuidArray1.Clone() as Guid[];

        static ArrayEqualityBenchmark()
        {
            for (var i = default(long); i < ShortGuidArray1.LongLength; i++)
                ShortGuidArray1[i] = ShortGuidArray2[i] = Guid.NewGuid();
            for (var i = default(long); i < LongGuidArray1.LongLength; i++)
                LongGuidArray1[i] = LongGuidArray2[i] = Guid.NewGuid();
        }

        [Benchmark]
        public static void ShortGuidArrayBitwiseEquals()
        {
            ShortGuidArray1.BitwiseEquals(ShortGuidArray2);
        }

        [Benchmark]
        public static void ShortGuidArraySequenceEqual()
        {
            ShortGuidArray1.SequenceEqual(ShortGuidArray2);
        }

        [Benchmark]
        public static void ShortGuidArrayForEachEqual()
        {
            for (var i = default(long); i < ShortGuidArray1.LongLength; i++)
                if (ShortGuidArray1[i] != ShortGuidArray2[i])
                    return;
        }

        [Benchmark]
        public static void LongGuidArrayBitwiseEquals()
        {
            LongGuidArray1.BitwiseEquals(LongGuidArray2);
        }

        [Benchmark]
        public static void LongGuidArraySequenceEqual()
        {
            LongGuidArray1.SequenceEqual(LongGuidArray2);
        }

        [Benchmark]
        public static void LongGuidArrayForEachEqual()
        {
            for (var i = default(long); i < LongGuidArray1.LongLength; i++)
                if (LongGuidArray1[i] != LongGuidArray2[i])
                    return;
        }
    }
}