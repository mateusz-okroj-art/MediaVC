﻿using MediaVC.Difference;

using Moq;

using Xunit;

namespace MediaVC.Core.Tests.Difference.FileSegments
{
    public class FileSegmentInfo
    {
        [Fact]
        public void StartPosition_ShouldSetProperty()
        {
            MediaVC.Difference.FileSegmentInfo result = default;

            const long testValue = long.MaxValue;

            result.StartPosition = testValue;

            Assert.Equal(testValue, result.StartPosition);
        }

        [Fact]
        public void EndPosition_ShouldSetProperty()
        {
            MediaVC.Difference.FileSegmentInfo result = default;

            const long testValue = 1_000_000;

            result.EndPosition = testValue;

            Assert.Equal(testValue, result.EndPosition);
        }

        [Fact]
        public void Source_ShouldSetProperty()
        {
            MediaVC.Difference.FileSegmentInfo result = default;

            var testValue = new Mock<IInputSource>().Object;

            result.Source = testValue;

            Assert.Equal(testValue, result.Source);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, 1)]
        [InlineData(1, 100)]
        [InlineData(-100, 0)]
        [InlineData(100, -1000)]
        public void Length_TestCases_ShouldChangeAfterSetPosition(long start, long end)
        {
            MediaVC.Difference.FileSegmentInfo result = default;

            result.StartPosition = start;
            result.EndPosition = end;
            
            Assert.Equal(
                result.StartPosition >= 0 && result.StartPosition <= result.EndPosition && result.EndPosition >= result.StartPosition ? (ulong)(result.EndPosition - result.StartPosition + 1) : 0,
                result.Length);
        }
    }
}