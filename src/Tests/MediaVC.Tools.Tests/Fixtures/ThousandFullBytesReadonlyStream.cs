using System;
using System.Threading;
using System.Threading.Tasks;

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
                    throw new ArgumentOutOfRangeException(nameof(Position));

                this.position = value;
            }
        }

        public long Length => this.buffer.Length;

        #endregion

        #region Methods

        public void Dispose() { }

        public int Read(byte[] buffer, int offset, int count) =>
            ReadAsync(buffer.AsMemory().Slice(offset,count)).GetAwaiter().GetResult();

        public async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
            await Task.Run(() =>
            {
                if(buffer.IsEmpty || buffer.Length < 1)
                    return 0;

                this.buffer[(int)Position..].CopyTo(buffer);

                var readedBytes = Math.Min(Length - Position, buffer.Length);

                Position += readedBytes;

                return (int)readedBytes;
            });

        /// <summary>
        /// Returns current byte and increments position.
        /// </summary>
        /// <returns></returns>
        public async ValueTask<byte> ReadByteAsync(CancellationToken cancellationToken = default) =>
            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return Position >= Length
                    ? throw new InvalidOperationException("Stream is on the end.")
                    : this.buffer.Span[(int)this.position++..][0];
            });

        #endregion
    }
}
