using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using MediaVC.Difference.Strategies;
using MediaVC.Enumerators;

namespace MediaVC.Difference
{
    /// <summary>
    /// Read-only source based on <see cref="Stream"/>
    /// </summary>
    public sealed class InputSource : Stream, IInputSource, IEquatable<InputSource>
    {
        #region Constructors

        /// <summary>
        /// Initializes file stream reading mode.
        /// </summary>
        /// <param name="file">Source file stream.</param>
        public InputSource(FileStream file)
        {
            ArgumentNullException.ThrowIfNull(file);

            Strategy = new StreamStrategy(file);
        }

        /// <summary>
        /// Initializes file segment merging mode.
        /// </summary>
        /// <param name="segments">Segments of data (MappedPosition is required).</param>
        /// <exception cref="InvalidOperationException"></exception>
        public InputSource(IEnumerable<IFileSegmentInfo> segments)
        {
            ArgumentNullException.ThrowIfNull(segments);
            var strategy = new FileSegmentStrategy(segments);

            if(!strategy.CheckIsNotUsedSource(this))
                throw new InvalidOperationException("Usage of this source has been detected on a child segment.");

            Strategy = strategy;
        }

        /// <summary>
        /// Initializes memory block reading mode.
        /// </summary>
        /// <param name="memory"></param>
        /// <exception cref="ArgumentException"></exception>
        public InputSource(ReadOnlyMemory<byte> memory)
        {
            if(memory.IsEmpty)
                throw new ArgumentException("Memory is empty.");

            Strategy = new MemoryStrategy(memory);
        }

        /// <summary>
        /// Allows to run custom mode.
        /// </summary>
        /// <param name="externalStrategy">Custom strategy</param>
        /// <exception cref="ArgumentNullException"></exception>
        internal InputSource(IInputSourceStrategy externalStrategy) =>
            Strategy = externalStrategy ?? throw new ArgumentNullException(nameof(externalStrategy));

        #endregion

        #region Properties

        /// <summary>
        /// Work strategy
        /// </summary>
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

        /// <summary>
        /// Default, empty source
        /// </summary>
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

        /// <summary>
        /// Asynchronously reads single byte
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

        [Obsolete("Input source is read-only.")]
        public override void Flush() => throw new InvalidOperationException();

        [Obsolete("Input source is read-only.")]
        public override void SetLength(long value) => throw new InvalidOperationException();

        [Obsolete("Input source is read-only.")]
        public override void Write(byte[] buffer, int offset, int count) => throw new InvalidOperationException();

        public IAsyncEnumerator<byte> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new InputSourceEnumerator(this, cancellationToken);

        #endregion

        #endregion
    }
}
