using System.Collections;
using System.Security.Cryptography;

namespace MediaVC.Tests.TestData
{
    public sealed class RandomBytesTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var count = RandomNumberGenerator.GetInt32(2, 10);

            var list = new byte[count][];
            for (var i = 0; i < count; ++i)
                list[i] = Guid.NewGuid().ToByteArray();

            yield return new object[] { list };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
