using System.IO;
using System.Text;
using System.Threading.Tasks;

using MediaVC.Core.Tests.Fixtures;
using MediaVC.Difference;

using Moq;

using Xunit;

namespace MediaVC.Core.Tests.Readers.StringReader
{
    public class Read : IClassFixture<StringReaderFixture>
    {
        public Read(StringReaderFixture fixture) =>
            this.fixture = fixture;

        private readonly StringReaderFixture fixture;

        [Fact]
        public async Task Read1_WhenPositionIsOnEndOfStream_ShouldReturnNullAndSetError()
        {
            var inputSource = Mock.Of<IInputSource>(mock => mock.Length == 1 && mock.Position == 1);

            var reader = new MediaVC.Readers.StringReader(inputSource);

            Assert.Null(await reader.ReadAsync());

            Assert.Equal(TextReadingState.UnexpectedEndOfStream, reader.LastReadingState);
        }

        [Fact]
        public async Task Read1_ShouldReadProperly_Example1()
        {
            var source = new InputSource(this.fixture.UTF8_Bytes);
            var reader = new MediaVC.Readers.StringReader(source);

            Assert.True(new Rune(this.fixture.UTF8_Content[0]).Equals(await reader.ReadAsync()));
        }

        //[Fact]
        public async Task Read1_ShouldReadProperly_Example2()
        {
            var source = new InputSource(this.fixture.UTF8_Bytes);
            var reader = new MediaVC.Readers.StringReader(source);

            Assert.True(new Rune(this.fixture.UTF8_Content[0]).Equals(await reader.ReadAsync()));
        }
    }
}
