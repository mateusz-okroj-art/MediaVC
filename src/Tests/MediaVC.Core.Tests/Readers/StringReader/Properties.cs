using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediaVC.Difference;

using Moq;

using Xunit;

namespace MediaVC.Core.Tests.Readers.StringReader
{
    public class Properties
    {
        [Fact]
        public void LineEnding_ShouldBeEqual()
        {
            var inputSource = Mock.Of<IInputSource>(mock => mock.Length == 1 && mock.Position == 0);

            var reader = new MediaVC.Readers.StringReader(inputSource);

            _ = reader.Read();

            Assert.Equal(reader.readingEngine.LineEnding, reader.LineEnding);
        }
    }
}
