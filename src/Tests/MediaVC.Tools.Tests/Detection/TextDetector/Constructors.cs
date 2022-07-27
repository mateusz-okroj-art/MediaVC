using System;
using System.IO;

using Xunit;

namespace MediaVC.Tools.Tests.Detection.TextDetector
{
    public class Constructors
    {
        [Fact]
        public void Constructor1_WhenArgumentIsNull_ShouldThrowException()
        {
            Stream stream = null;

            Assert.Throws<ArgumentNullException>(() => new Tools.Detection.TextDetector(stream));
        }

        [Fact]
        public void Constructor2_WhenArgumentIsEmpty_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Tools.Detection.TextDetector(Memory<byte>.Empty));
        }
    }
}
