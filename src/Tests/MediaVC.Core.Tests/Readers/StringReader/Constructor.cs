using System;

using MediaVC.Difference;

using Xunit;

namespace MediaVC.Core.Tests.Readers.StringReader
{
    public class Constructor
    {
        [Fact]
        public void Constructor_WhenArgumentIsNull_ShouldThrow()
        {
            IInputSource argument = null;

            Assert.Throws<ArgumentNullException>(() => new MediaVC.Readers.StringReader(argument));
        }
    }
}
