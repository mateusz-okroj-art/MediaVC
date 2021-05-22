using System;
using System.IO;

namespace MediaVC.Difference.Strategies
{
    internal sealed class FileStreamStrategy : IInputSourceStrategy
    {
        #region Constructor

        public FileStreamStrategy(FileStream file)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));

            if (!File.CanRead)
                throw new IOException("Stream is not readable.");
        }

        #endregion

        #region Properties

        public FileStream File { get; }

        public long Length => File.Length;

        public long Position
        {
            get => File.Position;
            set => File.Position = value;
        }

        #endregion

        #region Methods

        public int Read(byte[] buffer, int offset, int count) =>
            File.Read(buffer, offset, count);

        public int Read(Memory<byte> buffer, int offset, int count) =>
            File.Read(buffer.Slice(offset, count).Span);

        public byte ReadByte()
        {
            var value = File.ReadByte();

            return value >= 0 ? (byte)value : throw new InvalidOperationException();
        }

        public bool Equals(IInputSourceStrategy? other) =>
            other is FileStreamStrategy strategy &&
                strategy.File?.Name == File.Name;

        #endregion
    }
}