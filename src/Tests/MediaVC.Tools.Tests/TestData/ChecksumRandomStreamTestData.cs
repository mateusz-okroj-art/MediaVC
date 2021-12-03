using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaVC.Tools.Tests.TestData
{
    internal class ChecksumRandomStreamTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var random = new Random();
            var length = random.Next(1,255) * 16;
            var segmentsCount = random.Next(1, length);

            var buffer = new byte[length];
            for (var i = 0; i < length; i += 16)
            {
                var bytes = Guid.NewGuid().ToByteArray();

                bytes.CopyTo(buffer.AsMemory().Slice(i, 16));
            }

            var stream = new MemoryStream(buffer, false);

            var segmentMaxLength = length / segmentsCount;

            var calculatedSegments = ChecksumCalculator.CalculateInternalAsync(new ReadOnlyMemory<byte>(buffer).Split(segmentMaxLength).ToAsyncEnumerable());
            yield return new object[] { stream, segmentMaxLength, calculatedSegments };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
