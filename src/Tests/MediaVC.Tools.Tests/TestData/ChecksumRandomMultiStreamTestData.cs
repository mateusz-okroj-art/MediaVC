using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MediaVC.Tools.Tests.TestData
{
    internal class ChecksumRandomMultiStreamTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var testDataBuffer = new byte[4][];
            var summedBuffer = new byte[4 * 16];

            for(byte i = 0; i < 4; ++i)
            {
                testDataBuffer[i] = Guid.NewGuid().ToByteArray();
                testDataBuffer[i].CopyTo(summedBuffer, i * 4);
            }

            yield return new object[] { new MemoryStream(summedBuffer), testDataBuffer.Select(arr => new MemoryStream(arr)) };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
