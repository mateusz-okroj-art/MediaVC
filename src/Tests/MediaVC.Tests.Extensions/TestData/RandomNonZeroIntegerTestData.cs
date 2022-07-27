using System.Collections;
using System.Security.Cryptography;

namespace MediaVC.Tests.TestData
{
    public class RandomNonZeroIntegerTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { RandomNumberGenerator.GetInt32(1, int.MaxValue) };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
