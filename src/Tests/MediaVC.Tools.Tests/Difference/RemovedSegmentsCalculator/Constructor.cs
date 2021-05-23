using System;
using System.Collections.Generic;
using System.Linq;

using MediaVC.Difference;

using Moq;

using Xunit;

namespace MediaVC.Tools.Tests.Difference.RemovedSegmentsCalculator
{
    public class Constructor
    {
        #region TestData

        public static object[][] NullArguments { get; } = new object[][]
        {
            new object[] { null, null },
            new object[] { Enumerable.Empty<IFileSegmentInfo>(), null },
            new object[] { null, new Mock<IInputSource>().Object }
        };

        #endregion

        [Theory]
        [MemberData(nameof(NullArguments))]
        public void WhenArgumentIsNull_ShouldThrowException(IEnumerable<IFileSegmentInfo> arg1, IInputSource arg2)
        {
            Assert.Throws<ArgumentNullException>(() => new Tools.Difference.RemovedSegmentsCalculator(arg1, arg2));
        }

        [Fact]
        public void WhenArgumentIsValid_ShouldSetProperties()
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
                    StartPosition = 0,
                    EndPosition = 0
                }
            };

            var result = new Tools.Difference.RemovedSegmentsCalculator(segments, source);

            Assert.Equal(source, result.Source);
            Assert.Equal(segments, result.Segments);
        }
    }
}
