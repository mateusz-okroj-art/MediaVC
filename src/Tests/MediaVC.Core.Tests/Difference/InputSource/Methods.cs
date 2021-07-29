using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediaVC.Difference.Strategies;

using Moq;

using Xunit;

namespace MediaVC.Core.Tests.Difference.InputSource
{
    public class Methods
    {
        [Fact]
        public void Read1_ShouldInvokeInStrategy()
        {
            var mock = new Mock<IInputSourceStrategy>();
            mock.Setup(mocked => mocked.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(0);

            var result = new MediaVC.Difference.InputSource(mock.Object);

            Assert.Equal(0, result.Read(new byte[0], 0, 0));

            mock.Verify(mocked => mocked.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()));
        }

        [Fact]
        public void Read2_ShouldInvokeInStrategy()
        {
            var mock = new Mock<IInputSourceStrategy>();
            mock.Setup(mocked => mocked.Read(It.IsAny<Memory<byte>>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(0);

            var result = new MediaVC.Difference.InputSource(mock.Object);

            Assert.Equal(0, result.Read(Memory<byte>.Empty, 0, 0));

            mock.Verify(mocked => mocked.Read(It.IsAny<Memory<byte>>(), It.IsAny<int>(), It.IsAny<int>()));
        }
    }
}
