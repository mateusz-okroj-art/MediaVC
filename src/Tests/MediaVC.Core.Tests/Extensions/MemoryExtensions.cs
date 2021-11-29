using System;
using System.Linq;

using MediaVC.Core.Tests.TestData;

using Xunit;

namespace MediaVC.Core.Tests.Extensions
{
    public class MemoryExtensions
    {
        [Theory]
        [ClassData(typeof(ZeroAndRandomIntegerValuesTestData))]
        public void ToSplit_WhenMemoryIsEmpty_ShouldReturnEmpty(int segmentMaxLength)
        {
            Assert.Empty(ReadOnlyMemory<byte>.Empty.Split(segmentMaxLength));
        }

        [Fact]
        public void ToSplit_WhenMemoryIsNonEmpty_ShouldReturnSegments()
        {
            ReadOnlyMemory<byte> data = new byte[1000];
            var result = data.Split(500);

            Assert.Equal(2, result?.Count());
            Assert.Equal(500, result.ElementAt(0).Length);
            Assert.Equal(500, result.ElementAt(1).Length);
        }
    }
}
