using System.Collections;
using System.Security.Cryptography;

using MediaVC.Difference;

using Moq;

namespace MediaVC.Tests.TestData
{
    public sealed class RandomLengthAndCountEmptySegmentsTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var count = RandomNumberGenerator.GetInt32(1, 10);

            var list = new IFileSegmentInfo[count];
            for(byte i = 0; i < count; ++i)
            {
                var length = RandomNumberGenerator.GetInt32(1, int.MaxValue);
                list[i] = Mock.Of<IFileSegmentInfo>(mock => mock.Length == (ulong)length && mock.Source == Mock.Of<IInputSource>(mock => mock.Length == length));
            }

            yield return new object[] { list };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
