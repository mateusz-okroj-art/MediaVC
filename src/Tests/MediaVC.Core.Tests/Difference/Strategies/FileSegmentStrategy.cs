﻿using System;
using System.Linq;

using MediaVC.Difference;

using Moq;

using Xunit;

namespace MediaVC.Core.Tests.Difference.Strategies
{
    public class FileSegmentStrategy
    {
        [Fact]
        public void Constructor_WhenArgumentIsNull_ShouldThrowException()
        {
            Assert.Throws<ArgumentNullException>(() => new MediaVC.Difference.Strategies.FileSegmentStrategy(null));
        }

        [Fact]
        public void Length_WhenArgumentIsEmpty_ShouldReturn0()
        {
            var argument = Enumerable.Empty<IFileSegmentInfo>();

            var result = new MediaVC.Difference.Strategies.FileSegmentStrategy(argument);

            Assert.Equal(0, result.Length);
        }

        [Fact]
        public void Length_WhenArgumentHaveItems_ShouldReturnSummedLength()
        {
            var argument = new IFileSegmentInfo[]
            {
                Mock.Of<IFileSegmentInfo>(mock => mock.Length == 1),
                Mock.Of<IFileSegmentInfo>(mock => mock.Length == 10),
                Mock.Of<IFileSegmentInfo>(mock => mock.Length == 100)
            };

            var strategy = new MediaVC.Difference.Strategies.FileSegmentStrategy(argument);

            Assert.Equal(argument.Select(item => (decimal)item.Length).Sum(), strategy.Length);
        }

        [Fact]
        public void Position_WhenSettedValueIsTooLarge_ShouldThrowException()
        {
            var argument = new IFileSegmentInfo[]
            {
                Mock.Of<IFileSegmentInfo>(mock => mock.Length == 1)
            };

            var strategy = new MediaVC.Difference.Strategies.FileSegmentStrategy(argument);

            Assert.Throws<ArgumentOutOfRangeException>(() => strategy.Position = 1);
        }

        [Fact]
        public void Position_WhenSettedValueIsValid_ShouldSetProperty()
        {
            var argument = new IFileSegmentInfo[]
            {
                Mock.Of<IFileSegmentInfo>(mock => mock.Length == 2)
            };
            var strategy = new MediaVC.Difference.Strategies.FileSegmentStrategy(argument);

            strategy.Position = 1;

            Assert.Equal(1, strategy.Position);
        }

        [Fact]
        public void SelectMappedSegmentForCurrentPosition_WhenSetPosition_ShouldReturnValidSegment()
        {
            var segments = new IFileSegmentInfo[]
            {
                Mock.Of<IFileSegmentInfo>(mock => mock.MappedPosition == 0 && mock.Length == 2),
                Mock.Of<IFileSegmentInfo>(mock => mock.MappedPosition == 2 && mock.Length == 2)
            };
            var strategy = new MediaVC.Difference.Strategies.FileSegmentStrategy(segments);

            strategy.Position = 1;

            Assert.True(ReferenceEquals(segments[0], strategy.SelectMappedSegmentForCurrentPosition()));

            strategy.Position = 2;

            Assert.True(ReferenceEquals(segments[1], strategy.SelectMappedSegmentForCurrentPosition()));
        }

        [Fact]
        public void CheckIsNotUsedSource_WhenSelectedUsedSource_ShouldReturnFalse()
        {
            var source = Mock.Of<IInputSource>();

            var segments = new IFileSegmentInfo[]
            {
                Mock.Of<IFileSegmentInfo>(mock => mock.MappedPosition == 0 && mock.Length == 2 && mock.Source == source),
                Mock.Of<IFileSegmentInfo>(mock => mock.MappedPosition == 2 && mock.Length == 2 && mock.Source == null)
            };
            var strategy = new MediaVC.Difference.Strategies.FileSegmentStrategy(segments);

            Assert.False(strategy.CheckIsNotUsedSource(source));
        }

        [Fact]
        public async void ReadAsync_Variant1_ShouldReadFromSegment()
        {
            var data1 = new byte[] { 0, 255, 200, 100 };
            var data2 = new byte[] { 200, 255, 0, 100 };

            using var source1 = new MediaVC.Difference.InputSource(data1.AsMemory());
            using var source2 = new MediaVC.Difference.InputSource(data2.AsMemory());

            var segments = new IFileSegmentInfo[]
            {
                Mock.Of<IFileSegmentInfo>(mock =>
                mock.StartPositionInSource == 0 &&
                mock.EndPositionInSource == 1 &&
                mock.Length == 2 &&
                mock.MappedPosition == 0 &&
                mock.Source == source1),
                Mock.Of<IFileSegmentInfo>(mock =>
                mock.StartPositionInSource == 2 &&
                mock.EndPositionInSource == 3 &&
                mock.Length == 2 &&
                mock.MappedPosition == 2 &&
                mock.Source == source2)
            };

            var expectedResult = new byte[] { data1[0], data1[1], data2[2], data2[3] };

            var strategy = new MediaVC.Difference.Strategies.FileSegmentStrategy(segments);
            var resultBuffer = new Memory<byte>(new byte[expectedResult.Length]);

            Assert.Equal(expectedResult.Length, await strategy.ReadAsync(resultBuffer));

            Assert.Equal(expectedResult, resultBuffer.ToArray());
        }
    }
}