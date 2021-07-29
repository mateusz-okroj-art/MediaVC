using System;

using MediaVC.Difference;

namespace MediaVC.Tools.Tests.Fixtures
{
    public sealed class ThousandFullBytesReadonlyStream : IInputSource
    {
        #region Fields

        private const int count = 1000;
        private readonly Memory<byte> buffer = new byte[count];
        private long position;

        #endregion

        #region Constructor

        public ThousandFullBytesReadonlyStream()
        {
            var span = this.buffer.Span;

            for(var i = 0; i < count; ++i)
                span[i] = 255;
        }

        #endregion

        #region Properties

        public long Position
        {
            get => this.position;
            set
            {
                if(value is >= count or < 0)
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
                throw new ArgumentException("Buffer is empty");

            if(Position + offset >= Length)
                throw new ArgumentOutOfRangeException(nameof(offset));

            if(count < 1 || Position + offset + count - 1 >= Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            var destinationBufferArea = buffer.Slice(offset, count);

            this.buffer[(int)Position..].CopyTo(destinationBufferArea);

            var readedBytes = Math.Min(Length - Position, destinationBufferArea.Length);

            Position += readedBytes;

            return (int)readedBytes;
        }

        public byte ReadByte() =>
            Position >= Length
                ? throw new InvalidOperationException("Stream is on the end.")
                : this.buffer.Span[(int)this.position++..][0];

        #endregion
    }
}
