using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using MediaVC.Tools.Tests.Fixtures;

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
        public async Task CheckIsText_ArgumentIsMemoryText1_ShouldReturnTrue()
        {
            var detector = new Tools.Detection.TextDetector(fixture.Memory_Text1);
            var cancellationTokenSource = new CancellationTokenSource();

            if(!Debugger.IsAttached)
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            Assert.True(await detector.CheckIsTextAsync(cancellationTokenSource.Token));
        }

        [Fact]
        public async Task CheckIsText_ArgumentIsMemoryText2_ShouldReturnTrue()
        {
            var detector = new Tools.Detection.TextDetector(fixture.Memory_Text2);
            var cancellationTokenSource = new CancellationTokenSource();

            if(!Debugger.IsAttached)
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            Assert.True(await detector.CheckIsTextAsync(cancellationTokenSource.Token));
        }

        [Fact]
        public async Task CheckIsText_ArgumentIsMemoryNonText1_ShouldReturnFalse()
        {
            var detector = new Tools.Detection.TextDetector(fixture.Memory_NonText1);
            var cancellationTokenSource = new CancellationTokenSource();

            if(!Debugger.IsAttached)
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            Assert.False(await detector.CheckIsTextAsync(cancellationTokenSource.Token));
        }

        [Fact]
        public async Task CheckIsText_ArgumentIsMemoryNonText2_ShouldReturnFalse()
        {
            var detector = new Tools.Detection.TextDetector(fixture.Memory_NonText2);
            var cancellationTokenSource = new CancellationTokenSource();

            if(!Debugger.IsAttached)
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            Assert.False(await detector.CheckIsTextAsync(cancellationTokenSource.Token));
        }

        [Fact]
        public async Task CheckIsText_ArgumentIsStreamText1_ShouldReturnTrue()
        {
            var detector = new Tools.Detection.TextDetector(fixture.Stream_Text1);
            var cancellationTokenSource = new CancellationTokenSource();

            if(!Debugger.IsAttached)
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            Assert.True(await detector.CheckIsTextAsync(cancellationTokenSource.Token));
        }

        [Fact]
        public async Task CheckIsText_ArgumentIsStreamText2_ShouldReturnTrue()
        {
            var detector = new Tools.Detection.TextDetector(fixture.Stream_Text2);
            var cancellationTokenSource = new CancellationTokenSource();

            if(!Debugger.IsAttached)
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            Assert.True(await detector.CheckIsTextAsync(cancellationTokenSource.Token));
        }

        [Fact]
        public async Task CheckIsText_ArgumentIsStreamNonText1_ShouldReturnFalse()
        {
            var detector = new Tools.Detection.TextDetector(fixture.Stream_NonText1);
            var cancellationTokenSource = new CancellationTokenSource();

            if(!Debugger.IsAttached)
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            Assert.False(await detector.CheckIsTextAsync(cancellationTokenSource.Token));
        }

        [Fact]
        public async Task CheckIsText_ArgumentIsStreamNonText2_ShouldReturnFalse()
        {
            var detector = new Tools.Detection.TextDetector(fixture.Stream_NonText2);
            var cancellationTokenSource = new CancellationTokenSource();

            if(!Debugger.IsAttached)
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            Assert.False(await detector.CheckIsTextAsync(cancellationTokenSource.Token));
        }

        #endregion
    }
}
