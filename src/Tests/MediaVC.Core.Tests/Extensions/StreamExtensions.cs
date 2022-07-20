using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace MediaVC.Core.Tests.Extensions
{
    public class StreamExtensions
    {
        [Fact]
        public async Task ToMemorySegments_WhenStreamIsEmpty_ShouldReturnEmpty()
        {
            var stream = Stream.Null;

            var segments = stream.ToMemorySegments(0, CancellationToken.None);

            Assert.False(await segments.AnyAsync());
        }

        [Fact]
        public async Task ToMemorySegments_WhenStreamIsNonEmpty_ShouldReturnSegments()
        {
            var data = new byte[]
            {
                255,255,255,0
            };

            using var stream = new MemoryStream(data, false);

            var segments = stream.ToMemorySegments(2);

            Assert.Equal(2, await segments.CountAsync());

            Assert.Equal(data.Take(2).ToArray(), (await segments.ElementAtAsync(0)).ToArray());
            Assert.Equal(data.Skip(2).ToArray(), (await segments.ElementAtAsync(1)).ToArray());
        }

        [Fact]
        public async Task ToMemorySegments_WhenCancellationRequested_ShouldThrowIsCancelled()
        {
            var data = new byte[]
            {
                255,255,255,0
            };

            using var stream = new MemoryStream(data, false);
            var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            await Assert.ThrowsAsync<TaskCanceledException>(async() => await stream.ToMemorySegments(0, cancellation.Token).AnyAsync());
        }
    }
}
