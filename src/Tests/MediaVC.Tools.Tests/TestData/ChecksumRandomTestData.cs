using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace MediaVC.Tools.Tests.TestData
{
    internal class ChecksumRandomTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var count = new Random().Next(1,10);

            var list = new List<(byte[], byte[])>();

            for(byte i = 1; i <= count; ++i)
            {
                var data = Guid.NewGuid().ToByteArray();

                list.Add((data, SHA512.HashData(data)));
            }

            yield return new object[] { list.ToArray() };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
