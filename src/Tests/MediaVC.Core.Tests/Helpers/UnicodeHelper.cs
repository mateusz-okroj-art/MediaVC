using System.Text;

using Xunit;

namespace MediaVC.Core.Tests.Helpers
{
    public class UnicodeHelper
    {
        [Theory]
        [InlineData(0xD800U, true)]
        [InlineData(0xDFFFU, true)]
        [InlineData(0xD799U, false)]
        [InlineData(0xE000U, false)]
        [InlineData(0xD801U, true)]
        [InlineData(0xDFFEU, true)]
        public void IsSurrogateCodePoint_TestCases(uint value, bool expectedResult)
        {
            Assert.Equal(expectedResult, MediaVC.Helpers.UnicodeHelper.IsSurrogateCodePoint(value));
        }

        [Fact]
        public void UTF32Encoding_ShouldBeValid()
        {
            Assert.IsAssignableFrom<Encoding>(MediaVC.Helpers.UnicodeHelper.UTF32BigEndianEncoding);
            Assert.Equal(MediaVC.Helpers.UnicodeHelper.UTF32BigEndianCodePage, MediaVC.Helpers.UnicodeHelper.UTF32BigEndianEncoding.CodePage);
        }
    }
}
