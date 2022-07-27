using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;

namespace MediaVC.Tools
{
    public static class ChecksumCalculator
    {
        public async static IAsyncEnumerable<ReadOnlyMemory<byte>> CalculateAsync(Stream dataStream, int segmentMaxLength, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dataStream, nameof(dataStream));

            var segments = dataStream.ToMemorySegments(segmentMaxLength, cancellationToken);

            await foreach(var hash in CalculateInternalAsync(segments, cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return hash;
            }
        }

        internal async static IAsyncEnumerable<ReadOnlyMemory<byte>> CalculateInternalAsync(IAsyncEnumerable<ReadOnlyMemory<byte>> dataSegments, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dataSegments, nameof(dataSegments));

            await foreach(var segment in dataSegments)
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return SHA512.HashData(segment.Span);
            }
        }
    }
}
