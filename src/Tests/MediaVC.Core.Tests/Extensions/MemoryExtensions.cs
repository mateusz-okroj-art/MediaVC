﻿using System;
using System.Linq;

using MediaVC.Tests.TestData;

using Xunit;

namespace MediaVC.Core.Tests.Extensions
{
    public class MemoryExtensions
    {
        [Theory]
        [ClassData(typeof(ZeroAndRandomIntegerValuesTestData))]
        public void Split_WhenMemoryIsEmpty_ShouldReturnEmpty(int segmentMaxLength)
        {
            Assert.Empty(Memory<byte>.Empty.Split(segmentMaxLength));
        }

        [Theory]
        [InlineData(0, 255, 0)]
        [InlineData(255, 0, 0)]
        [InlineData(1000, 1000, 1)]
        [InlineData(1000, 500, 2)]
        [InlineData(1000, 250, 4)]
        public void Split_WhenMemoryIsNonEmpty_ShouldReturnSegments_Variant1(int dataLength, int maxSegmentLength, int expectedSegmentCount)
        {
            Memory<byte> data = new byte[dataLength];
            var result = data.Split(maxSegmentLength);

            Assert.Equal(expectedSegmentCount, result?.Count());

            Assert.All(result, segment => Assert.Equal(maxSegmentLength, segment.Length));
        }

        [Theory]
        [InlineData(1000, 99, 10, 11)]
        [InlineData(2000, 249, 8, 9)]
        public void Split_WhenMemoryIsNonEmpty_ShouldReturnSegments_Variant2(int dataLength, int maxSegmentLength, int expectedLastSegmentLength, int expectedSegmentCount)
        {
            Memory<byte> data = new byte[dataLength];
            ReadOnlyMemory<byte> data2 = new byte[dataLength];
            var result = data.Split(maxSegmentLength);

            Assert.Equal(expectedSegmentCount, result?.Count());
            Assert.All(result.SkipLast(1), segment => Assert.Equal(maxSegmentLength, segment.Length));
            Assert.Equal(expectedLastSegmentLength, result.Last().Length);

            var result2 = data2.Split(maxSegmentLength);

            Assert.Equal(expectedSegmentCount, result2?.Count());
            Assert.All(result2.SkipLast(1), segment => Assert.Equal(maxSegmentLength, segment.Length));
            Assert.Equal(expectedLastSegmentLength, result2.Last().Length);
        }

        [Fact]
        public void Split_WhenSegmentMaxLengthIsZero_ShouldReturnEmpty()
        {
            Memory<byte> memory1 = new byte[1];
            ReadOnlyMemory<byte> memory2 = memory1;

            Assert.Empty(memory1.Split(0));
            Assert.Empty(memory2.Split(0));
        }
    }
}
