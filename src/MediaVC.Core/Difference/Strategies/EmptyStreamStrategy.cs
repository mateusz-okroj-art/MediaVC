using System;

namespace MediaVC.Difference.Strategies
{
    internal sealed class EmptyStreamStrategy : IInputSourceStrategy
    {
        public long Length => 0;

        public long Position
        {
            get => -1;
            set => throw new ArgumentOutOfRangeException("Position", "Position is greather than Length.");
        }

        public bool Equals(IInputSourceStrategy? other) => other is not null && other.Length == 0;

        public int Read(byte[] buffer, int offset, int count) => throw new InvalidOperationException();

        public int Read(Memory<byte> buffer) => throw new InvalidOperationException();

        public byte ReadByte() => throw new InvalidOperationException();
    }
}
