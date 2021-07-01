using System;
using System.IO;

using MediaVC.Difference;

using Moq;

namespace MediaVC.Tools.Tests
{
    public sealed class DifferenceCalculatorTestFixture : IDifferenceCalculatorTestFixture
    {
        #region Constructor

        public DifferenceCalculatorTestFixture()
        {
            var oneZeroArray = new byte[] { 0 };
            this.oneZeroStream = new MemoryStream(oneZeroArray);

            var oneZeroMock = new Mock<IInputSource>();
            ConfigureMockToStream(oneZeroMock, this.oneZeroStream);
            OneZero = oneZeroMock.Object;

            var mFullBytesArray = new byte[1_000_000];
            for(var i = 0; i < 999_999; ++i)
                mFullBytesArray[i] = 255;

            this.mFullBytesStream = new MemoryStream(mFullBytesArray);

            var mFullBytesMock = new Mock<IInputSource>();
            ConfigureMockToStream(mFullBytesMock, this.mFullBytesStream);
            MFullBytes = mFullBytesMock.Object;
        }


        #endregion

        #region Fields

        private readonly Stream oneZeroStream;
        private readonly Stream mFullBytesStream;

        #endregion

        #region Properties

        public IInputSource OneZero { get; }

        public IInputSource MFullBytes { get; }

        #endregion

        #region Methods

        private void ConfigureMockToStream(Mock<IInputSource> mock, Stream sourceStream)
        {
            mock.SetupGet(mock => mock.Position)
                .Returns(sourceStream.Position);
            mock.SetupSet(mock => mock.Position = It.IsAny<long>())
                .Callback(new Action<long>(newPosition => sourceStream.Position = newPosition));
            mock.SetupGet(mock => mock.Length)
                .Returns(sourceStream.Length);
            mock.Setup(mock => mock.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new Func<byte[], int, int, int>((buffer, offset, count) => sourceStream.Read(buffer, offset, count)));
            mock.Setup(mock => mock.Read(It.IsAny<Memory<byte>>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new Func<Memory<byte>, int, int, int>((buffer, offset, count) => sourceStream.Read(buffer.Span.Slice(offset, count))));
            mock.Setup(mock => mock.ReadByte())
                .Returns(sourceStream.ReadByte() is int i && i >= 0 ? (byte)i : throw new InvalidOperationException());
        }

        public void Dispose()
        {
            this.oneZeroStream.Dispose();
            this.mFullBytesStream.Dispose();
        }

        #endregion
    }
}
