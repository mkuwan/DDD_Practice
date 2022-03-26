using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace SampleConsole.Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class DateParserBenchmarks
    {
        private const string dateTime = "2022-3-21T16:31:30Z";
        private static readonly DateParser parser = new DateParser();

        [Benchmark(Baseline = true)]
        public void GetYearFromDateTime()
        {
            parser.GetYearFromDateTime(dateTime);
        }
    }
}
