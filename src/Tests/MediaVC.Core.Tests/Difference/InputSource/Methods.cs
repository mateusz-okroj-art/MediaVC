using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediaVC.Difference.Strategies;
using MediaVC.Tests.Extensions.TestData;

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

            _ = result.Read(Array.Empty<byte>(), 0, 0);

            mock.Verify(mocked => mocked.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()));
        }

        [Fact]
        public async Task ReadAsync_ShouldInvokeInStrategy()
        {
            var mock = new Mock<IInputSourceStrategy>(MockBehavior.Loose);
            mock.Setup(mocked => mocked.ReadAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>())).Verifiable();

            var result = new MediaVC.Difference.InputSource(mock.Object);

            _ = await result.ReadAsync(Memory<byte>.Empty, default);

            mock.Verify(mocked => mocked.ReadAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task ReadByteAsync_ShouldInvokeInStrategy()
        {
            var mock = new Mock<IInputSourceStrategy>(MockBehavior.Loose);
            mock.Setup(mocked => mocked.ReadByteAsync(It.IsAny<CancellationToken>())).Verifiable();

            var result = new MediaVC.Difference.InputSource(mock.Object);

            _ = await result.ReadByteAsync(CancellationToken.None);

            mock.Verify(mocked => mocked.ReadByteAsync(It.IsAny<CancellationToken>()));
        }

        [Theory]
        [ClassData(typeof(RandomValueTestData<long>))]
        public async Task Enumerator_ShouldEnumerateBytesFromStream(long testedLength)
        {
            const byte testValue = 2;

            var strategyMock = new Mock<IInputSourceStrategy>();
            _ = strategyMock.SetupProperty(mock => mock.Position);
            _ = strategyMock.SetupGet(mock => mock.Length).Returns(testedLength);
            _ = strategyMock.Setup(mock => mock.ReadByteAsync(It.IsAny<CancellationToken>()))
                .Returns(ValueTask.FromResult(testValue));

            using var inputSource = new MediaVC.Difference.InputSource(strategyMock.Object);

            var results = new List<byte>((int)testedLength);
            await foreach(var item in inputSource)
                results.Add(item);

            Assert.Equal(strategyMock.Object.Length, results.Count);
            Assert.All(results, item => Assert.Equal(testValue, item));

            strategyMock.Verify(mock => mock.ReadByteAsync(It.IsAny<CancellationToken>()), Times.Exactly((int)testedLength));
            strategyMock.VerifySet(mock => mock.Position = It.IsInRange(-1, testedLength-1, Moq.Range.Inclusive));
        }
    }
}
