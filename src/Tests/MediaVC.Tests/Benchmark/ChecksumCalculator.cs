using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
