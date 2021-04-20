using System;
using System.IO;
using System.Threading.Tasks;

namespace MediaVC.Difference.Strategies
{
    internal sealed class FileStreamStrategy : IInputSourceStrategy
    {
        public FileStreamStrategy(FileStream file)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));

            if (!File.CanRead)
                throw new IOException("Stream is not readable.");
        }

        public FileStream File { get; }

        public long Length => File.Length;

        public long Position
        {
            get => File.Position;
            set => File.Position = value;
        }

        public int Read(byte[] buffer, int offset, int count) =>
            File.Read(buffer, offset, count);

        public ValueTask<int> ReadAsync(byte[] buffer, int offset, int count) =>
            File.ReadAsync(byte[] buffer, int offset, int count);


        public byte ReadByte()
        {
            var value = File.ReadByte();

            if (value >= 0)
                return value;
            else
                throw new InvalidOperationException();
        }

        public bool Equals(IInputSourceStrategy? other)
        {
            if (other is FileStreamStrategy strategy &&
                strategy.File?.Name == File.Name)
                return true;

            return false;
        }
    }
}
