using System;

using Xunit;

namespace MediaVC.Tools.Tests.Detection.TextDetector.Strategies.MemoryTextDetectionStrategy
{
    public class Constructor
    {
        [Fact]
        public void Constructor_WhenArgumentIsEmpty_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Tools.Detection.Strategies.MemoryTextDetectionStrategy(Memory<byte>.Empty));
        }

        [Fact]
        public void Constructor_WhenArgumentIsValid_ShouldSetProperty()
        {
            var memory = new Memory<byte>(new byte[1]);

            var result = new Tools.Detection.Strategies.MemoryTextDetectionStrategy(memory);

            Assert.Equal(memory, result.Memory);
        }
    }
}
