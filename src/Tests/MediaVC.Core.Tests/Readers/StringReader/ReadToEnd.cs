using System.Text;

using MediaVC.Core.Tests.Fixtures;
using MediaVC.Difference;
using MediaVC.Helpers;

using Xunit;

namespace MediaVC.Core.Tests.Readers.StringReader
{
    public class ReadToEnd : IClassFixture<StringReaderFixture>
    {
        public ReadToEnd(StringReaderFixture fixture) =>
            this.fixture = fixture;

        private readonly StringReaderFixture fixture;

        [Fact]
        public async void ReadToEnd_UTF8()
        {
            var source = new InputSource(this.fixture.UTF8_Bytes);
            var reader = new MediaVC.Readers.StringReader(source);

            var result = await reader.ReadToEndAsync();

            Assert.Null(reader.SelectedEncoding);
            Assert.Equal(TextReadingState.Done, reader.LastReadingState);
            Assert.Equal(this.fixture.UTF8_Content, result);
        }

        [Fact]
        public async void ReadToEnd_UTF8WithBOM()
        {
            var source = new InputSource(this.fixture.UTF8BOM_Bytes);
            var reader = new MediaVC.Readers.StringReader(source);

            var result = await reader.ReadToEndAsync();

            Assert.True(ReferenceEquals(Encoding.UTF8, reader.SelectedEncoding));
            Assert.Equal(TextReadingState.Done, reader.LastReadingState);
            Assert.Equal(this.fixture.UTF8_Content, result);
        }

        [Fact]
        public async void ReadToEnd_UTF16LE()
        {
            var source = new InputSource(this.fixture.UTF16LE_Bytes);
            var reader = new MediaVC.Readers.StringReader(source);

            var result = await reader.ReadToEndAsync();

            Assert.True(ReferenceEquals(Encoding.Unicode, reader.SelectedEncoding));
            Assert.Equal(TextReadingState.Done, reader.LastReadingState);
            Assert.Equal(this.fixture.UTF16LE_Content[1..], result);
        }

        [Fact]
        public async void ReadToEnd_UTF16BE()
        {
            var source = new InputSource(this.fixture.UTF16BE_Bytes);
            var reader = new MediaVC.Readers.StringReader(source);

            var result = await reader.ReadToEndAsync();

            Assert.True(ReferenceEquals(Encoding.BigEndianUnicode, reader.SelectedEncoding));
            Assert.Equal(TextReadingState.Done, reader.LastReadingState);
            Assert.Equal(this.fixture.UTF16BE_Content[1..], result);
        }

        [Fact]
        public async void ReadToEnd_UTF32LE()
        {
            var source = new InputSource(this.fixture.UTF32LE_Bytes);
            var reader = new MediaVC.Readers.StringReader(source);

            var result = await reader.ReadToEndAsync();

            Assert.True(ReferenceEquals(Encoding.UTF32, reader.SelectedEncoding));
            Assert.Equal(TextReadingState.Done, reader.LastReadingState);
            Assert.Equal(this.fixture.UTF32LE_Content[1..], result);
        }

        [Fact]
        public async void ReadToEnd_UTF32BE()
        {
            var source = new InputSource(this.fixture.UTF32BE_Bytes);
            var reader = new MediaVC.Readers.StringReader(source);

            var result = await reader.ReadToEndAsync();

            Assert.Equal(UnicodeHelper.UTF32BigEndianCodePage, reader.SelectedEncoding?.CodePage);
            Assert.Equal(TextReadingState.Done, reader.LastReadingState);
            Assert.Equal(this.fixture.UTF32BE_Content[1..], result);
        }
    }
}
