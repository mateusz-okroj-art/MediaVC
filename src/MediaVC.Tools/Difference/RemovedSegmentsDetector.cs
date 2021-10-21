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

            Detect(inputSegments, sourceForDetection, result);

            return result.AsEnumerable();
        }

        public static void Detect(IEnumerable<IFileSegmentInfo> inputSegments, IInputSource sourceForDetection, ICollection<IFileSegmentInfo> result, CancellationToken cancellationToken = default)
        {
            if(inputSegments is null)
                throw new ArgumentNullException(nameof(inputSegments));

            if(sourceForDetection is null)
                throw new ArgumentNullException(nameof(sourceForDetection));

            if(sourceForDetection is null)
                throw new ArgumentNullException(nameof(result));

            var segmentsFromSelectedSource =
                from segment in inputSegments
                where segment.Source == sourceForDetection
                orderby segment.StartPositionInSource ascending
                select segment;

            if(!segmentsFromSelectedSource.Any())
                return;
            var segments = segmentsFromSelectedSource.ToArray(); 
            for(var index = 0; index < segments.Length; ++index)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var currentSegment = segments[index];
                if(index == 0 && currentSegment.StartPositionInSource > 0)
                {
                    result.Add(new FileSegmentInfo
                    {
                        Source = sourceForDetection,
                        StartPositionInSource = 0,
                        EndPositionInSource = currentSegment.StartPositionInSource - 1
                    });
                }
                else
                {
                    result.Add(new FileSegmentInfo
                    {
                        Source = sourceForDetection,
                        StartPositionInSource = segments[index - 1].EndPositionInSource + 1,
                        EndPositionInSource = currentSegment.StartPositionInSource - 1
                    });
                }

                if(index == segments.Length - 1 && currentSegment.EndPositionInSource < sourceForDetection.Length - 1)
                {
                    result.Add(new FileSegmentInfo
                    {
                        Source = sourceForDetection,
                        StartPositionInSource = currentSegment.EndPositionInSource + 1,
                        EndPositionInSource = sourceForDetection.Length - 1
                    });
                }
            }
        }
    }
}
