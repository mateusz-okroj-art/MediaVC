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
        public async ValueTask CalculateAsync(CancellationToken cancellationToken = default, IDifferenceCalculatorProgress? progress = null)
        {
            try
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

                Synchronize(() => progress?.ReportProcessState(ProcessState.Started));

                if(CurrentVersion?.Length > 0 && NewVersion.Length > 0)
                {
                    await CalculateWhenBothFilesNotEmpty(progress, cancellationToken);
                }
                else if((CurrentVersion?.Length ?? 0) < 1 && NewVersion.Length > 0)
                {
                    AddSegmentForFullNewFile(progress);
                }
                else if(CurrentVersion?.Length > 0 && NewVersion.Length < 1)
                {
                    AddSegmentAsRemoved(progress);
                }

                Synchronize(() => progress?.ReportProcessState(ProcessState.Completed));
            }
            catch(OperationCanceledException ex)
            {
                Synchronize(() => progress?.ReportProcessState(ProcessState.Cancelled));

                throw ex;
            }
        }

        /// <summary>
        /// Adds segment for full CurrentVersion to removed.
        /// </summary>
        /// <param name="progress"></param>
        private void AddSegmentAsRemoved(IDifferenceCalculatorProgress? progress)
        {
            Synchronize(() => progress?.ReportLeftMainPosition(0));

            Synchronize(() => this.removed.Add(new FileSegmentInfo
            {
                MappedPosition = 0,
                Source = CurrentVersion!,
                StartPositionInSource = 0,
                EndPositionInSource = CurrentVersion!.Length - 1
            }));

            if(CurrentVersion is not null)
                Synchronize(() => progress?.ReportLeftMainPosition(CurrentVersion.Length - 1));
        }

        /// <summary>
        /// Adds segment with full NewVersion to result when CurrentVersion is empty
        /// </summary>
        /// <param name="progress"></param>
        private void AddSegmentForFullNewFile(IDifferenceCalculatorProgress? progress)
        {
            Synchronize(() => progress?.ReportRightMainPosition(0));

            Synchronize(() => this.result.Add(new FileSegmentInfo
            {
                MappedPosition = 0,
                Source = NewVersion,
                StartPositionInSource = 0,
                EndPositionInSource = NewVersion.Length - 1
            }));

            Synchronize(() => progress?.ReportRightMainPosition(NewVersion.Length - 1));
        }

        /// <summary>
        /// Calculates difference when two versions are not empty
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        private async Task CalculateWhenBothFilesNotEmpty(IDifferenceCalculatorProgress? progress, CancellationToken cancellationToken)
        {
            var fileSegmentInfo = new FileSegmentInfo();

            Synchronize(() =>
            {
                progress?.ReportLeftMainPosition(0);
                progress?.ReportRightMainPosition(0);
            });

            long newVersionPosition = 0, oldVersionPosition = 0;
            while(newVersionPosition < NewVersion.Length)
            {
                cancellationToken.ThrowIfCancellationRequested();

                NewVersion.Position = newVersionPosition;

                Synchronize(() => progress?.ReportRightMainPosition(newVersionPosition));

                long lastOffset = 0;
                while(oldVersionPosition < CurrentVersion!.Length)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    CurrentVersion.Position = oldVersionPosition;

                    Synchronize(() => progress?.ReportLeftMainPosition(oldVersionPosition));

                    for(long offset = 0; newVersionPosition + offset < NewVersion.Length && oldVersionPosition + offset < CurrentVersion.Length; ++offset)
                    {
                        CurrentVersion.Position = AdjustCurrentVersionPosition(fileSegmentInfo, offset, oldVersionPosition);

                        Synchronize(() =>
                        {
                            progress?.ReportLeftOffsetedPosition(CurrentVersion.Position);
                            progress?.ReportRightOffsetedPosition(newVersionPosition + offset);
                        });

                        NewVersion.Position = newVersionPosition + offset;

                        byte left, right;
                        left = await CurrentVersion.ReadByteAsync(cancellationToken);

                        if(ReferenceEquals(CurrentVersion, NewVersion))
                            --CurrentVersion.Position;

                        right = await NewVersion.ReadByteAsync(cancellationToken);

                        var completed =
                            left == right
                            ? CalculateWhenBytesAreEquals(ref fileSegmentInfo, ref newVersionPosition, ref oldVersionPosition, lastOffset, offset)
                            : CalculateWhenBytesAreDifferent(ref fileSegmentInfo, ref newVersionPosition, ref oldVersionPosition, lastOffset, offset);

                        if(!completed)
                            break;

                        lastOffset = offset;
                    }

                    if(newVersionPosition >= NewVersion.Length)
                        break;
                }

                if(oldVersionPosition >= CurrentVersion.Length)
                    break;
            }

            AddSegmentForNewVersionEnding(newVersionPosition);

            Synchronize(() => RemovedSegmentsDetector.Detect(this.result, CurrentVersion!, this.removed, cancellationToken));
        }

        private bool CalculateWhenBytesAreDifferent(ref FileSegmentInfo fileSegmentInfo, ref long newVersionPosition, ref long oldVersionPosition, long lastOffset, long offset)
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
            }
            else if(ReferenceEquals(fileSegmentInfo.Source, CurrentVersion))
            {
                SaveSegmentAndClear(ref fileSegmentInfo, ref newVersionPosition, lastOffset);
                oldVersionPosition += lastOffset + 1;

                return false;
            }
            else if(ReferenceEquals(fileSegmentInfo.Source, NewVersion))
            {
                if(SaveCurrentSegmentAndClear(ref fileSegmentInfo, ref newVersionPosition, lastOffset, offset))
                    return false;

                fileSegmentInfo.EndPositionInSource = newVersionPosition + offset;
            }
            else
            {
                throw new InvalidOperationException("Unknown source of file segment.");
            }

            return true;
        }

        private bool CalculateWhenBytesAreEquals(ref FileSegmentInfo fileSegmentInfo, ref long newVersionPosition, ref long oldVersionPosition, long lastOffset, long offset)
        {
            if(fileSegmentInfo.Source is null)
            {
                fileSegmentInfo = new FileSegmentInfo
                {
                    Source = CurrentVersion!,
                    StartPositionInSource = oldVersionPosition + offset,
                    EndPositionInSource = oldVersionPosition + offset,
                    MappedPosition = newVersionPosition + offset
                };
            }
            else if(ReferenceEquals(fileSegmentInfo.Source, CurrentVersion))
            {
                if(offset <= lastOffset)
                {
                    SaveSegmentAndClear(ref fileSegmentInfo, ref newVersionPosition, lastOffset);
                    oldVersionPosition += lastOffset + 1;

                    return false;
                }

                fileSegmentInfo.EndPositionInSource = oldVersionPosition + offset;
            }
            else if(ReferenceEquals(fileSegmentInfo.Source, NewVersion))
            {
                SaveSegmentForCurrentVersionAndClear(ref fileSegmentInfo, ref newVersionPosition, ref oldVersionPosition, lastOffset, offset);
                return false;
            }
            else
            {
                throw new InvalidOperationException("Unknown source of file segment.");
            }

            return true;
        }

        /// <summary>
        /// Adds last segment when NewVersion is not on end.
        /// </summary>
        /// <param name="newVersionPosition"></param>
        private void AddSegmentForNewVersionEnding(long newVersionPosition)
        {
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
        }

        private bool SaveCurrentSegmentAndClear(ref FileSegmentInfo fileSegmentInfo, ref long newVersionPosition, long lastOffset, long offset)
        {
            if(offset <= lastOffset)
            {
                SaveSegmentAndClear(ref fileSegmentInfo, ref newVersionPosition, lastOffset);
                return true;
            }

            return false;
        }

        private void SaveSegmentAndClear(ref FileSegmentInfo fileSegmentInfo, ref long newVersionPosition, long lastOffset)
        {
            var currentSegment = fileSegmentInfo;
            Synchronize(() => this.result.Add(currentSegment));

            fileSegmentInfo = new FileSegmentInfo();
            
            newVersionPosition += lastOffset + 1;
        }

        private void SaveSegmentForCurrentVersionAndClear(ref FileSegmentInfo fileSegmentInfo, ref long newVersionPosition, ref long oldVersionPosition, long lastOffset, long offset)
        {
            var currentSegment = fileSegmentInfo;
            Synchronize(() => this.result.Add(currentSegment));

            fileSegmentInfo = new FileSegmentInfo();
            if(offset <= lastOffset)
            {
                newVersionPosition += lastOffset + 1;
                oldVersionPosition += lastOffset + 1;
            }
            else if(CurrentVersion!.Position == oldVersionPosition + 1)
            {
                newVersionPosition += lastOffset + 1;
            }
        }

        /// <summary>
        /// When current segment source is not CurrentVersion, then CurrentVersion should not be moved by offset.
        /// </summary>
        /// <returns>Adjusted stream position</returns>
        private long AdjustCurrentVersionPosition(IFileSegmentInfo fileSegmentInfo, long offset, long oldVersionPosition) =>
            ReferenceEquals(fileSegmentInfo.Source, CurrentVersion) ?
            oldVersionPosition + offset :
            oldVersionPosition;

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
