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

        [Fact]
        public void Length_ShouldGetFromStrategy()
        {
            var mock = new Mock<IInputSourceStrategy>(MockBehavior.Loose);
            mock.SetupGet(mocked => mocked.Length);

            var result = new MediaVC.Difference.InputSource(mock.Object);

            _ = result.Length;

            mock.VerifyGet(mocked => mocked.Length);
        }

        [Fact]
        public void Position_ShouldGetFromStrategy()
        {
            var mock = new Mock<IInputSourceStrategy>(MockBehavior.Loose);
            mock.SetupGet(mocked => mocked.Position);

            var result = new MediaVC.Difference.InputSource(mock.Object);

            _ = result.Position;

            mock.VerifyGet(mocked => mocked.Position);
        }

        [Fact]
        public void Position_ShouldSetInStrategy()
        {
            var mock = new Mock<IInputSourceStrategy>(MockBehavior.Loose);
            mock.SetupProperty(mocked => mocked.Position);

            var result = new MediaVC.Difference.InputSource(mock.Object);
            result.Position = 1;

            mock.VerifySet(mocked => mocked.Position = It.IsAny<long>());
        }
    }
}
