using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using MediaVC.Difference;
using MediaVC.Readers;

using Moq;

using Xunit;

namespace MediaVC.Core.Tests.Enumerators
{
    public class StringReaderEnumerator
    {
        [Fact]
        public void Constructor_WhenArgumentIsNull_ShouldThrow()
        {
            StringReader argument = null;

            Assert.Throws<ArgumentNullException>(() => new MediaVC.Enumerators.StringReaderEnumerator(argument));
        }

        [Fact]
        public void Constructor_WhenArgumentIsEmptyReader_ShouldNotSetPositionOfSource()
        {
            var sourceMock = new Mock<IInputSource>(MockBehavior.Strict);
            sourceMock.SetupGet(mocked => mocked.Length).Returns(0);
            sourceMock.SetupProperty(mocked => mocked.Position);

            var reader = new StringReader(sourceMock.Object);

            _ = new MediaVC.Enumerators.StringReaderEnumerator(reader);

            sourceMock.VerifyGet(mocked => mocked.Length);
            sourceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Constructor_WhenArgumentIsNonEmptyReader_ShouldResetPositionOfSource()
        {
            var sourceMock = new Mock<IInputSource>(MockBehavior.Strict);
            sourceMock.SetupGet(mocked => mocked.Length).Returns(RandomNumberGenerator.GetInt32(1, int.MaxValue));
            sourceMock.SetupProperty(mocked => mocked.Position);

            var reader = new StringReader(sourceMock.Object);

            _ = new MediaVC.Enumerators.StringReaderEnumerator(reader);

            sourceMock.VerifyGet(mocked => mocked.Length);
            sourceMock.VerifySet(mocked => mocked.Position = 0);
            sourceMock.VerifyNoOtherCalls();

            Assert.Equal(0, sourceMock.Object.Position);
        }

        [Fact]
        public async Task MoveNextAsync_ShouldWorkProperlyWithLinq()
        {
            var lines = new string[]
            {
                "A",
                "ć",
                "9",
                " %",
                string.Empty
            };

            var stringBuilder = new StringBuilder();
            Array.ForEach(lines, s => stringBuilder.AppendLine(s));

            var text = stringBuilder.ToString();

            ReadOnlyMemory<byte> utf8Bytes = Encoding.UTF8.GetBytes(text);

            using var inputSource = new InputSource(utf8Bytes);
            using var reader = new StringReader(inputSource);

            var result = await reader.Lines.ToArrayAsync();

            Assert.Equal(lines, result);
        }
    }
}
