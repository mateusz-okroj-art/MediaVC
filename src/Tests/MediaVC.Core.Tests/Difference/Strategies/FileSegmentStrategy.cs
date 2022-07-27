using System;
using System.Collections.Generic;
using System.Linq;

using MediaVC.Tests.TestData;
using MediaVC.Difference;

using Moq;

using Xunit;
using System.Threading.Tasks;

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

        [Theory]
        [ClassData(typeof(RandomLengthAndCountEmptySegmentsTestData))]
        public void Length_WhenArgumentHaveItems_ShouldReturnSummedLength(IEnumerable<IFileSegmentInfo> segments)
        {
            var strategy = new MediaVC.Difference.Strategies.FileSegmentStrategy(segments);

            Assert.Equal(segments.Select(item => (decimal)item.Length).Sum(), strategy.Length);
        }

        [Theory]
        [ClassData(typeof(RandomLengthAndCountEmptySegmentsTestData))]
        public void Position_WhenSettedValueIsTooLarge_ShouldThrowException(IEnumerable<IFileSegmentInfo> segments)
        {
            var strategy = new MediaVC.Difference.Strategies.FileSegmentStrategy(segments);

            var randomPosition = (long)(new Random().NextDouble() * int.MaxValue / 2) + strategy.Length;

            Assert.Throws<ArgumentOutOfRangeException>(() => strategy.Position = randomPosition);
        }

        [Theory]
        [ClassData(typeof(RandomLengthAndCountEmptySegmentsTestData))]
        public void Position_WhenSettedValueIsValid_ShouldSetProperty(IEnumerable<IFileSegmentInfo> segments)
        {
            var strategy = new MediaVC.Difference.Strategies.FileSegmentStrategy(segments);

            var randomPosition = new Random().Next(0, (int)Math.Min(strategy.Length - 1, int.MaxValue));
            strategy.Position = randomPosition;

            Assert.Equal(randomPosition, strategy.Position);
        }

        [Fact]
        public void SelectMappedSegmentForCurrentPosition_WhenSetPosition_ShouldReturnValidSegment()
        {
            var randomLength = new Random().Next(2, int.MaxValue);
            var segments = new IFileSegmentInfo[]
            {
                Mock.Of<IFileSegmentInfo>(mock => mock.MappedPosition == 0 && mock.Length == (ulong)(randomLength/2) && mock.Source == MediaVC.Difference.InputSource.Empty),
                Mock.Of<IFileSegmentInfo>(mock => mock.MappedPosition == randomLength/2 - 1 && mock.Length == (ulong)(randomLength/2) && mock.Source == MediaVC.Difference.InputSource.Empty)
            };
            var strategy = new MediaVC.Difference.Strategies.FileSegmentStrategy(segments);

            strategy.Position = segments[0].MappedPosition;

            Assert.True(ReferenceEquals(segments[0], strategy.SelectMappedSegmentForCurrentPosition()));

            strategy.Position = segments[1].MappedPosition;

            Assert.True(ReferenceEquals(segments[1], strategy.SelectMappedSegmentForCurrentPosition()));
        }

        [Fact]
        public void CheckIsNotUsedSource_WhenSelectedUsedSource_ShouldReturnFalse()
        {
            var source = Mock.Of<IInputSource>();

            var segments = new IFileSegmentInfo[]
            {
                Mock.Of<IFileSegmentInfo>(mock => mock.Source == source),
                Mock.Of<IFileSegmentInfo>(mock => mock.Source == MediaVC.Difference.InputSource.Empty)
            };
            var strategy = new MediaVC.Difference.Strategies.FileSegmentStrategy(segments);

            Assert.False(strategy.CheckIsNotUsedSource(source));
        }

        [Theory]
        [ClassData(typeof(RandomBytesTestData))]
        public async Task ReadAsync_Variant1_ShouldReadFromSegments(byte[][] data)
        {
            var sources = data.Select(bytes => new MediaVC.Difference.InputSource(bytes)).ToArray();

            var segments = new IFileSegmentInfo[sources.Length];
            long currentPosition = 0;
            for (var i = 0; i < sources.Length; ++i)
            {
                segments[i] = Mock.Of<IFileSegmentInfo>(mock =>
                    mock.StartPositionInSource == 0 &&
                    mock.EndPositionInSource == sources[i].Length - 1 &&
                    mock.Length == (ulong)sources[i].Length &&
                    mock.MappedPosition == currentPosition &&
                    mock.Source == sources[i]
                );

                currentPosition += sources[i].Length;
            }

            var expectedResult = data.SelectMany(bytes => bytes).ToArray();

            var strategy = new MediaVC.Difference.Strategies.FileSegmentStrategy(segments);
            var resultBuffer = new Memory<byte>(new byte[expectedResult.Length]);

            Assert.Equal(expectedResult.Length, await strategy.ReadAsync(resultBuffer));

            Assert.Equal(expectedResult, resultBuffer.ToArray());
        }

        [Theory]
        [ClassData(typeof(RandomBytesTestData))]
        public async Task ReadByteAsync_Variant1_ShouldReadFromSegments(byte[][] data)
        {
            var sources = data.Select(bytes => new MediaVC.Difference.InputSource(bytes)).ToArray();

            var segments = new IFileSegmentInfo[sources.Length];
            long currentPosition = 0;
            for (var i = 0; i < sources.Length; ++i)
            {
                segments[i] = Mock.Of<IFileSegmentInfo>(mock =>
                    mock.StartPositionInSource == 0 &&
                    mock.EndPositionInSource == sources[i].Length - 1 &&
                    mock.Length == (ulong)sources[i].Length &&
                    mock.MappedPosition == currentPosition &&
                    mock.Source == sources[i]
                );

                currentPosition += sources[i].Length;
            }

            var strategy = new MediaVC.Difference.Strategies.FileSegmentStrategy(segments);
            
            for(var i = 0; i < segments.Length; ++i)
            {
                strategy.Position = segments[i].MappedPosition;
                Assert.Equal(data[i][0], await strategy.ReadByteAsync());
            }
        }

        [Fact]
        public async Task ReadByteAsync_Variant2_ShouldReadFromSegmentsWithBuffering()
        {
            const int testLength = MediaVC.Difference.Strategies.FileSegmentStrategy.BufferLength + 5;
            var data1 = new byte[testLength];

            for(var i = 0; i < testLength; ++i)
                data1[i] = (byte)(i % byte.MaxValue);

            using var source1 = new MediaVC.Difference.InputSource(data1.AsMemory());

            var segments = new IFileSegmentInfo[]
            {
                Mock.Of<IFileSegmentInfo>(mock =>
                mock.StartPositionInSource == 0 &&
                mock.EndPositionInSource == source1.Length-1 &&
                mock.Length == (ulong)data1.Length &&
                mock.MappedPosition == 0 &&
                mock.Source == source1)
            };

            var strategy = new MediaVC.Difference.Strategies.FileSegmentStrategy(segments);

            strategy.Position = 0;
            Assert.Equal(data1[0], await strategy.ReadByteAsync());

            strategy.Position = MediaVC.Difference.Strategies.FileSegmentStrategy.BufferLength;
            Assert.Equal(data1[MediaVC.Difference.Strategies.FileSegmentStrategy.BufferLength], await strategy.ReadByteAsync());
        }
    }
}
