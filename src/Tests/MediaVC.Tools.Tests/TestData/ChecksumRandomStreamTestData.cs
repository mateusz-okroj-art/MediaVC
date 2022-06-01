using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace MediaVC.Tools.Tests.TestData
{
    internal sealed class ChecksumRandomStreamTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var length = RandomNumberGenerator.GetInt32(1, 256) * 16;
            var segmentsCount = RandomNumberGenerator.GetInt32(1, length+1);

            var buffer = new byte[length];
            for (var i = 0; i < length; i += 16)
            {
                var bytes = Guid.NewGuid().ToByteArray();

                bytes.CopyTo(buffer.AsMemory().Slice(i, 16));
            }

            var stream = new MemoryStream(buffer, false);

            var segmentMaxLength = length / segmentsCount;

            var calculatedSegments = ChecksumCalculator.CalculateInternalAsync(new ReadOnlyMemory<byte>(buffer).Split(segmentMaxLength).ToArray().ToAsyncEnumerable());
            yield return new object[] { stream, segmentMaxLength, calculatedSegments };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
