using System;
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

        [Fact]
        public void Read2_ShouldReadProperly()
        {
            //Assert.Null(new MediaVC.Readers.StringReader(InputSource.Empty).Read());
        }

        [Fact]
        public void Read3_ShouldReadProperly()
        {
            var text = "L*! 0|\r";

            Memory<byte> buffer = Encoding.Latin1.GetBytes(text);

            var source = new InputSource(buffer);

            var reader = new MediaVC.Readers.StringReader(source);
            reader.SelectedEncoding = Encoding.Latin1;

            Span<Rune> resultBuffer = new Rune[text.Length];
            Assert.Equal(resultBuffer.Length, reader.Read(resultBuffer));

            var result = new StringBuilder();
            foreach(var r in resultBuffer)
                result.Append(r.ToString());

            Assert.Equal(text, result.ToString());
        }
    }
}
