using System;
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

        private readonly ObservableList<IFileSegmentInfo> result = new();
        private readonly ObservableList<IFileSegmentInfo> removed = new();

        #endregion

        #region Properties

        public IInputSource? CurrentVersion { get; }

        public IInputSource NewVersion { get; }

        public IObservableEnumerable<IFileSegmentInfo> Result => this.result;

        public IObservableEnumerable<IFileSegmentInfo> Removed => this.removed;

        public SynchronizationContext? SynchronizationContext { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Calculates differences between sources
        /// </summary>
        /// <param name="cancellation"></param>
        /// <exception cref="OperationCanceledException" />
        public async ValueTask CalculateAsync(CancellationToken cancellationToken = default, IProgress<float>? progress = null)
        {
            Synchronize(() =>
            {
                this.result.Clear();
                this.removed.Clear();
            });

            cancellationToken.ThrowIfCancellationRequested();

            if(CurrentVersion?.Length > 0)
                CurrentVersion.Position = 0;

            if(NewVersion.Length > 0)
                NewVersion.Position = 0;

            if(CurrentVersion?.Length > 0 && NewVersion.Length > 0)
            {
                FileSegmentInfo fileSegmentInfo = default;

                long newVersionPosition = 0, oldVersionPosition = 0;
                while(newVersionPosition < NewVersion.Length)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    NewVersion.Position = newVersionPosition;

                    Synchronize(() => progress?.Report((float)(newVersionPosition + 1) / NewVersion.Length));

                    long lastOffset = 0;
                    while(oldVersionPosition < CurrentVersion.Length)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        CurrentVersion.Position = oldVersionPosition;

                        for(long offset = 0; newVersionPosition + offset < NewVersion.Length && oldVersionPosition + offset < CurrentVersion.Length; ++offset)
                        {
                            CurrentVersion.Position = oldVersionPosition + offset;
                            NewVersion.Position = newVersionPosition + offset;

                            byte left, right;
                            left = await CurrentVersion.ReadByteAsync();

                            if(ReferenceEquals(CurrentVersion, NewVersion))
                                --CurrentVersion.Position;

                            right = await NewVersion.ReadByteAsync();

                            if(left == right)
                            {
                                if(fileSegmentInfo.Source is null)
                                {
                                    fileSegmentInfo = new FileSegmentInfo
                                    {
                                        Source = CurrentVersion,
                                        StartPositionInSource = oldVersionPosition + offset,
                                        EndPositionInSource = oldVersionPosition + offset,
                                        MappedPosition = newVersionPosition + offset
                                    };
                                }
                                else if(fileSegmentInfo.Source == CurrentVersion)
                                {
                                    if(offset <= lastOffset)
                                    {
                                        Synchronize(() => this.result.Add(fileSegmentInfo));

                                        fileSegmentInfo = default;

                                        oldVersionPosition += lastOffset + 1;
                                        newVersionPosition += lastOffset + 1;

                                        break;
                                    }

                                    fileSegmentInfo.EndPositionInSource = oldVersionPosition + offset;
                                }
                                else if(fileSegmentInfo.Source == NewVersion)
                                {
                                    Synchronize(() => this.result.Add(fileSegmentInfo));

                                    fileSegmentInfo = default;
                                    if(offset <= lastOffset)
                                    {
                                        newVersionPosition += lastOffset + 1;
                                        oldVersionPosition += lastOffset + 1;
                                    }

                                    break;
                                }
                                else
                                    throw new InvalidOperationException("Unknown source of file segment.");
                            }
                            else
                            {
                                if(fileSegmentInfo.Source is null)
                                {
                                    fileSegmentInfo = new FileSegmentInfo
                                    {
                                        Source = NewVersion,
                                        MappedPosition = newVersionPosition + offset,
                                        StartPositionInSource = newVersionPosition + offset,
                                        EndPositionInSource = newVersionPosition + offset
                                    };

                                    ++newVersionPosition;
                                    break;
                                }
                                else if(fileSegmentInfo.Source == CurrentVersion)
                                {
                                    Synchronize(() => this.result.Add(fileSegmentInfo));

                                    fileSegmentInfo = default;

                                    break;
                                }
                                else if(fileSegmentInfo.Source == NewVersion)
                                {
                                    fileSegmentInfo.EndPositionInSource = newVersionPosition + offset;
                                }
                                else
                                    throw new InvalidOperationException("Unknown source of file segment.");
                            }

                            lastOffset = offset;
                        }
                    }
                }

                if(newVersionPosition < NewVersion.Length - 1)
                {
                    Synchronize(() => this.result.Add(new FileSegmentInfo
                    {
                        Source = NewVersion,
                        MappedPosition = newVersionPosition,
                        StartPositionInSource = newVersionPosition,
                        EndPositionInSource = NewVersion.Length - 1
                    }));
                }

                Synchronize(() => RemovedSegmentsDetector.Detect(this.result, CurrentVersion, this.removed, cancellationToken));
            }
            else if((CurrentVersion?.Length ?? 0) < 1 && NewVersion.Length > 0)
            {
                Synchronize(() => progress?.Report(0));

                Synchronize(() => this.result.Add(new FileSegmentInfo
                    {
                        MappedPosition = 0,
                        Source = NewVersion,
                        StartPositionInSource = 0,
                        EndPositionInSource = NewVersion.Length - 1
                    }));

                Synchronize(() => progress?.Report(1));
            }
            else if(CurrentVersion?.Length > 0 && NewVersion.Length < 1)
            {
                Synchronize(() => progress?.Report(0));

                Synchronize(() => this.removed.Add(new FileSegmentInfo
                {
                    MappedPosition = 0,
                    Source = CurrentVersion,
                    StartPositionInSource = 0,
                    EndPositionInSource = CurrentVersion.Length - 1
                }));

                Synchronize(() => progress?.Report(1));
            }
        }

        private void Synchronize(Action workToDo)
        {
            if(workToDo != null)
            {
                if(SynchronizationContext is not null)
                    SynchronizationContext.Post(_ => workToDo(), null);
                else
                    workToDo();
            }
        }

        #endregion
    }
}
