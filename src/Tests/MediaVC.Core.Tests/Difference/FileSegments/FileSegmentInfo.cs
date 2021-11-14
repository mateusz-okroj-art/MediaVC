using MediaVC.Difference;

using Moq;

using Xunit;

namespace MediaVC.Core.Tests.Difference.FileSegments
{
    public class FileSegmentInfo
    {
        [Fact]
        public void StartPosition_ShouldSetProperty()
        {
            MediaVC.Difference.FileSegmentInfo result = default;

            const long testValue = long.MaxValue;

            result.StartPositionInSource = testValue;

            Assert.Equal(testValue, result.StartPositionInSource);
        }

        [Fact]
        public void EndPosition_ShouldSetProperty()
        {
            MediaVC.Difference.FileSegmentInfo result = default;

            const long testValue = 1_000_000;

            result.EndPositionInSource = testValue;

            Assert.Equal(testValue, result.EndPositionInSource);
        }

        [Fact]
        public void Source_ShouldSetProperty()
        {
            MediaVC.Difference.FileSegmentInfo result = default;

            var testValue = new Mock<IInputSource>().Object;

            result.Source = testValue;

            Assert.Equal(testValue, result.Source);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, 1)]
        [InlineData(1, 100)]
        [InlineData(-100, 0)]
        [InlineData(100, -1000)]
        public void Length_TestCases_ShouldChangeAfterSetPosition(long start, long end)
        {
            MediaVC.Difference.FileSegmentInfo result = default;

            result.StartPositionInSource = start;
            result.EndPositionInSource = end;
            
            Assert.Equal(
                result.StartPositionInSource >= 0 && result.StartPositionInSource <= result.EndPositionInSource && result.EndPositionInSource >= result.StartPositionInSource ? (ulong)(result.EndPositionInSource - result.StartPositionInSource + 1) : 0,
                result.Length);
        }
    }
}