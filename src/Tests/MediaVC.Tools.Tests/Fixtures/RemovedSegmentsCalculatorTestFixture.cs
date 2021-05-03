using System;
using System.Collections.Generic;
using System.IO;

using MediaVC.Difference;
using MediaVC.Difference.FileSegments;

using Moq;

namespace MediaVC.Tools.Tests.Fixtures
{
    public sealed class RemovedSegmentsCalculatorTestFixture : IDisposable, IRemovedSegmentsCalculatorTestFixture
    {
        #region Constructor

        public RemovedSegmentsCalculatorTestFixture()
        {
            ConfigureSource1();

            ConfigureSource2();

            Test1_Segments = new IFileSegmentInfo[]
            {
                new FileSegmentInfo
                {
                    Source = InputSource1,
                    StartPosition = 0,
                    EndPosition = InputSource1.Length - 1
                }
            };

            Test2_Segments = new IFileSegmentInfo[]
            {
                new FileSegmentInfo
                {
                    Source = InputSource1,
                    StartPosition = 0,
                    EndPosition = 1
                },
                new FileSegmentInfo
                {
                    Source = InputSource2,
                    StartPosition = 2,
                    EndPosition = 3
                }
            };
        }

        #endregion

        #region Fields

        private MemoryStream stream1;
        private MemoryStream stream2;

        #endregion

        #region Properties

        public IInputSource InputSource1 { get; private set; }

        public IInputSource InputSource2 { get; private set; }

        public IEnumerable<IFileSegmentInfo> Test1_Segments { get; }

        public IEnumerable<IFileSegmentInfo> Test2_Segments { get; }

        #endregion

        #region Methods

        private void ConfigureSource1()
        {
            var content1 = new byte[] { 255, 255, 255, 255 };
            this.stream1 = new MemoryStream(content1, false);

            var mock = new Mock<IInputSource>();

            mock.SetupSet(mocked => mocked.Position = It.IsAny<long>())
                .Callback<long>(value => stream1.Position = value);
            mock.SetupGet(mocked => mocked.Position)
                .Returns(stream1.Position);

            mock.SetupGet(mocked => mocked.Length)
                .Returns(stream1.Length);

            mock.Setup(mocked => mocked.ReadByte())
                .Returns(ReadByteFromStream1());

            mock.Setup(mocked => mocked.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns<byte[], int, int>((buffer, offset, count) => stream1.Read(buffer, offset, count));

            mock.Setup(mocked => mocked.Read(It.IsAny<Memory<byte>>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns<Memory<byte>, int, int>((buffer, offset, count) => stream1.Read(buffer.Slice(offset, count).Span));

            InputSource1 = mock.Object;
        }

        private void ConfigureSource2()
        {
            var content2 = new byte[] { 0, 0, 0, 0 };
            this.stream2 = new MemoryStream(content2);

            var mock = new Mock<IInputSource>();

            mock.SetupSet(mocked => mocked.Position = It.IsAny<long>())
                .Callback<long>(value => stream2.Position = value);
            mock.SetupGet(mocked => mocked.Position)
                .Returns(stream2.Position);

            mock.SetupGet(mocked => mocked.Length)
                .Returns(stream2.Length);

            mock.Setup(mocked => mocked.ReadByte())
                .Returns(ReadByteFromStream2());

            mock.Setup(mocked => mocked.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns<byte[], int, int>((buffer, offset, count) => stream2.Read(buffer, offset, count));

            mock.Setup(mocked => mocked.Read(It.IsAny<Memory<byte>>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns<Memory<byte>, int, int>((buffer, offset, count) => stream2.Read(buffer.Slice(offset, count).Span));

            InputSource2 = mock.Object;
        }

        private byte ReadByteFromStream1()
        {
            var result = this.stream1.ReadByte();

            return result != -1 ? (byte)result : throw new InvalidOperationException();
        }

        private byte ReadByteFromStream2()
        {
            var result = this.stream2.ReadByte();

            return result != -1 ? (byte)result : throw new InvalidOperationException();
        }

        public void Dispose()
        {
            this.stream1?.Dispose();
            this.stream2?.Dispose();
            InputSource1.Dispose();
            InputSource2.Dispose();
        }

        #endregion
    }
}
