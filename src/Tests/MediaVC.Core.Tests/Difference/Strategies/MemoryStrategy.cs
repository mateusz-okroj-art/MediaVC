using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

using MediaVC.Difference;
using MediaVC.Difference.Strategies;
using MediaVC.Tests.TestData;

using Moq;

using Xunit;

namespace MediaVC.Core.Tests.Difference.Strategies
{
    public class MemoryStrategy
    {
        [Fact]
        public void Constructor_WhenArgumentIsNull_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new MediaVC.Difference.Strategies.MemoryStrategy(null));
        }

        [Fact]
        public void Constructor_WhenArgumentIsEmpty_ShouldThrow()
        {
            Memory<byte> argument = Array.Empty<byte>();

            Assert.Throws<ArgumentException>(() => new MediaVC.Difference.Strategies.MemoryStrategy(argument));
        }

        [Theory]
        [ClassData(typeof(RandomNonZeroIntegerTestData))]
        public void Length_WhenArgumentIsNotEmpty_ShouldReturnBufferLength(int randomLength)
        {
            Memory<byte> buffer = new byte[randomLength];
            var strategy = new MediaVC.Difference.Strategies.MemoryStrategy(buffer);

            Assert.Equal(randomLength, strategy.Length);
        }

        [Theory]
        [ClassData(typeof(RandomNonZeroIntegerTestData))]
        public void Position_WhenSettedValueIsTooLarge_ShouldThrowException(int randomLength)
        {
            Memory<byte> buffer = new byte[randomLength];
            var strategy = new MediaVC.Difference.Strategies.MemoryStrategy(buffer);

            var randomPosition = (long)randomLength * 2;

            Assert.Throws<ArgumentOutOfRangeException>(() => strategy.Position = randomPosition);
        }

        [Theory]
        [ClassData(typeof(RandomNonZeroIntegerTestData))]
        public void Position_WhenSettedValueIsValid_ShouldSetProperty(int randomLength)
        {
            Memory<byte> buffer = new byte[randomLength];
            var strategy = new MediaVC.Difference.Strategies.MemoryStrategy(buffer);

            var randomPosition = (long)Math.Round(new Random().NextDouble() * randomLength);
            strategy.Position = randomPosition;

            Assert.Equal(randomPosition, strategy.Position);
        }

        [Theory]
        [ClassData(typeof(RandomBytesTestData))]
        public async Task ReadAsync_WhenOutputBufferIsNonEmpty_ShouldReadFromSegments(byte[][] data)
        {
            Memory<byte> buffer = data[0];
            var strategy = new MediaVC.Difference.Strategies.MemoryStrategy(buffer);

            Memory<byte> resultBuffer = new byte[buffer.Length];
            var count = await strategy.ReadAsync(resultBuffer);

            Assert.Equal(resultBuffer.Length, count);

            Assert.Equal(data[0], resultBuffer.ToArray());
        }

        [Fact]
        public async Task ReadAsync_WhenOutputBufferIsEmpty_ShouldReturnZero()
        {
            Memory<byte> buffer = new byte[1];
            Memory<byte> resultBuffer = Memory<byte>.Empty;

            var strategy = new MediaVC.Difference.Strategies.MemoryStrategy(buffer);
            Assert.Equal(0, await strategy.ReadAsync(resultBuffer));
        }

        [Theory]
        [ClassData(typeof(RandomBytesTestData))]
        public async Task ReadByteAsync_WhenPositionIsBeforeEnd_ShouldReadFromSegments(byte[][] data)
        {
            Memory<byte> buffer = data[0];
            var strategy = new MediaVC.Difference.Strategies.MemoryStrategy(buffer);

            var result = await strategy.ReadByteAsync();

            Assert.Equal(data[0][0], result);

            Assert.Equal(1, strategy.Position);
        }

        [Fact]
        public async Task ReadByteAsync_WhenPositionIsOnEnd_ShouldThrow()
        {
            Memory<byte> buffer = new byte[1];
            var strategy = new MediaVC.Difference.Strategies.MemoryStrategy(buffer);

            _ = await strategy.ReadByteAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await strategy.ReadByteAsync());
        }

        [Fact]
        public void Equals_WhenIsEquals_ShouldReturnTrue()
        {
            Memory<byte> buffer = new byte[10];

            var strategy = new MediaVC.Difference.Strategies.MemoryStrategy(buffer);

            var mock = new Mock<IInputSourceStrategy>();
            mock.SetupGet(m => m.Length).Returns(strategy.Length);
            mock.Setup(m => m.GetHashCode()).Returns(strategy.GetHashCode());

            Assert.True(strategy.Equals(mock.Object));
        }

        [Fact]
        public void Equals_WhenIsNotEquals_ShouldReturnFalse()
        {
            Memory<byte> buffer = new byte[10];

            var strategy = new MediaVC.Difference.Strategies.MemoryStrategy(buffer);

            var mock = new Mock<IInputSourceStrategy>();
            mock.SetupGet(m => m.Length).Returns(strategy.Length + 10);
            mock.Setup(m => m.GetHashCode()).Returns(RandomNumberGenerator.GetInt32(1, 20));

            Assert.False(strategy.Equals(mock.Object));
        }
    }
}
