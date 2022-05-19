using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediaVC.Core.Tests.Fixtures;
using MediaVC.Difference;

using Xunit;

namespace MediaVC.Core.Tests.Readers.StringReader
{
    public class ReadLine : IClassFixture<StringReaderFixture>
    {
        public ReadLine(StringReaderFixture fixture) => this.fixture = fixture;

        private readonly StringReaderFixture fixture;

        [Fact]
        public async Task ReadLine_CRLF()
        {
            using var source = new InputSource(this.fixture.CRLF_UTF8_Bytes);
            using var reader = new MediaVC.Readers.StringReader(source);
            var expectedLength = this.fixture.CRLF_UTF8_Bytes.Length;

            string result;
            var resultsList = new List<string>(expectedLength);

            while((result = await reader.ReadLineAsync()) != null)
                resultsList.Add(result);

            Assert.Equal(expectedLength, resultsList.Count);
            
            for(var i = 0; i < expectedLength; i++)
                Assert.Equal(this.fixture.CRLF_UTF8_Content[i], resultsList[i]);
        }

        [Fact]
        public async Task ReadLine_LF()
        {
            using var source = new InputSource(this.fixture.LF_UTF16_Bytes);
            using var reader = new MediaVC.Readers.StringReader(source);
            var expectedLength = this.fixture.LF_UTF16_Bytes.Length;

            string result;
            var resultsList = new List<string>(expectedLength);

            while((result = await reader.ReadLineAsync()) != null)
                resultsList.Add(result);

            Assert.Equal(expectedLength, resultsList.Count);

            for(var i = 0; i < expectedLength; i++)
                Assert.Equal(this.fixture.LF_UTF16_Content[i], resultsList[i]);
        }

        [Fact]
        public async Task ReadLine_CR()
        {
            using var source = new InputSource(this.fixture.CR_UTF16_Bytes);
            using var reader = new MediaVC.Readers.StringReader(source);
            var expectedLength = this.fixture.CR_UTF16_Bytes.Length;

            string result;
            var resultsList = new List<string>(expectedLength);

            while((result = await reader.ReadLineAsync()) != null)
                resultsList.Add(result);

            Assert.Equal(expectedLength, resultsList.Count);

            for(var i = 0; i < expectedLength; i++)
                Assert.Equal(this.fixture.CR_UTF16_Content[i], resultsList[i]);
        }
    }
}
