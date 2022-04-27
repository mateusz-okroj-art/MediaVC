using System;
using System.Threading;
using System.Threading.Tasks;

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

        public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => throw new InvalidOperationException();

        public ValueTask<byte> ReadByteAsync(CancellationToken cancellationToken = default) => throw new InvalidOperationException();
    }
}
