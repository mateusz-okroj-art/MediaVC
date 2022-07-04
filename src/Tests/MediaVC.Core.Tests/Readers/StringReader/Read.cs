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

        [Theory]
        [InlineData(0, 0, -1)]
        [InlineData(1, 1, -1)]
        [InlineData(2, 2, -1)]
        [InlineData(10, 7, 8)]
        public async Task Read1_ShouldReadProperly_UTF8(int sourceIndex, int contentIndex0, int contentIndex1)
        {
            var source = new InputSource(this.fixture.UTF8_Bytes);
            var reader = new MediaVC.Readers.StringReader(source);

            source.Position = sourceIndex;
            var result = await reader.ReadAsync();

            var rune = contentIndex1 > -1
                ? new Rune(this.fixture.UTF8_Content[contentIndex0], this.fixture.UTF8_Content[contentIndex1])
                : new Rune(this.fixture.UTF8_Content[contentIndex0]);

            Assert.True(rune.Equals(result));
        }

        [Theory]
        [InlineData(2, 0, -1)]
        [InlineData(4, 1, -1)]
        [InlineData(6, 2, -1)]
        [InlineData(16, 7, 8)]
        public async Task Read1_ShouldReadProperly_UTF16(int sourceIndex, int contentIndex0, int contentIndex1)
        {
            var source = new InputSource(this.fixture.UTF16LE_Bytes);
            var reader = new MediaVC.Readers.StringReader(source);

            source.Position = sourceIndex;
            var result = await reader.ReadAsync();

            var rune = contentIndex1 > -1
                ? new Rune(this.fixture.UTF8_Content[contentIndex0], this.fixture.UTF8_Content[contentIndex1])
                : new Rune(this.fixture.UTF8_Content[contentIndex0]);

            Assert.True(rune.Equals(result));
        }
    }
}
