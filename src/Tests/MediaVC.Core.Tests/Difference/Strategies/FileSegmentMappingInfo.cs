using MediaVC.Difference;

using Moq;

using Xunit;

namespace MediaVC.Core.Tests.Difference.Strategies
{
    public class FileSegmentMappingInfo
    {
        [Fact]
        public void Segment_ShouldSetProperty()
        {
            MediaVC.Difference.Strategies.FileSegmentMappingInfo result = default;

            var segmentInfo = new Mock<IFileSegmentInfo>().Object;

            result.Segment = segmentInfo;

            Assert.Equal(segmentInfo, result.Segment);
        }

        [Fact]
        public void StartIndex_ShouldSetProperty()
        {
            MediaVC.Difference.Strategies.FileSegmentMappingInfo result = default;

            const long testValue = 20;

            result.StartIndex = testValue;

            Assert.Equal(testValue, result.StartIndex);
        }

        [Theory]
        [InlineData(0, 0, 0, false)]
        [InlineData(0, 0, 1, true)]
        [InlineData(1, 0, 1, false)]
        [InlineData(1, 0, 2, true)]
        [InlineData(long.MaxValue-1, long.MaxValue-1, 1, true)]
        [InlineData(long.MaxValue-1, 0, long.MaxValue, true)]
        public void CheckIsPositionInRange_TestCases_ShouldValid(long position, long startIndex, long segmentLength, bool expectedResult)
        {
            MediaVC.Difference.Strategies.FileSegmentMappingInfo result = default;

            result.StartIndex = startIndex;

            var segmentMock = new Mock<IFileSegmentInfo>();
            segmentMock.SetupGet(model => model.Length)
                .Returns((ulong)segmentLength);
            result.Segment = segmentMock.Object;

            Assert.Equal(expectedResult, result.CheckPositionIsInRange(position));
        }
    }
}
