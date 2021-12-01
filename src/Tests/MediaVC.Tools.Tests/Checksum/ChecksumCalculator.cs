using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediaVC.Tools.Tests.TestData;

using Xunit;

namespace MediaVC.Tools.Tests.Checksum
{
    public class ChecksumCalculator
    {
        [Fact]
        public void CalculateInternalAsync_WhenArgumentIsEmpty_ShouldReturnEmpty()
        {
            var result = Tools.ChecksumCalculator.CalculateInternalAsync(AsyncEnumerable.Empty<ReadOnlyMemory<byte>>()).ToEnumerable();

            Assert.Empty(result);
        }

        [Theory]
        [ClassData(typeof(ChecksumRandomTestData))]
        public void CalculateInternalAsync_WhenArgumentIsNonEmpty_ShouldReturnCalculated((byte[], byte[]) data)
        {
            var result = Tools.ChecksumCalculator
        }
    }
}
