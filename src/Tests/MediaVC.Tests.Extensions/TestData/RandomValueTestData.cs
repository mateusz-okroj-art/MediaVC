using System.Collections;

using MediaVC.Tests.Extensions.Helpers;

namespace MediaVC.Tests.TestData
{
    public class RandomValueTestData<Tvalue> : IEnumerable<object[]>
        where Tvalue : struct, IComparable<Tvalue>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { RandomGenerator.Generate((Tvalue)(dynamic)0, (Tvalue)(dynamic)100L) };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
