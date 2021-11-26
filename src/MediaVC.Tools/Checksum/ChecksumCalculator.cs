using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MediaVC.Tools
{
    public static class ChecksumCalculator
    {
        public async static IAsyncEnumerable<ReadOnlyMemory<byte>> CalculateAsync(Stream dataStream, int segmentMaxLength, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var segments = dataStream.ToMemorySegments(segmentMaxLength, cancellationToken);
            
            await foreach(var hash in CalculateInternalAsync(segments, cancellationToken))
                yield return hash;
        }

        private async static IAsyncEnumerable<Memory<byte>> CalculateInternalAsync(IAsyncEnumerable<ReadOnlyMemory<byte>> dataSegments, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach(var segment in dataSegments)
                yield return SHA512.HashData(segment.Span);
        }
    }
}
