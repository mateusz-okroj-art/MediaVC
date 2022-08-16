using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MediaVC.Difference.Concurrency
{
    internal sealed class InputSourceConcurrentProvider : IInputSourceConcurrentProvider
    {
        internal InputSourceConcurrentProvider(IInputSource originalSource) =>
            this.originalSource = originalSource ?? throw new ArgumentNullException(nameof(originalSource));

        private readonly IInputSource originalSource;

        public SynchronizationObject SynchronizationObject { get; } = new SynchronizationObject();

        public long Position
        {
            get => originalSource.Position;
            set => originalSource.Position = value;
        }

        public long Length { get; }

        [Obsolete("Use Enumerator with InputSourceProxy.")]
        public IAsyncEnumerator<byte> GetAsyncEnumerator(CancellationToken cancellationToken = default) => throw new InvalidOperationException("Use Enumerator with InputSourceProxy.");

        public int Read(byte[] buffer, int offset, int count) => this.originalSource.Read(buffer, offset, count);

        public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => this.originalSource.ReadAsync(buffer, cancellationToken);

        public ValueTask<byte> ReadByteAsync(CancellationToken cancellationToken = default) => this.originalSource.ReadByteAsync(cancellationToken);
    }
}
