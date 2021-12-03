using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;

using MediaVC.Tools.Tests.TestData;

using Xunit;

namespace MediaVC.Tools.Tests.Checksum
{
    public class ChecksumCalculator
    {
        [Fact]
        public async void CalculateInternalAsync_WhenArgumentIsNull_ShouldThrowException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async() => await Tools.ChecksumCalculator.CalculateInternalAsync(null).CountAsync());
        }

        [Fact]
        public void CalculateInternalAsync_WhenArgumentIsEmpty_ShouldReturnEmpty()
        {
            var result = Tools.ChecksumCalculator.CalculateInternalAsync(AsyncEnumerable.Empty<ReadOnlyMemory<byte>>()).ToEnumerable();

            Assert.Empty(result);
        }

        [Fact]
        public async void CalculateInternalAsync_WhenCancellationRequested_ShouldThrowException()
        {
            var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(async() => await Tools.ChecksumCalculator.CalculateInternalAsync(AsyncEnumerable.Repeat(new ReadOnlyMemory<byte>(new byte[0]), 1), cancellation.Token).CountAsync());
        }

        [Theory]
        [ClassData(typeof(ChecksumRandomTestData))]
        public async void CalculateInternalAsync_WhenArgumentIsNonEmpty_ShouldReturnCalculated((byte[], byte[])[] data)
        {
            var result = Tools.ChecksumCalculator.CalculateInternalAsync(data.Select(row => new ReadOnlyMemory<byte>(row.Item1)).ToAsyncEnumerable());

            var expectedResults = data.Select(row => row.Item2);

            Assert.Equal(expectedResults.Count(), await result.CountAsync());

            for(var i = 0; i < expectedResults.Count(); ++i)
                Assert.Equal(expectedResults.ElementAt(i), (await result.ElementAtAsync(i)).ToArray());
        }

        [Fact]
        public async void CalculateAsync_WhenArgumentIsNull_ShouldThrowException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await Tools.ChecksumCalculator.CalculateAsync(null, 0).CountAsync());
        }

        [Fact]
        public async void CalculateAsync_WhenArgumentIsEmpty_ShouldReturnEmpty()
        {
            var result = await Tools.ChecksumCalculator.CalculateAsync(Stream.Null, 0).ToArrayAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async void CalculateAsync_WhenCancellationRequested_ShouldThrowException()
        {
            var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            var data = new byte[2];
            using var stream = new MemoryStream(data, false);

            await Assert.ThrowsAsync<TaskCanceledException>(async () => await Tools.ChecksumCalculator.CalculateAsync(stream, 2, cancellation.Token).CountAsync());
        }

        [Theory]
        [ClassData(typeof(ChecksumRandomStreamTestData))]
        public async void CalculateAsync_WhenArgumentIsNonEmpty_ShouldReturnCalculated(Stream stream, int segmentMaxLength, IAsyncEnumerable<Memory<byte>> expectedResults)
        {
            using(stream)
            {
                var results = await Tools.ChecksumCalculator.CalculateAsync(stream, segmentMaxLength).ToArrayAsync();
                
                int count;
                Assert.Equal(count = await expectedResults.CountAsync(), results.Length);

                for(var i = 0; i < count; ++i)
                    Assert.Equal((await expectedResults.ElementAtAsync(i)).ToArray(), results[i].ToArray());
            }
        }

        [Theory]
        [ClassData(typeof(ChecksumRandomMultiStreamTestData))]
        public async void CalculateAsync_WhenArgumentIsNonEmpty_VariantWithSplittedAndFullStream_ShouldCalculateEqualValues(Stream fullStream, IEnumerable<Stream> streamSegments)
        {
            using(new CompositeDisposable(
                fullStream,
                streamSegments.ElementAt(0),
                streamSegments.ElementAt(1),
                streamSegments.ElementAt(2),
                streamSegments.ElementAt(3)
            ))
            {
                var results = Tools.ChecksumCalculator.CalculateAsync(fullStream, 16).Select(item => item.ToArray());

                Assert.Equal(streamSegments.Count(), await results.CountAsync());

                for(byte i = 0; i < 4; ++i)
                {
                    var segmentToCalculate = streamSegments.ElementAt(i);
                    var calculated = await Tools.ChecksumCalculator.CalculateAsync(segmentToCalculate, (int)segmentToCalculate.Length).Select(item => item.ToArray()).FirstAsync();
                    Assert.Equal(calculated, (await results.ElementAtAsync(i)).ToArray());
                }
            }
        }
    }
}
