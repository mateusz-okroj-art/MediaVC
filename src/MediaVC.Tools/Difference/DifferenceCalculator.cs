using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediaVC.Difference;
using MediaVC.Difference.FileSegments;

namespace MediaVC.Tools.Difference
{
    public class DifferenceCalculator : IDifferenceCalculator
    {
        #region Constructor

        public DifferenceCalculator(IInputSource currentVersion, IInputSource newVersion) : this(newVersion)
        {
            CurrentVersion = currentVersion ?? throw new ArgumentNullException(nameof(currentVersion));
        }

        public DifferenceCalculator(IInputSource newVersion)
        {
            NewVersion = newVersion ?? throw new ArgumentNullException(nameof(newVersion));
        }

        #endregion

        #region Properties

        public IInputSource CurrentVersion { get; }

        public IInputSource NewVersion { get; }

        public IList<IFileSegmentInfo> Result { get; } = new List<IFileSegmentInfo>();

        public SynchronizationContext SynchronizationContext { get; set; } = SynchronizationContext.Current;

        #endregion

        #region Methods

        public ValueTask CalculateAsync() => CalculateAsync(default, null);

        /// <summary>
        /// Calculates differences between sources
        /// </summary>
        /// <param name="cancellation"></param>
        /// <exception cref="OperationCanceledException" />
        public async ValueTask CalculateAsync(CancellationToken cancellation, IProgress<float>? progress)
        {
            cancellation.ThrowIfCancellationRequested();

            Synchronize(() => Result.Clear());

            if(CurrentVersion != null)
            {
                long leftPosition = 0, rightPosition = 0;
                IFileSegmentInfo segment = null;

                while(leftPosition < CurrentVersion.Length)
                {
                    Synchronize(() => progress?.Report((float) Math.Round((double) leftPosition / CurrentVersion.Length, 2)));
                    cancellation.ThrowIfCancellationRequested();

                    Func<long, bool> loopPredicate = index =>
                        index + leftPosition < CurrentVersion.Length &&
                        index + rightPosition < NewVersion.Length;

                    // Searching
                    for(long searchingIndex = 0; loopPredicate(searchingIndex); ++searchingIndex)
                    {
                        CurrentVersion.Position = searchingIndex + leftPosition;
                        NewVersion.Position = searchingIndex + rightPosition;

                        byte searchedByte = CurrentVersion.ReadByte();

                        if(searchedByte == NewVersion.ReadByte())
                        {
                            if(segment == null)
                            {
                                segment = new FileSegmentInfo
                                {
                                    Source = CurrentVersion,
                                    StartPosition = leftPosition + searchingIndex,
                                    EndPosition = leftPosition + searchingIndex
                                };
                            }
                            else if(segment.Source.Equals(CurrentVersion))
                            {
                                segment.EndPosition = leftPosition + searchingIndex;
                            }
                            else if(segment.Source.Equals(NewVersion))
                            {
                                Synchronize(() => Result.Add(segment));

                                segment = null;

                                leftPosition += searchingIndex;
                                rightPosition += searchingIndex;

                                break;
                            }
                        }
                        else
                        {
                            if(segment == null)
                            {
                                segment = new FileSegmentInfo
                                {
                                    Source = NewVersion,
                                    StartPosition = rightPosition + searchingIndex,
                                    EndPosition = rightPosition + searchingIndex
                                };
                            }
                            else if(segment.Source.Equals(CurrentVersion))
                            {
                                Synchronize(() => Result.Add(segment));

                                segment = null;

                                leftPosition += searchingIndex;
                                rightPosition += searchingIndex;

                                break;
                            }
                            else if(segment.Source.Equals(NewVersion))
                            {
                                segment.EndPosition = rightPosition + searchingIndex;
                            }
                        }
                    }
                }

                /*var query = Result.Where(segment => segment.Source.Equals(NewVersion))
                        .OrderBy(segment => segment.EndPosition)
                        .Select(segment => segment.EndPosition);

                if (query.Any())
                    rightPosition = query.Last() + 1;*/

                if (rightPosition < NewVersion.Length)
                {
                    Result.Add(new FileSegmentInfo
                    {
                        Source = NewVersion,
                        StartPosition = rightPosition,
                        EndPosition = NewVersion.Length - 1
                    });
                }
            }
            else
            {
                Result.Add(new FileSegmentInfo
                {
                    Source = NewVersion,
                    StartPosition = 0,
                    EndPosition = NewVersion.Length - 1
                });
            }
        }

        private void Synchronize(Action workToDo)
        {
            if (workToDo != null)
                SynchronizationContext.Post(_ => workToDo(), null);
        }

        /// <summary>
        /// Calculates removed segments between calculated difference and selected source from calculation.
        /// </summary>
        /// <param name="calculatedDifference">Calculated segments</param>
        /// <param name="sourceToCalculate">Selected source used in calculation</param>
        /// <returns>Removed segments from source</returns>
        public static IEnumerable<IFileSegmentInfo> CalculateRemovedSegments(IEnumerable<IFileSegmentInfo> calculatedDifference, IInputSource sourceToCalculate)
        {
            if (calculatedDifference == null)
                throw new ArgumentNullException(nameof(calculatedDifference));

            if (sourceToCalculate == null)
                throw new ArgumentNullException(nameof(sourceToCalculate));

            var query = calculatedDifference.Where(segment => segment.Source.Equals(sourceToCalculate))
                .OrderBy(segment => segment.StartPosition);

            if(query.Any())
            {
                long lastEndIndex = 0;
                var segments = query.ToArray();

                for(int index = 0; index < segments.Length; ++index)
                {
                    if(segments[index].StartPosition - lastEndIndex > 0)
                    {
                        yield return new FileSegmentInfo
                        {
                            Source = sourceToCalculate,
                            StartPosition = lastEndIndex,
                            EndPosition = segments[index].StartPosition - 1
                        };
                    }

                    lastEndIndex = segments[index].EndPosition + 1;
                }

                if(sourceToCalculate.Length - lastEndIndex > 0)
                {
                    yield return new FileSegmentInfo
                    {
                        Source = sourceToCalculate,
                        StartPosition = lastEndIndex,
                        EndPosition = sourceToCalculate.Length - 1
                    };
                }
            }
            else
            {
                yield return new FileSegmentInfo
                {
                    Source = sourceToCalculate,
                    StartPosition = 0,
                    EndPosition = sourceToCalculate.Length - 1
                };
            }    
        }

        #endregion
    }
}
