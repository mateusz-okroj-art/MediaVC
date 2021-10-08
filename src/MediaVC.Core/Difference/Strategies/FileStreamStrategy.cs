using System;
using System.IO;

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

        public int Read(Memory<byte> buffer, int offset, int count) =>
            Stream.Read(buffer.Slice(offset, count).Span);

        public byte ReadByte()
        {
            var value = Stream.ReadByte();

            return value >= 0 ? (byte)value : throw new InvalidOperationException();
        }

        public bool Equals(IInputSourceStrategy? other)
        {
            var result =
                other is StreamStrategy strategy &&
                strategy.Length == Stream.Length;

            if(Stream is FileStream file1)
            {
                return result &&
                    strategy.Stream is FileStream file2 &&
                    file1.Name == file2.Name;
            }
        }

        #endregion
    }
}