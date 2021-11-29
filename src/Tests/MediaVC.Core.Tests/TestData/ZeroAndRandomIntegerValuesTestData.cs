using System;
using System.Collections;
using System.Collections.Generic;

namespace MediaVC.Core.Tests.TestData
{
    internal class ZeroAndRandomIntegerValuesTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { 0 };

            var rand = new Random();
            yield return new object[] { rand.Next(1, int.MaxValue) };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
