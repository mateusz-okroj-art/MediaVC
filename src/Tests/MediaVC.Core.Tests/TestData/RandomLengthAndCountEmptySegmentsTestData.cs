using System;
using System.Collections;
using System.Collections.Generic;

using MediaVC.Difference;

using Moq;

namespace MediaVC.Core.Tests.TestData
{
    internal sealed class RandomLengthAndCountEmptySegmentsTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var random = new Random();

            var count = random.Next(1, 10);

            var list = new IFileSegmentInfo[count];
            for(byte i = 0; i < count; ++i)
            {
                var length = random.Next(1, int.MaxValue);
                list[i] = Mock.Of<IFileSegmentInfo>(mock => mock.Length == (ulong)length && mock.Source == Mock.Of<IInputSource>(mock => mock.Length == length));
            }

            yield return new object[] { list };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
