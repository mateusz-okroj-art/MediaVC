using System;
using System.Threading;
using System.Threading.Tasks;

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
            mock.Setup(mocked => mocked.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(0);

            var result = new MediaVC.Difference.InputSource(mock.Object);

            Assert.Equal(0, result.Read(Array.Empty<byte>(), 0, 0));

            mock.Verify(mocked => mocked.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()));
        }

        [Fact]
        public async Task ReadAsync_ShouldInvokeInStrategy()
        {
            var mock = new Mock<IInputSourceStrategy>();
            mock.Setup(mocked => mocked.ReadAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>()))
                .Returns(ValueTask.FromResult(0));

            var result = new MediaVC.Difference.InputSource(mock.Object);

            Assert.Equal(0, await result.ReadAsync(Memory<byte>.Empty, default));

            mock.Verify(mocked => mocked.ReadAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>()));
        }
    }
}
