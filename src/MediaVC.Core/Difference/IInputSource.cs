using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediaVC.Difference
{
    public interface IInputSource : IDisposable
    {
        long Position { get; set; }

        long Length { get; }

        ValueTask<byte> ReadByteAsync(CancellationToken cancellationToken = default);

        int Read(byte[] buffer, int offset, int count);

        ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);
    }
}
