using System.Collections;

using MediaVC.Tests.Extensions.Helpers;

namespace MediaVC.Tests.Extensions.TestData
{
    public class RandomValueTestData<Tvalue> : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { RandomGenerator.Generate(0L, 100L) };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
