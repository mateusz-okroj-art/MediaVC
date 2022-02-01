using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediaVC.Core.Tests.Fixtures;
using MediaVC.Difference;

using Xunit;

namespace MediaVC.Core.Tests.Readers
{
    public class StringReader : IClassFixture<StringReaderFixture>
    {
        public StringReader(StringReaderFixture fixture) =>
            this.fixture = fixture;

        private readonly StringReaderFixture fixture;

        public async void ReadToEnd_Utf8()
        {
            var source = new InputSource(this.fixture.UTF8_Bytes);
            var reader = new MediaVC.Readers.StringReader(source);

            var result = await reader.ReadToEndAsync();

            Assert.Null(reader.SelectedEncoding);
            Assert.Equal(TextReadingState.Done, reader.LastReadingState);
            Assert.Equal(this.fixture.UTF8_Content, result);
        }
    }
}
