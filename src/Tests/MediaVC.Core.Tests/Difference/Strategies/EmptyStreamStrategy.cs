using System;
using MediaVC.Difference.Strategies;

using Moq;

using Xunit;

namespace MediaVC.Core.Tests.Difference.Strategies
{
    public class EmptyStreamStrategy
    {
        private readonly MediaVC.Difference.Strategies.EmptyStreamStrategy fixture = new();

        [Fact]
        public void Length_ShouldReturnValidValue()
        {
            Assert.Equal(0, this.fixture.Length);
        }

        [Fact]
        public void Position_get_ShouldReturnValidValue()
        {
            Assert.Equal(-1, this.fixture.Position);
        }

        [Fact]
        public void Position_set_ShouldThrowException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => this.fixture.Position = 0);
        }

        [Fact]
        public void Equals_WhenArgumentLengthIsEquals_ShouldReturnTrue()
        {
            var emptyStrategyMock = new Mock<IInputSourceStrategy>(MockBehavior.Loose);
            emptyStrategyMock.SetupGet(mocked => mocked.Length).Returns(0);

            Assert.True(this.fixture.Equals(emptyStrategyMock.Object));

            emptyStrategyMock.VerifyGet(mocked => mocked.Length);
        }

        [Fact]
        public void Equals_WhenArgumentLengthIsNotEqual_ShouldReturnFalse()
        {
            var emptyStrategyMock = new Mock<IInputSourceStrategy>(MockBehavior.Loose);
            emptyStrategyMock.SetupGet(mocked => mocked.Length).Returns(100);

            Assert.False(this.fixture.Equals(emptyStrategyMock.Object));

            emptyStrategyMock.VerifyGet(mocked => mocked.Length);
        }

        [Fact]
        public void Read_ShouldThrowException()
        {
            Assert.Throws<InvalidOperationException>(() => this.fixture.Read(new byte[0], 0, 1));
        }

        [Fact]
        public void ReadByteAsync_ShouldThrowException()
        {
            Assert.ThrowsAsync<InvalidOperationException>(() => this.fixture.ReadByteAsync().AsTask());
        }

        [Fact]
        public void ReadAsync_ShouldThrowException()
        {
            Assert.ThrowsAsync<InvalidOperationException>(() => this.fixture.ReadAsync(Array.Empty<byte>()).AsTask());
        }
    }
}
