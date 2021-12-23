using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MediaVC.Difference
{
    public interface IStringReader
    {
        IAsyncEnumerable<string> Lines { get; }

        int Read();

        int Read(char[] buffer, int index, int count);

        int Read(Span<char> buffer);

        Task<int> ReadAsync(char[] buffer, int index, int count);

        ValueTask<int> ReadAsync(Memory<char> buffer, CancellationToken cancellationToken = default);

        string? ReadLine();

        Task<string?> ReadLineAsync();
    }
}