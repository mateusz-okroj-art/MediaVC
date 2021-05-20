using System;

using Xunit;

namespace MediaVC.Tools.Tests.Detection.TextDetector
{
    public class Methods : IClassFixture<TextDetectorFixture>
    {
        #region Fields

        private readonly ITextDetectorFixture fixture;

        #endregion

        #region Constructor

        public Methods(TextDetectorFixture fixture) =>
            this.fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));

        #endregion

        #region Tests

        [Fact]
        public async void CheckIsText_ArgumentIsMemoryText1_ShouldReturnTrue()
        {
            var detector = new Tools.Detection.TextDetector(fixture.Memory_Text1);

            Assert.True(await detector.CheckIsTextAsync());
        }

        [Fact]
        public async void CheckIsText_ArgumentIsMemoryText2_ShouldReturnTrue()
        {
            var detector = new Tools.Detection.TextDetector(fixture.Memory_Text2);

            Assert.True(await detector.CheckIsTextAsync());
        }

        [Fact]
        public async void CheckIsText_ArgumentIsMemoryNonText1_ShouldReturnFalse()
        {
            var detector = new Tools.Detection.TextDetector(fixture.Memory_NonText1);

            Assert.False(await detector.CheckIsTextAsync());
        }

        [Fact]
        public async void CheckIsText_ArgumentIsMemoryNonText2_ShouldReturnFalse()
        {
            var detector = new Tools.Detection.TextDetector(fixture.Memory_NonText2);

            Assert.False(await detector.CheckIsTextAsync());
        }

        [Fact]
        public async void CheckIsText_ArgumentIsStreamText1_ShouldReturnTrue()
        {
            var detector = new Tools.Detection.TextDetector(fixture.Stream_Text1);

            Assert.True(await detector.CheckIsTextAsync());
        }

        [Fact]
        public async void CheckIsText_ArgumentIsStreamText2_ShouldReturnTrue()
        {
            var detector = new Tools.Detection.TextDetector(fixture.Stream_Text2);

            Assert.True(await detector.CheckIsTextAsync());
        }

        [Fact]
        public async void CheckIsText_ArgumentIsStreamNonText1_ShouldReturnFalse()
        {
            var detector = new Tools.Detection.TextDetector(fixture.Stream_NonText1);

            Assert.False(await detector.CheckIsTextAsync());
        }

        [Fact]
        public async void CheckIsText_ArgumentIsStreamNonText2_ShouldReturnFalse()
        {
            var detector = new Tools.Detection.TextDetector(fixture.Stream_NonText2);

            Assert.False(await detector.CheckIsTextAsync());
        }

        #endregion
    }
}
