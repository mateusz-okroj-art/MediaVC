using System;
using System.Text;
using System.Threading.Tasks;

using MediaVC.Core.Tests.Fixtures;
using MediaVC.Difference;

using Xunit;

namespace MediaVC.Core.Tests.Helpers
{
    public class TextReadingEngine : IClassFixture<StringReaderFixture>
    {
        public TextReadingEngine(StringReaderFixture fixture) => this.fixture = fixture;

        private readonly StringReaderFixture fixture;

        [Fact]
        public void Constructor_WhenArgumentIsNull_ShouldThrow()
        {
            IInputSource argument = null;

            Assert.Throws<ArgumentNullException>(() => new MediaVC.Helpers.TextReadingEngine(argument));
        }

        [Fact]
        public async Task ReadAsync_WhenSelectedEncodingIsCustom_ShouldDecodeOnEncodingObject()
        {
            var text = "L*! 0|\r";

            Memory<byte> buffer = Encoding.Latin1.GetBytes(text);

            var source = new InputSource(buffer);

            var reader = new MediaVC.Helpers.TextReadingEngine(source);
            reader.SelectedEncoding = Encoding.Latin1;

            StringBuilder result = new();

            Rune? rune;
            while((rune = await reader.ReadAsync()).HasValue)
            {
                result.Append(rune.Value.ToString());
            }

            Assert.Equal(text, result.ToString());
        }
    }
}
