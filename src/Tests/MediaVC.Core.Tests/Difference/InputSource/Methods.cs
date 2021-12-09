using System;
using System.Collections.Generic;
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

        [Fact]
        public async void Enumerator_ShouldEnumerateBytesFromStream()
        {
            var strategyMock = new Mock<IInputSourceStrategy>();
            strategyMock.SetupProperty(mock => mock.Position);
            strategyMock.SetupGet(mock => mock.Length).Returns(10);
            strategyMock.Setup(mock => mock.ReadByteAsync(It.IsAny<CancellationToken>()))
                .Returns(ValueTask.FromResult<byte>(0));

            using var inputSource = new MediaVC.Difference.InputSource(strategyMock.Object);

            var results = new List<byte>(10);
            await foreach(var item in inputSource)
                results.Add(item);

            Assert.Equal(strategyMock.Object.Length, results.Count);
            Assert.All(results, item => Assert.Equal(0, item));

            strategyMock.Verify(mock => mock.ReadByteAsync(It.IsAny<CancellationToken>()), Times.Exactly(10));
            strategyMock.VerifySet(mock => mock.Position = It.IsInRange<long>(-1, 9, Moq.Range.Inclusive));
        }
    }
}
