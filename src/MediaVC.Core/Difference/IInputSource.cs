using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MediaVC.Difference
{
    /// <summary>
    /// Provides access to input source of data
    /// </summary>
    public interface IInputSource : IAsyncDisposable, IAsyncEnumerable<byte>
    {
        long Position { get; set; }

        long Length { get; }

        ValueTask<byte> ReadByteAsync(CancellationToken cancellationToken = default);

        int Read(byte[] buffer, int offset, int count);

        ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);
    }
}
