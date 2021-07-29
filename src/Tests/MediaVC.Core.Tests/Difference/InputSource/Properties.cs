using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediaVC.Difference.Strategies;

using Moq;

using Xunit;

namespace MediaVC.Core.Tests.Difference.InputSource
{
    public class Properties
    {
        [Fact]
        public void CanRead_ShouldReturnTrue()
        {
            var result = new MediaVC.Difference.InputSource(Mock.Of<IInputSourceStrategy>(MockBehavior.Loose));

            Assert.True(result.CanRead);
        }

        [Fact]
        public void CanWrite_ShouldReturnFalse()
        {
            var result = new MediaVC.Difference.InputSource(Mock.Of<IInputSourceStrategy>(MockBehavior.Loose));

            Assert.False(result.CanWrite);
        }

        [Fact]
        public void CanSeek_ShouldReturnTrue()
        {
            var result = new MediaVC.Difference.InputSource(Mock.Of<IInputSourceStrategy>(MockBehavior.Loose));

            Assert.True(result.CanSeek);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-10)]
        [InlineData(2000)]
        [InlineData(long.MaxValue)]
        public void Length_ShouldGetFromStrategy(long testValue)
        {
            var result = new MediaVC.Difference.InputSource
            (
                Mock.Of<IInputSourceStrategy>(mock => mock.Length == testValue)
            );

            Assert.Equal(result.Strategy.Length, result.Length);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-10)]
        [InlineData(2000)]
        [InlineData(long.MaxValue)]
        public void Position_ShouldGetFromStrategy(long testValue)
        {
            var result = new MediaVC.Difference.InputSource
            (
                Mock.Of<IInputSourceStrategy>(mock => mock.Position == testValue)
            );

            Assert.Equal(result.Strategy.Position, result.Position);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-10)]
        [InlineData(2000)]
        [InlineData(long.MaxValue)]
        public void Position_ShouldSetInStrategy(long testValue)
        {
            var mock = new Mock<IInputSourceStrategy>();
            mock.SetupProperty(mocked => mocked.Position);

            var result = new MediaVC.Difference.InputSource(mock.Object);

            result.Position = testValue;

            mock.VerifySet(mocked => mocked.Position = testValue);
        }
    }
}
