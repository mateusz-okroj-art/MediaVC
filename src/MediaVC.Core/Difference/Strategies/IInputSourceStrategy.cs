using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediaVC.Difference.Strategies
{
    public interface IInputSourceStrategy : IEquatable<IInputSourceStrategy>
    {
        long Length { get; }

        long Position { get; set; }

        int Read(byte[] buffer, int offset, int count);

        ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);

        ValueTask<byte> ReadByteAsync(CancellationToken cancellationToken = default);
    }
}