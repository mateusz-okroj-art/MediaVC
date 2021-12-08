using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MediaVC.Difference.Strategies
{
    internal sealed class StreamStrategy : IInputSourceStrategy
    {
        #region Constructor

        public StreamStrategy(Stream stream)
        {
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));

            if (!Stream.CanRead)
                throw new IOException("Stream is not readable.");
        }

        private const int BufferLength = 4000;

        #endregion

        #region Fields

        private readonly Memory<byte> readerBuffer = new byte[BufferLength];
        private long bufferStartPosition = -1;

        #endregion

        #region Properties

        public Stream Stream { get; }

        public long Length => Stream.Length;

        public long Position
        {
            get => Stream.Position;
            set => Stream.Position = value;
        }

        #endregion

        #region Methods

        public int Read(byte[] buffer, int offset, int count) =>
            Stream.Read(buffer, offset, count);

        public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
            Stream.ReadAsync(buffer, cancellationToken);

        public async ValueTask<byte> ReadByteAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if(Position < this.bufferStartPosition || Position > this.bufferStartPosition + BufferLength)
            {
                var currentPosition = Position;
                this.bufferStartPosition = Position;
                _ = await ReadAsync(this.readerBuffer, cancellationToken);
                Position = currentPosition;
            }

            var value = this.readerBuffer.Span[(int)(Position - this.bufferStartPosition)];

            ++Position;

            return value;
        }

        public bool Equals(IInputSourceStrategy? other)
        {
            var strategy = other as StreamStrategy;

            var result =
                strategy is not null &&
                strategy.Length == Stream.Length;

            return Stream is FileStream file1
                ? result &&
                    strategy?.Stream is FileStream file2 &&
                    file1.Name == file2.Name
                : result;
        }

        public override bool Equals(object? obj) => Equals(obj as IInputSourceStrategy);

        public override int GetHashCode() => Stream.GetHashCode();

        #endregion
    }
}