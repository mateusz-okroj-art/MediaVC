using System;
using System.Linq;

using MediaVC.Difference;

using Moq;

using Xunit;

namespace MediaVC.Core.Tests.Difference.Strategies
{
    public class FileSegmentStrategy
    {
        [Fact]
        public void Constructor_WhenArgumentIsNull_ShouldThrowException()
        {
            Assert.Throws<ArgumentNullException>(() => new MediaVC.Difference.Strategies.FileSegmentStrategy(null));
        }

        [Fact]
        public void Length_WhenArgumentIsEmpty_ShouldReturn0()
        {
            var argument = Enumerable.Empty<IFileSegmentInfo>();

            var result = new MediaVC.Difference.Strategies.FileSegmentStrategy(argument);

            Assert.Equal(0, result.Length);
        }

        [Fact]
        public void Length_WhenArgumentHaveItems_ShouldReturnSummedLength()
        {
            var argument = new IFileSegmentInfo[]
            {
                Mock.Of<IFileSegmentInfo>(mock => mock.Length == 1),
                Mock.Of<IFileSegmentInfo>(mock => mock.Length == 10),
                Mock.Of<IFileSegmentInfo>(mock => mock.Length == 100)
            };

            var strategy = new MediaVC.Difference.Strategies.FileSegmentStrategy(argument);

            Assert.Equal(argument.Select(item => (decimal)item.Length).Sum(), strategy.Length);
        }

        [Fact]
        public void Position_WhenSettedValueIsTooLarge_ShouldThrowException()
        {
            var argument = new IFileSegmentInfo[]
            {
                Mock.Of<IFileSegmentInfo>(mock => mock.Length == 1)
            };

            var strategy = new MediaVC.Difference.Strategies.FileSegmentStrategy(argument);

            Assert.Throws<ArgumentOutOfRangeException>(() => strategy.Position = 1);
        }

        [Fact]
        public void Position_WhenSettedValueIsValid_ShouldSetProperty()
        {
            var argument = new IFileSegmentInfo[]
            {
                Mock.Of<IFileSegmentInfo>(mock => mock.Length == 2)
            };
            var strategy = new MediaVC.Difference.Strategies.FileSegmentStrategy(argument);

            strategy.Position = 1;

            Assert.Equal(1, strategy.Position);
        }
    }
}
