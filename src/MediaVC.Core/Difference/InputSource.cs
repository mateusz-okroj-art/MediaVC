using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using MediaVC.Difference.Strategies;
using MediaVC.Enumerators;

namespace MediaVC.Difference
{
    public sealed class InputSource : Stream, IInputSource, IEquatable<InputSource>
    {
        #region Constructors

        public InputSource(FileStream file)
        {
            ArgumentNullException.ThrowIfNull(file);

            Strategy = new StreamStrategy(file);
        }

        public InputSource(IEnumerable<IFileSegmentInfo> segments)
        {
            ArgumentNullException.ThrowIfNull(segments);
            var strategy = new FileSegmentStrategy(segments);

            if(!strategy.CheckIsNotUsedSource(this))
                throw new InvalidOperationException("Usage of this source has been detected on a child segment.");

            Strategy = strategy;
        }

        public InputSource(ReadOnlyMemory<byte> memory)
        {
            if(memory.IsEmpty)
                throw new ArgumentException("Memory is empty.");

            Strategy = new MemoryStrategy(memory);
        }

        internal InputSource(IInputSourceStrategy externalStrategy) =>
            Strategy = externalStrategy ?? throw new ArgumentNullException(nameof(externalStrategy));

        #endregion

        #region Properties

        internal IInputSourceStrategy Strategy { get; }

        public override bool CanRead { get; } = true;

        public override bool CanSeek { get; } = true;

        public override bool CanWrite { get; } = false;

        public override long Length => Strategy.Length;

        public override long Position
        {
            get => Strategy.Position;
            set => Strategy.Position = value;
        }

        public static IInputSource Empty { get; } = new InputSource(new EmptyStreamStrategy());

        #endregion

        #region Methods

        public override int Read(byte[] buffer, int offset, int count) =>
            Strategy.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin)
        {
            var calculatedPosition = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => Position + offset,
                SeekOrigin.End => Length - offset - 1,
                _ => throw new NotImplementedException(),
            };

            Position = calculatedPosition;

            return calculatedPosition;
        }

        public ValueTask<byte> ReadByteAsync(CancellationToken cancellationToken = default) => Strategy.ReadByteAsync(cancellationToken);

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
            Strategy.ReadAsync(buffer, cancellationToken);

        public bool Equals(InputSource? other) => Strategy.Equals(other?.Strategy);

        public override bool Equals(object? obj) => Equals(obj as InputSource);

        public override int GetHashCode() => Strategy.GetHashCode();

        protected override void Dispose(bool disposing)
        {
            if(disposing && Strategy is IDisposable disposable)
                disposable.Dispose();
        }

        public override async ValueTask DisposeAsync()
        {
            if(Strategy is IAsyncDisposable disposable)
                await disposable.DisposeAsync();
        }

        #region Obsoletes

        [Obsolete]
        public override void Flush() => throw new InvalidOperationException();

        [Obsolete]
        public override void SetLength(long value) => throw new InvalidOperationException();

        [Obsolete]
        public override void Write(byte[] buffer, int offset, int count) => throw new InvalidOperationException();

        public IAsyncEnumerator<byte> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new InputSourceEnumerator(this, cancellationToken);

        #endregion

        #endregion
    }
}
