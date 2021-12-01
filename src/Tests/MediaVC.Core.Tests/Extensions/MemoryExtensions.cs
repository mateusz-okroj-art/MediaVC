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

        [Theory]
        [InlineData(0, 255, 0)]
        [InlineData(255, 0, 0)]
        [InlineData(1000, 1000, 1)]
        [InlineData(1000, 500, 2)]
        [InlineData(1000, 250, 4)]
        public void ToSplit_WhenMemoryIsNonEmpty_ShouldReturnSegments_Variant1(int dataLength, int maxSegmentLength, int expectedSegmentCount)
        {
            ReadOnlyMemory<byte> data = new byte[dataLength];
            var result = data.Split(maxSegmentLength);

            Assert.Equal(expectedSegmentCount, result?.Count());

            Assert.All(result, segment => Assert.Equal(maxSegmentLength, segment.Length));
        }

        [Theory]
        [InlineData(1000, 99, 10, 11)]
        [InlineData(2000, 249, 8, 9)]
        public void ToSplit_WhenMemoryIsNonEmpty_ShouldReturnSegments_Variant2(int dataLength, int maxSegmentLength, int expectedLastSegmentLength, int expectedSegmentCount)
        {
            ReadOnlyMemory<byte> data = new byte[dataLength];
            var result = data.Split(maxSegmentLength);

            Assert.Equal(expectedSegmentCount, result?.Count());

            Assert.All(result.SkipLast(1), segment => Assert.Equal(maxSegmentLength, segment.Length));

            Assert.Equal(expectedLastSegmentLength, result.Last().Length);
        }
    }
}
