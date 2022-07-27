using System;
using System.IO;
using Xunit;

namespace MediaVC.Tests.Benchmark
{
    public class ChecksumCalculator
    {
        [Fact]
        public void CheckIsRunningProperly()
        {
            var result = new StringWriter();
            Console.SetOut(result);

            Tools.Benchmark.ChecksumCalculator.Run();
            var output = result.ToString();
            Assert.NotEmpty(output);
            Assert.Matches(@"\w", output);
        }
    }
}
