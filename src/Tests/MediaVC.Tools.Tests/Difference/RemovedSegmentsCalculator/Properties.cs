using MediaVC.Difference;

using Moq;

using Xunit;

namespace MediaVC.Tools.Tests.Difference.RemovedSegmentsCalculator
{
    public class Properties
    {
        [Fact]
        public void AfterInitialized_ShouldReturnEmptyList()
        {
            var sourceMock = new Mock<IInputSource>();
            sourceMock.Setup(mocked => mocked.Position).Returns(0);
            sourceMock.Setup(mocked => mocked.Length).Returns(1);

            var source = sourceMock.Object;

            var segments = new IFileSegmentInfo[]
            {
                new FileSegmentInfo
                {
                    Source = source,
                    StartPositionInSource = 0,
                    EndPositionInSource = 0
                }
            };

            var result = new Tools.Difference.RemovedSegmentsCalculator(segments, source);

            Assert.NotNull(result.Result);
            Assert.Empty(result.Result);
        }
    }
}
