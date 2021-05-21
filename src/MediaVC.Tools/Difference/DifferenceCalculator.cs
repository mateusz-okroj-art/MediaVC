using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediaVC.Difference;
using MediaVC.Difference.FileSegments;

namespace MediaVC.Tools.Difference
{
    public class DifferenceCalculator : IDifferenceCalculator
    {
        #region Constructor

        public DifferenceCalculator(IInputSource currentVersion, IInputSource newVersion) : this(newVersion) =>
            CurrentVersion = currentVersion ?? throw new ArgumentNullException(nameof(currentVersion));

        public DifferenceCalculator(IInputSource newVersion) =>
            NewVersion = newVersion ?? throw new ArgumentNullException(nameof(newVersion));

        #endregion

        #region Properties

        public IInputSource? CurrentVersion { get; }

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

            if(CurrentVersion is not null)
            {
                long leftPosition = 0, rightPosition = 0;
                IFileSegmentInfo? segment = null;

                while(leftPosition < CurrentVersion.Length)
                {
                    Synchronize(() => progress?.Report((float) Math.Round((double) leftPosition / CurrentVersion.Length, 2)));
                    cancellation.ThrowIfCancellationRequested();

                    // Searching
                    for(long searchingIndex = 0; LoopPredicate(searchingIndex, leftPosition, rightPosition); ++searchingIndex)
                    {
                        CurrentVersion.Position = searchingIndex + leftPosition;
                        NewVersion.Position = searchingIndex + rightPosition;

                        var searchedByte = CurrentVersion.ReadByte();

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
                    Synchronize(() =>
                        Result.Add(new FileSegmentInfo
                        {
                            Source = NewVersion,
                            StartPosition = rightPosition,
                            EndPosition = NewVersion.Length - 1
                        })
                    );
                }
            }
            else
            {
                Synchronize(() =>
                    Result.Add(new FileSegmentInfo
                    {
                        Source = NewVersion,
                        StartPosition = 0,
                        EndPosition = NewVersion.Length - 1
                    })
                );
            }
        }

        private bool LoopPredicate(long index, long leftPosition, long rightPosition) =>
            index + leftPosition < CurrentVersion?.Length &&
                        index + rightPosition < NewVersion.Length;

        private void Synchronize(Action workToDo)
        {
            if (workToDo != null)
                SynchronizationContext.Post(_ => workToDo(), null);
        }

        #endregion
    }
}
