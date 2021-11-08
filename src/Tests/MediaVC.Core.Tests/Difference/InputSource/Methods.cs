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
            var mock = new Mock<IInputSourceStrategy>(MockBehavior.Loose);
            mock.Setup(mocked => mocked.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Verifiable();

            var result = new MediaVC.Difference.InputSource(mock.Object);

            result.Read(Array.Empty<byte>(), 0, 0);

            mock.Verify(mocked => mocked.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()));
        }

        [Fact]
        public async Task ReadAsync_ShouldInvokeInStrategy()
        {
            var mock = new Mock<IInputSourceStrategy>(MockBehavior.Loose);
            mock.Setup(mocked => mocked.ReadAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>())).Verifiable();

            var result = new MediaVC.Difference.InputSource(mock.Object);

            await result.ReadAsync(Memory<byte>.Empty, default);

            mock.Verify(mocked => mocked.ReadAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task ReadByteAsync_ShouldInvokeInStrategy()
        {
            var mock = new Mock<IInputSourceStrategy>(MockBehavior.Loose);
            mock.Setup(mocked => mocked.ReadByteAsync(It.IsAny<CancellationToken>())).Verifiable();

            var result = new MediaVC.Difference.InputSource(mock.Object);

            await result.ReadByteAsync(CancellationToken.None);

            mock.Verify(mocked => mocked.ReadByteAsync(It.IsAny<CancellationToken>()));
        }
    }
}
