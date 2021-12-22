using System;
using System.Collections;
using System.Collections.Generic;

namespace MediaVC.Core.Tests.TestData
{
    internal sealed class RandomBytesTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var random = new Random();
            var count = random.Next(2, 10);

            var list = new byte[count][];
            for (var i = 0; i < count; ++i)
                list[i] = Guid.NewGuid().ToByteArray();

            yield return new object[] { list };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
