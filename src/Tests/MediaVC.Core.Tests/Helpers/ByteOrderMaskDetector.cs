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

        [Theory]
        [InlineData(ByteOrder.LittleEndian)]
        [InlineData(ByteOrder.BigEndian)]
        public async Task ScanForUTF16BOM_WhenStreamHaveBOM_ShouldReturnValid(ByteOrder variant)
        {
            ReadOnlyMemory<byte> buffer;
            switch(variant)
            {
                case ByteOrder.LittleEndian:
                    buffer = new byte[] { 0xff, 0xfe };
                    break;
                case ByteOrder.BigEndian:
                    buffer = new byte[] { 0xfe, 0xff };
                    break;
                default:
                    throw new ArgumentException("TestData: Invalid variant.");
            }

            var inputSource = new InputSource(buffer);

            var result = new MediaVC.Helpers.ByteOrderMaskDetector(inputSource);

            Assert.Equal(variant, await result.ScanForUTF16BOM());
            Assert.Equal(TextReadingState.Done, result.LastReadingState);
        }

        [Fact]
        public async Task ScanForUTF8BOM_WhenAvailableDataIsTooSmall_ShouldSetError()
        {
            var inputSourceMock = new Mock<IInputSource>(MockBehavior.Strict);

            const long startPosition = -1;
            inputSourceMock.SetupProperty(mocked => mocked.Position);
            inputSourceMock.Object.Position = startPosition;

            inputSourceMock.SetupGet(mocked => mocked.Length)
                .Returns(1);

            var result = new MediaVC.Helpers.ByteOrderMaskDetector(inputSourceMock.Object);

            Assert.False(await result.ScanForUTF8BOM());
            Assert.Equal(TextReadingState.UnexpectedEndOfStream, result.LastReadingState);
            Assert.Equal(startPosition, inputSourceMock.Object.Position);

            inputSourceMock.VerifySet(mocked => mocked.Position = It.IsAny<long>(), Times.Exactly(3));
            inputSourceMock.VerifyGet(mocked => mocked.Position, Times.AtLeastOnce());
            inputSourceMock.VerifyGet(mocked => mocked.Length, Times.AtLeastOnce());
            inputSourceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ScanForUTF8BOM_WhenStreamHaveBOM_ShouldReturnValid()
        {
            ReadOnlyMemory<byte> buffer = new byte[] { 0xef, 0xbb, 0xbf };

            var inputSource = new InputSource(buffer);

            var result = new MediaVC.Helpers.ByteOrderMaskDetector(inputSource);

            Assert.True(await result.ScanForUTF8BOM());
            Assert.Equal(TextReadingState.Done, result.LastReadingState);
        }

        [Fact]
        public async Task ScanForUTF32BOM_WhenAvailableDataIsTooSmall_ShouldSetError()
        {
            var inputSourceMock = new Mock<IInputSource>(MockBehavior.Strict);

            const long startPosition = -1;
            inputSourceMock.SetupProperty(mocked => mocked.Position);
            inputSourceMock.Object.Position = startPosition;

            inputSourceMock.SetupGet(mocked => mocked.Length)
                .Returns(1);

            var result = new MediaVC.Helpers.ByteOrderMaskDetector(inputSourceMock.Object);

            Assert.Null(await result.ScanForUTF32BOM());
            Assert.Equal(TextReadingState.UnexpectedEndOfStream, result.LastReadingState);
            Assert.Equal(startPosition, inputSourceMock.Object.Position);

            inputSourceMock.VerifySet(mocked => mocked.Position = It.IsAny<long>(), Times.Exactly(3));
            inputSourceMock.VerifyGet(mocked => mocked.Position, Times.AtLeastOnce());
            inputSourceMock.VerifyGet(mocked => mocked.Length, Times.AtLeastOnce());
            inputSourceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(ByteOrder.LittleEndian)]
        [InlineData(ByteOrder.BigEndian)]
        public async Task ScanForUTF32BOM_WhenStreamHaveBOM_ShouldReturnValid(ByteOrder variant)
        {
            ReadOnlyMemory<byte> buffer;
            switch(variant)
            {
                case ByteOrder.LittleEndian:
                    buffer = new byte[] { 0xff, 0xfe, 0, 0 };
                    break;
                case ByteOrder.BigEndian:
                    buffer = new byte[] { 0, 0, 0xfe, 0xff };
                    break;
                default:
                    throw new ArgumentException("TestData: Invalid variant.");
            }

            var inputSource = new InputSource(buffer);

            var result = new MediaVC.Helpers.ByteOrderMaskDetector(inputSource);

            Assert.Equal(variant, await result.ScanForUTF32BOM());
            Assert.Equal(TextReadingState.Done, result.LastReadingState);
        }
    }
}
