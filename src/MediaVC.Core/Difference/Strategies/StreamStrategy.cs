using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MediaVC.Difference.Strategies
{
    internal sealed class StreamStrategy : IInputSourceStrategy, IDisposable, IAsyncDisposable
    {
        #region Constructor

        public StreamStrategy(Stream stream)
        {
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));

            if (!Stream.CanRead)
                throw new IOException("Stream is not readable.");
        }

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

        public async ValueTask<byte> ReadByteAsync(CancellationToken cancellationToken = default) =>
            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var value = Stream.ReadByte();

                cancellationToken.ThrowIfCancellationRequested();

                return value >= 0 ? (byte)value : throw new InvalidOperationException();
            });

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

        public ValueTask DisposeAsync() => Stream.DisposeAsync();

        public void Dispose() => Stream.Dispose();

        #endregion
    }
}