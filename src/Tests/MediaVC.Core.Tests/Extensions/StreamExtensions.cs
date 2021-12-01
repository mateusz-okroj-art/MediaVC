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
        public void ToMemorySegments_WhenStreamIsEmpty_ShouldReturnEmpty()
        {
            var stream = Stream.Null;

            var segments = stream.ToMemorySegments(0, CancellationToken.None);

            Assert.Empty(segments.ToEnumerable());
        }

        [Fact]
        public async void ToMemorySegments_WhenStreamIsNonEmpty_ShouldReturnSegments()
        {
            var data = new byte[]
            {
                255,255,255,0
            };

            using var stream = new MemoryStream(data, false);

            var segments = await stream.ToMemorySegments(2).ToArrayAsync();

            Assert.Equal(2, segments.Count());

            Assert.Equal(data.Take(2), segments[0].ToArray());
            Assert.Equal(data.Skip(2), segments[1].ToArray());
        }

        [Fact]
        public async void ToMemorySegments_WhenCancellationRequested_ShouldThrowIsCancelled()
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
