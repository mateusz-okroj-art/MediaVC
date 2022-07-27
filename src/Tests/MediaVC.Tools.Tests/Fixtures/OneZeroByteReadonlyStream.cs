
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediaVC.Difference;
using MediaVC.Enumerators;

namespace MediaVC.Tools.Tests.Fixtures
{
    internal sealed class OneZeroByteReadonlyStream : IInputSource
    {
        private long position;

        public long Position
        {
            get => this.position;
            set
            {
                if(value != 0)
                    throw new ArgumentOutOfRangeException(nameof(Position));

                this.position = value;
            }
        }

        public long Length => 1;

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        public IAsyncEnumerator<byte> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new InputSourceEnumerator(this, cancellationToken);

        public int Read(byte[] buffer, int offset, int count) =>
            buffer is not null ?
            ReadAsync(buffer.AsMemory().Slice(offset, count)).AsTask().GetAwaiter().GetResult() :
            throw new ArgumentNullException(nameof(buffer));

        public async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                if(buffer.IsEmpty || buffer.Length < 1)
                    return 0;

                buffer.Span[0] = 0;

                return 1;
            });

        public ValueTask<byte> ReadByteAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if(Position != 0)
                throw new InvalidOperationException("Stream is on end.");

            ++this.position;

            return ValueTask.FromResult<byte>(0);
        }
    }
}
