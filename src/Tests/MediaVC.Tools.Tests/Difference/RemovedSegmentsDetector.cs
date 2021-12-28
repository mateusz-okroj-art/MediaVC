using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MediaVC.Difference;

using Moq;

using Xunit;

namespace MediaVC.Tools.Tests.Difference
{
    public class RemovedSegmentsDetector
    {
        [Fact]
        public void Detect1_WhenArgumentIsNull_ShouldThrowException()
        {
            Assert.Throws<ArgumentNullException>(() => Tools.Difference.RemovedSegmentsDetector.Detect(null, InputSource.Empty));
            Assert.Throws<ArgumentNullException>(() => Tools.Difference.RemovedSegmentsDetector.Detect(Enumerable.Empty<IFileSegmentInfo>(), null));
        }

        [Fact]
        public void Detect2_WhenArgumentIsNull_ShouldThrowException()
        {
            Assert.Throws<ArgumentNullException>(() => Tools.Difference.RemovedSegmentsDetector.Detect(null, InputSource.Empty, new List<IFileSegmentInfo>()));
            Assert.Throws<ArgumentNullException>(() => Tools.Difference.RemovedSegmentsDetector.Detect(Enumerable.Empty<IFileSegmentInfo>(), null, new List<IFileSegmentInfo>()));
            Assert.Throws<ArgumentNullException>(() => Tools.Difference.RemovedSegmentsDetector.Detect(Enumerable.Empty<IFileSegmentInfo>(), InputSource.Empty, null));
        }

        [Fact]
        public void Detect_WhenFullSourceIsInSegments_ShouldReturnEmpty()
        {
            var inputSource = Mock.Of<IInputSource>();

            var segments = new IFileSegmentInfo[]
            {
                new FileSegmentInfo
                {
                    Source = inputSource,
                    MappedPosition = 0,
                    StartPositionInSource = 0,
                    EndPositionInSource = int.MaxValue
                }
            };

            var cancellation = new CancellationTokenSource();

            if(!Debugger.IsAttached)
                cancellation.CancelAfter(TimeSpan.FromSeconds(10));

            var result = Tools.Difference.RemovedSegmentsDetector.Detect(segments, inputSource, cancellation.Token);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void Detect_WhenFullSourceIsNotInSegments_ShouldReturnSingleWithRemovedSource()
        {
            var selectedInputSource = Mock.Of<IInputSource>();
            var otherInputSource = Mock.Of<IInputSource>();

            var segments = new IFileSegmentInfo[]
            {
                new FileSegmentInfo
                {
                    Source = otherInputSource,
                    MappedPosition = 0,
                    StartPositionInSource = 0,
                    EndPositionInSource = 1
                }
            };

            var cancellation = new CancellationTokenSource();

            if(!Debugger.IsAttached)
                cancellation.CancelAfter(TimeSpan.FromSeconds(10));

            var results = Tools.Difference.RemovedSegmentsDetector.Detect(segments, selectedInputSource, cancellation.Token);

            Assert.NotNull(results);
            var result = Assert.Single(results);

            Assert.True(ReferenceEquals(selectedInputSource, result.Source));
            Assert.Equal(0, result.StartPositionInSource);
            Assert.Equal(selectedInputSource.Length - 1, result.EndPositionInSource);
        }

        [Fact]
        public void Detect_WhenIsMixed_ShouldReturnManySegments()
        {
            var selectedInputSource = Mock.Of<IInputSource>(mock => mock.Length == 4L);
            var otherInputSource = Mock.Of<IInputSource>(mock => mock.Length == 4L);

            var segments = new IFileSegmentInfo[]
            {
                new FileSegmentInfo
                {
                    Source = otherInputSource,
                    MappedPosition = 0,
                    StartPositionInSource = 0,
                    EndPositionInSource = 1
                },
                new FileSegmentInfo
                {
                    Source = selectedInputSource,
                    MappedPosition = 2,
                    StartPositionInSource = 1,
                    EndPositionInSource = 1
                },
                new FileSegmentInfo
                {
                    Source = otherInputSource,
                    MappedPosition = 3,
                    StartPositionInSource = 2,
                    EndPositionInSource = 3
                }
            };

            var cancellation = new CancellationTokenSource();

            if(!Debugger.IsAttached)
                cancellation.CancelAfter(TimeSpan.FromSeconds(10));

            var results = Tools.Difference.RemovedSegmentsDetector.Detect(segments, selectedInputSource, cancellation.Token).ToArray();

            Assert.NotNull(results);
            Assert.Equal(2, results.Length);

            var result = results[0];
            Assert.True(ReferenceEquals(selectedInputSource, result.Source));
            Assert.Equal(0, result.StartPositionInSource);
            Assert.Equal(0, result.EndPositionInSource);

            result = results[1];
            Assert.True(ReferenceEquals(selectedInputSource, result.Source));
            Assert.Equal(2, result.StartPositionInSource);
            Assert.Equal(3, result.EndPositionInSource);
        }
    }
}
