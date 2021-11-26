using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MediaVC
{
    public static class StreamExtensions
    {
        public async static IAsyncEnumerable<ReadOnlyMemory<byte>> ToMemorySegments(this Stream source, long segmentMaxLength, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if(!source.CanRead)
                throw new IOException("Can't read from stream.");

            for(long position = 0; position < source.Length; position += segmentMaxLength)
            {
                var count = Math.Min(segmentMaxLength, source.Length - position);
                Memory<byte> buffer = new byte[count];

                _ = await source.ReadAsync(buffer, cancellationToken);

                yield return buffer;
            }
        }
    }
}
