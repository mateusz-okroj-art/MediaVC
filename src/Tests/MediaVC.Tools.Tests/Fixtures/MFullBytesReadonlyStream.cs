using System;

using MediaVC.Difference;

namespace MediaVC.Tools.Tests.Fixtures
{
    public sealed class MFullBytesReadonlyStream : IInputSource
    {
        #region Fields

        private const int M = 1_000_000;
        private readonly Memory<byte> buffer = new byte[M];
        private long position;

        #endregion

        #region Constructor

        public MFullBytesReadonlyStream()
        {
            var span = this.buffer.Span;

            for(var i = 0; i < M; ++i)
                span[i] = 255;
        }

        #endregion

        #region Properties

        public long Position
        {
            get => this.position;
            set
            {
                if(value is >= M or < 0)
                    throw new ArgumentOutOfRangeException();

                this.position = value;
            }
        }

        public long Length => this.buffer.Length;

        #endregion

        #region Methods

        public void Dispose() { }

        public int Read(byte[] buffer, int offset, int count) =>
            Read(buffer.AsMemory(), offset, count);

        public int Read(Memory<byte> buffer, int offset, int count)
        {
            if(buffer.IsEmpty)
                throw new ArgumentException(nameof(buffer));

            if(Position + offset >= Length)
                throw new ArgumentOutOfRangeException(nameof(offset));

            if(count < 1 || Position + offset + count - 1 >= Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            var destinationBufferArea = buffer.Slice(offset, count);

            this.buffer.Slice((int)Position).CopyTo(destinationBufferArea);

            var readedBytes = Math.Min(Length - Position, destinationBufferArea.Length);

            Position += readedBytes;

            return (int)readedBytes;
        }

        public byte ReadByte()
        {
            if(Position >= Length)
                throw new InvalidOperationException("Stream is on the end.");

            return this.buffer.Span.Slice((int)Position)[0];
        }

        #endregion
    }
}
