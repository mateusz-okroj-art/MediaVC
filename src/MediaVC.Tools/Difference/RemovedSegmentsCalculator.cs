using System;
using System.Collections.Generic;
using System.Linq;

using MediaVC.Difference;
using MediaVC.Difference.FileSegments;

namespace MediaVC.Tools.Difference
{
    public class RemovedSegmentsCalculator
    {
        #region Constructor

        public RemovedSegmentsCalculator(IEnumerable<IFileSegmentInfo> segmentsToCalculate, IInputSource sourceToCalculate)
        {
            Segments = segmentsToCalculate ?? throw new ArgumentNullException(nameof(segmentsToCalculate));

            Source = sourceToCalculate ?? throw new ArgumentNullException(nameof(sourceToCalculate));
        }

        #endregion

        #region Properties

        public IEnumerable<IFileSegmentInfo> Segments { get; }
        
        public IInputSource Source { get; }

        public IList<IFileSegmentInfo> Result { get; } = new List<IFileSegmentInfo>();

        #endregion

        /// <summary>
        /// Calculates removed segments between calculated difference and selected source from calculation.
        /// </summary>
        /// <param name="calculatedDifference">Calculated segments</param>
        /// <param name="sourceToCalculate">Selected source used in calculation</param>
        /// <returns>Removed segments from source</returns>
        public void Calculate()
        {
            var query = Segments.Where(segment => segment.Source.Equals(Source))
                .OrderBy(segment => segment.StartPosition);

            if(query.Any())
            {
                long lastEndIndex = 0;
                var segments = query.ToArray();

                foreach(var segment in segments)
                {
                    if(segment.StartPosition - lastEndIndex > 0)
                    {
                        Result.Add(new FileSegmentInfo
                        {
                            Source = Source,
                            StartPosition = lastEndIndex,
                            EndPosition = segment.StartPosition - 1
                        });
                    }

                    lastEndIndex = segment.EndPosition + 1;
                }

                if(Source.Length - lastEndIndex > 0)
                {
                    Result.Add(new FileSegmentInfo
                    {
                        Source = Source,
                        StartPosition = lastEndIndex,
                        EndPosition = Source.Length - 1
                    });
                }
            }
            else
            {
                Result.Add(new FileSegmentInfo
                {
                    Source = Source,
                    StartPosition = 0,
                    EndPosition = Source.Length - 1
                });
            }
        }
    }
}
