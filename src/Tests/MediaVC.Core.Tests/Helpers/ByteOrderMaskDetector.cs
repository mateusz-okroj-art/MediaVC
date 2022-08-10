using System;
using System.Threading.Tasks;

using MediaVC.Difference;

using Moq;

using Xunit;

namespace MediaVC.Core.Tests.Helpers
{
    public class ByteOrderMaskDetector
    {
        [Fact]
        public void Constructor_WhenArgumentIsNull_ShouldThrow()
        {
            IInputSource argument = null;

            Assert.Throws<ArgumentNullException>(() => new MediaVC.Helpers.ByteOrderMaskDetector(argument));
        }

        [Fact]
        public async Task ScanForUTF16BOM_WhenAvailableDataIsTooSmall_ShouldSetError()
        {
            var inputSourceMock = new Mock<IInputSource>(MockBehavior.Strict);

            const long startPosition = -1;
            inputSourceMock.SetupProperty(mocked => mocked.Position);
            inputSourceMock.Object.Position = startPosition;

            inputSourceMock.SetupGet(mocked => mocked.Length)
                .Returns(1);

            var result = new MediaVC.Helpers.ByteOrderMaskDetector(inputSourceMock.Object);

            Assert.Null(await result.ScanForUTF16BOM());
            Assert.Equal(TextReadingState.UnexpectedEndOfStream, result.LastReadingState);
            Assert.Equal(startPosition, inputSourceMock.Object.Position);

            inputSourceMock.VerifySet(mocked => mocked.Position = It.IsAny<long>(), Times.Exactly(3));
            inputSourceMock.VerifyGet(mocked => mocked.Position, Times.AtLeastOnce());
            inputSourceMock.VerifyGet(mocked => mocked.Length, Times.AtLeastOnce());
            inputSourceMock.VerifyNoOtherCalls();
        }
    }
}
