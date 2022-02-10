using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediaVC.Core.Tests.Fixtures;

using Xunit;

namespace MediaVC.Core.Tests.Readers.StringReader
{
    public class ReadLine : IClassFixture<StringReaderFixture>
    {
        public ReadLine(StringReaderFixture fixture)
        {
            this.fixture = fixture;
        }

        private readonly StringReaderFixture fixture;

        [Fact]
        public async void ReadLine_CRLF()
        {
            
        }
    }
}
