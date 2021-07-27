using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

using MediaVC.Difference;

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

        #region Fields

        private readonly ISubject<Unit> resultAdded = new Subject<Unit>();
        private readonly ISubject<Unit> resultCleared = new Subject<Unit>();

        #endregion

        #region Properties

        public IInputSource? CurrentVersion { get; }

        public IInputSource NewVersion { get; }

        public IList<IFileSegmentInfo> Result { get; } = new List<IFileSegmentInfo>();

        public IObservable<Unit> ResultAdded => this.resultAdded.AsObservable();

        public IObservable<Unit> ResultCleared => this.resultCleared.AsObservable();

        public SynchronizationContext SynchronizationContext { get; set; } = SynchronizationContext.Current;

        #endregion

        #region Methods

        public ValueTask CalculateAsync() => CalculateAsync(default, null);

        /// <summary>
        /// Calculates differences between sources
        /// </summary>
        /// <param name="cancellation"></param>
        /// <exception cref="OperationCanceledException" />
        public ValueTask CalculateAsync(CancellationToken cancellation, IProgress<float>? progress) =>
            new(Task.Run(() =>
            {
                cancellation.ThrowIfCancellationRequested();

                Synchronize(() =>
                {
                    Result.Clear();
                    this.resultCleared.OnNext(Unit.Default);
                });

                if(CurrentVersion is not null)
                {
                    long leftPosition = 0, rightPosition = 0;
                    IFileSegmentInfo? segment = null;

                    while(leftPosition < CurrentVersion.Length)
                    {
                        Synchronize(() => progress?.Report((float)Math.Round((double)leftPosition / CurrentVersion.Length, 2)));
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
                                    if(searchingIndex + rightPosition + 1 >= NewVersion.Length)
                                    {
                                        leftPosition += searchingIndex + 1;
                                        rightPosition += searchingIndex + 1;

                                        break;
                                    }

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
                                    Synchronize(() =>
                                    { 
                                        Result.Add(segment);
                                        this.resultAdded.OnNext(Unit.Default);
                                    });

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
                                    Synchronize(() =>
                                    {
                                        Result.Add(segment);
                                        this.resultAdded.OnNext(Unit.Default);
                                    });

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

                    if(rightPosition < NewVersion.Length)
                    {
                        Synchronize(() =>
                        {
                            Result.Add(new FileSegmentInfo
                            {
                                Source = NewVersion,
                                StartPosition = rightPosition,
                                EndPosition = NewVersion.Length - 1
                            });
                            this.resultAdded.OnNext(Unit.Default);
                        });
                    }
                }
                else
                {
                    Synchronize(() =>
                    {
                        Result.Add(new FileSegmentInfo
                        {
                            Source = NewVersion,
                            StartPosition = 0,
                            EndPosition = NewVersion.Length - 1
                        });
                        this.resultAdded.OnNext(Unit.Default);
                    });
                }
            }));

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
