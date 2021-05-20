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
        public void Constructor1_WhenArgumentIsValid_ShouldInitialize()
        {
            new Tools.Detection.TextDetector(Stream.Null);
        }

        [Fact]
        public void Constructor2_WhenArgumentIsEmpty_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Tools.Detection.TextDetector(Memory<byte>.Empty));
        }

        [Fact]
        public void Constructor2_WhenArgumentIsValid_ShouldInitialize()
        {
            new Tools.Detection.TextDetector(new Memory<byte>(new byte[1]));
        }
    }
}
