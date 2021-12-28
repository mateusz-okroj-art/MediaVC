using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using MediaVC.Difference;

namespace MediaVC.Tools.Difference
{
    /// <summary>
    /// Detects removed segments for selected source
    /// </summary>
    public static class RemovedSegmentsDetector
    {
        public static IEnumerable<IFileSegmentInfo> Detect(IEnumerable<IFileSegmentInfo> inputSegments, IInputSource sourceForDetection, CancellationToken cancellationToken = default)
        {
            var result = new List<IFileSegmentInfo>();

            Detect(inputSegments, sourceForDetection, result, cancellationToken);

            return result.AsEnumerable();
        }

        public static void Detect(IEnumerable<IFileSegmentInfo> inputSegments, IInputSource sourceForDetection, ICollection<IFileSegmentInfo> result, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(inputSegments);
            ArgumentNullException.ThrowIfNull(sourceForDetection);
            ArgumentNullException.ThrowIfNull(result);

            var segments =
                SelectSegmentsFromSource(inputSegments, sourceForDetection)
                .ToArray();

            if(!segments.Any())
                return;

            for(var index = 0; index < segments.Length; ++index)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var currentSegment = segments[index];
                if(index == 0 &&
                    currentSegment.StartPositionInSource > 0)
                {
                    AddRemovedIntroductionSegment(sourceForDetection, result, currentSegment);
                }
                else if(index > 0 &&
                    currentSegment.StartPositionInSource - segments[index - 1].EndPositionInSource > 1)
                {
                    AddRemovedMidSegment(sourceForDetection, result, segments, index, currentSegment);
                }

                if(index == segments.Length - 1 &&
                    currentSegment.EndPositionInSource < sourceForDetection.Length - 1)
                {
                    AddRemovedEndingSegment(sourceForDetection, result, currentSegment);
                }
            }
        }

        private static IOrderedEnumerable<IFileSegmentInfo> SelectSegmentsFromSource(IEnumerable<IFileSegmentInfo> inputSegments, IInputSource sourceForDetection) =>
            from segment in inputSegments
            where segment.Source == sourceForDetection
            orderby segment.StartPositionInSource ascending
            select segment;

        private static void AddRemovedIntroductionSegment(IInputSource sourceForDetection, ICollection<IFileSegmentInfo> result, IFileSegmentInfo currentSegment) => result.Add(new FileSegmentInfo
        {
            Source = sourceForDetection,
            StartPositionInSource = 0,
            EndPositionInSource = currentSegment.StartPositionInSource - 1
        });

        private static void AddRemovedMidSegment(IInputSource sourceForDetection, ICollection<IFileSegmentInfo> result, IFileSegmentInfo[] segments, int index, IFileSegmentInfo currentSegment) => result.Add(new FileSegmentInfo
        {
            Source = sourceForDetection,
            StartPositionInSource = segments[index - 1].EndPositionInSource + 1,
            EndPositionInSource = currentSegment.StartPositionInSource - 1
        });

        private static void AddRemovedEndingSegment(IInputSource sourceForDetection, ICollection<IFileSegmentInfo> result, IFileSegmentInfo currentSegment) => result.Add(new FileSegmentInfo
        {
            Source = sourceForDetection,
            StartPositionInSource = currentSegment.EndPositionInSource + 1,
            EndPositionInSource = sourceForDetection.Length - 1
        });
    }
}
