using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using MediaVC.Difference;
using MediaVC.Difference.Strategies;

using Moq;

using Xunit;

namespace MediaVC.Core.Tests.Difference.InputSource
{
    public class Constructors
    {
        [Fact]
        public void Constructor1_WhenArgumentIsNull_ShouldThrowException()
        {
            FileStream argument = null;

            Assert.Throws<ArgumentNullException>(() => new MediaVC.Difference.InputSource(argument));
        }

        [Fact]
        public void Constructor2_WhenArgumentIsNull_ShouldThrowException()
        {
            IEnumerable<IFileSegmentInfo> argument = null;

            Assert.Throws<ArgumentNullException>(() => new MediaVC.Difference.InputSource(argument));
        }

        [Fact]
        public void Constructor3_WhenArgumentIsNull_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new MediaVC.Difference.InputSource(ReadOnlyMemory<byte>.Empty));
        }

        [Fact]
        public void Constructor4_WhenArgumentIsNull_ShouldThrowException()
        {
            IInputSourceStrategy argument = null;

            Assert.Throws<ArgumentNullException>(() => new MediaVC.Difference.InputSource(argument));
        }

        [Fact]
        public void Constructor1_WhenArgumentIsValid_ShouldSetStrategy()
        {
            using var argument = File.Create($"test-{Guid.NewGuid()}.tmp", 1, FileOptions.DeleteOnClose);

            var result = new MediaVC.Difference.InputSource(argument);

            Assert.IsType<StreamStrategy>(result?.Strategy);
        }

        [Fact]
        public void Constructor2_WhenArgumentIsValid_ShouldSetStrategy()
        {
            var argument = Enumerable.Empty<IFileSegmentInfo>();

            var result = new MediaVC.Difference.InputSource(argument);

            Assert.IsType<FileSegmentStrategy>(result?.Strategy);
        }

        [Fact]
        public void Constructor3_WhenArgumentIsValid_ShouldSetStrategy()
        {
            ReadOnlyMemory<byte> argument = new byte[1];

            var result = new MediaVC.Difference.InputSource(argument);

            Assert.IsType<MemoryStrategy>(result?.Strategy);
        }

        [Fact]
        public void Constructor4_WhenArgumentIsValid_ShouldSetStrategy()
        {
            var argument = Mock.Of<IInputSourceStrategy>();

            var result = new MediaVC.Difference.InputSource(argument);

            Assert.True(ReferenceEquals(argument, result.Strategy));
        }
    }
}
