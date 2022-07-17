using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        public async Task ReadToEnd_WhenCanceled_ShouldThrow()
        {
            using var source = new InputSource(this.fixture.UTF8_Bytes);
            using var reader = new MediaVC.Readers.StringReader(source);

            var cancelSource = new CancellationTokenSource();
            cancelSource.CancelAfter(20);

            await Task.Delay(21);

            await Assert.ThrowsAsync<OperationCanceledException>(async () => await reader.ReadToEndAsync(cancelSource.Token));
        }

        [Fact]
        public async Task ReadToEnd_UTF8()
        {
            using var source = new InputSource(this.fixture.UTF8_Bytes);
            using var reader = new MediaVC.Readers.StringReader(source);

            var result = await reader.ReadToEndAsync();

            Assert.Null(reader.SelectedEncoding);
            Assert.Equal(TextReadingState.Done, reader.LastReadingState);
            Assert.Equal(this.fixture.UTF8_Content, result);
        }

        [Fact]
        public async Task ReadToEnd_UTF8WithBOM()
        {
            using var source = new InputSource(this.fixture.UTF8BOM_Bytes);
            using var reader = new MediaVC.Readers.StringReader(source);

            var result = await reader.ReadToEndAsync();

            Assert.True(ReferenceEquals(Encoding.UTF8, reader.SelectedEncoding));
            Assert.Equal(TextReadingState.Done, reader.LastReadingState);
            Assert.Equal(this.fixture.UTF8_Content, result);
        }

        [Fact]
        public async Task ReadToEnd_UTF16LE()
        {
            using var source = new InputSource(this.fixture.UTF16LE_Bytes);
            using var reader = new MediaVC.Readers.StringReader(source);

            var result = await reader.ReadToEndAsync();

            Assert.True(ReferenceEquals(Encoding.Unicode, reader.SelectedEncoding));
            Assert.Equal(TextReadingState.Done, reader.LastReadingState);
            Assert.Equal(this.fixture.UTF16LE_Content, result);
        }

        [Fact]
        public async Task ReadToEnd_UTF16BE()
        {
            using var source = new InputSource(this.fixture.UTF16BE_Bytes);
            using var reader = new MediaVC.Readers.StringReader(source);

            var result = await reader.ReadToEndAsync();

            Assert.True(ReferenceEquals(Encoding.BigEndianUnicode, reader.SelectedEncoding));
            Assert.Equal(TextReadingState.Done, reader.LastReadingState);
            Assert.Equal(this.fixture.UTF16BE_Content, result);
        }

        [Fact]
        public async Task ReadToEnd_UTF32LE()
        {
            using var source = new InputSource(this.fixture.UTF32LE_Bytes);
            using var reader = new MediaVC.Readers.StringReader(source);

            var result = await reader.ReadToEndAsync();

            Assert.True(ReferenceEquals(Encoding.UTF32, reader.SelectedEncoding));
            Assert.Equal(TextReadingState.Done, reader.LastReadingState);
            Assert.Equal(this.fixture.UTF32LE_Content, result);
        }

        [Fact]
        public async Task ReadToEnd_UTF32BE()
        {
            using var source = new InputSource(this.fixture.UTF32BE_Bytes);
            using var reader = new MediaVC.Readers.StringReader(source);

            var result = await reader.ReadToEndAsync();

            Assert.Equal(UnicodeHelper.UTF32BigEndianCodePage, reader.SelectedEncoding?.CodePage);
            Assert.Equal(TextReadingState.Done, reader.LastReadingState);
            Assert.Equal(this.fixture.UTF32BE_Content, result);
        }

        [Fact]
        public async Task ReadToEnd_CRLF_UTF8()
        {
            using var source = new InputSource(this.fixture.CRLF_UTF8_Bytes);
            using var reader = new MediaVC.Readers.StringReader(source);

            var result = await reader.ReadToEndAsync();

            var expected = this.fixture.CRLF_UTF8_Content.Aggregate((currentResult, next) => currentResult + "\r\n" + next);
            Assert.Equal(expected, result);
        }
    }
}
