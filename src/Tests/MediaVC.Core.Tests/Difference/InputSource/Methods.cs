using System;
using MediaVC.Difference.Strategies;

using Moq;

using Xunit;

namespace MediaVC.Core.Tests.Difference.InputSource
{
    public class Methods
    {
        [Fact]
        public void Read_ShouldInvokeInStrategy()
        {
            var mock = new Mock<IInputSourceStrategy>();
            mock.Setup(mocked => mocked.Read(It.IsAny<Memory<byte>>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(0);

            var result = new MediaVC.Difference.InputSource(mock.Object);

            Assert.Equal(0, result.Read(Memory<byte>.Empty, 0, 0));

            mock.Verify(mocked => mocked.Read(It.IsAny<Memory<byte>>(), It.IsAny<int>(), It.IsAny<int>()));
        }
    }
}
