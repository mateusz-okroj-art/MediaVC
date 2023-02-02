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

        private FileSegmentInfo _fileSegmentInfo = default;
        private long _oldVersionPosition = 0;
        private long _newVersionPosition = 0;

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
        public async ValueTask CalculateAsync(IDifferenceCalculatorProgress? progress = null, CancellationToken cancellationToken = default)
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
            catch(OperationCanceledException)
            {
                Synchronize(() => progress?.ReportProcessState(ProcessState.Cancelled));

                throw;
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
        private async ValueTask CalculateWhenBothFilesNotEmpty(IDifferenceCalculatorProgress? progress, CancellationToken cancellationToken)
        {
            Synchronize(() =>
            {
                progress?.ReportLeftMainPosition(0);
                progress?.ReportRightMainPosition(0);
            });

            _newVersionPosition = 0;
            _oldVersionPosition = 0;
            _fileSegmentInfo = default;

            while(_newVersionPosition < NewVersion.Length)
            {
                cancellationToken.ThrowIfCancellationRequested();

                NewVersion.Position = _newVersionPosition;

                Synchronize(() => progress?.ReportRightMainPosition(_newVersionPosition));

                long lastOffset = 0;
                while(_oldVersionPosition < CurrentVersion!.Length)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    CurrentVersion.Position = _oldVersionPosition;

                    Synchronize(() => progress?.ReportLeftMainPosition(_oldVersionPosition));

                    lastOffset = await ScanDifferencesCore(progress, lastOffset, cancellationToken);

                    if(_newVersionPosition >= NewVersion.Length)
                        break;
                }

                if(_oldVersionPosition >= CurrentVersion.Length)
                    break;
            }

            AddSegmentForNewVersionEnding();

            Synchronize(() => RemovedSegmentsDetector.Detect(this.result, CurrentVersion!, this.removed, cancellationToken));
        }

        private async ValueTask<long> ScanDifferencesCore(IDifferenceCalculatorProgress? progress, long lastOffset, CancellationToken cancellationToken)
        {
            if(CurrentVersion is null)
                throw new InvalidOperationException("CurrentVersion cannot be null in this case.");

            for(long offset = 0; _newVersionPosition + offset < NewVersion.Length && _oldVersionPosition + offset < CurrentVersion.Length; ++offset)
            {
                CurrentVersion.Position = AdjustCurrentVersionPosition(offset);

                Synchronize(() =>
                {
                    progress?.ReportLeftOffsetedPosition(CurrentVersion.Position);
                    progress?.ReportRightOffsetedPosition(_newVersionPosition + offset);
                });

                NewVersion.Position = _newVersionPosition + offset;

                var left = await CurrentVersion.ReadByteAsync(cancellationToken);

                if(ReferenceEquals(CurrentVersion, NewVersion))
                    --CurrentVersion.Position;

                var right = await NewVersion.ReadByteAsync(cancellationToken);

                var completed =
                    left == right
                    ? CalculateWhenBytesAreEquals(lastOffset, offset)
                    : CalculateWhenBytesAreDifferent(lastOffset, offset);

                if(!completed)
                    break;

                lastOffset = offset;
            }

            return lastOffset;
        }

        private bool CalculateWhenBytesAreDifferent(long lastOffset, long offset)
        {
            if(_fileSegmentInfo.Source is null)
            {
                _fileSegmentInfo = new FileSegmentInfo
                {
                    Source = NewVersion,
                    MappedPosition = _newVersionPosition + offset,
                    StartPositionInSource = _newVersionPosition + offset,
                    EndPositionInSource = _newVersionPosition + offset
                };
            }
            else if(ReferenceEquals(_fileSegmentInfo.Source, CurrentVersion))
            {
                SaveSegmentAndClear(lastOffset);
                _oldVersionPosition += lastOffset + 1;

                return false;
            }
            else if(ReferenceEquals(_fileSegmentInfo.Source, NewVersion))
            {
                if(SaveCurrentSegmentAndClear(lastOffset, offset))
                    return false;

                _fileSegmentInfo.EndPositionInSource = _newVersionPosition + offset;
            }
            else
            {
                throw new InvalidOperationException("Unknown source of file segment.");
            }

            return true;
        }

        private bool CalculateWhenBytesAreEquals(long lastOffset, long offset)
        {
            if(_fileSegmentInfo.Source is null)
            {
                _fileSegmentInfo = new FileSegmentInfo
                {
                    Source = CurrentVersion!,
                    StartPositionInSource = _oldVersionPosition + offset,
                    EndPositionInSource = _oldVersionPosition + offset,
                    MappedPosition = _newVersionPosition + offset
                };
            }
            else if(ReferenceEquals(_fileSegmentInfo.Source, CurrentVersion))
            {
                if(offset <= lastOffset)
                {
                    SaveSegmentAndClear(lastOffset);
                    _oldVersionPosition += lastOffset + 1;

                    return false;
                }

                _fileSegmentInfo.EndPositionInSource = _oldVersionPosition + offset;
            }
            else if(ReferenceEquals(_fileSegmentInfo.Source, NewVersion))
            {
                SaveSegmentForCurrentVersionAndClear(lastOffset, offset);
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
        private void AddSegmentForNewVersionEnding()
        {
            if(_newVersionPosition < NewVersion.Length - 1)
            {
                Synchronize(() => this.result.Add(new FileSegmentInfo
                {
                    Source = NewVersion,
                    MappedPosition = _newVersionPosition,
                    StartPositionInSource = _newVersionPosition,
                    EndPositionInSource = NewVersion.Length - 1
                }));
            }
        }

        private bool SaveCurrentSegmentAndClear(long lastOffset, long offset)
        {
            if(offset <= lastOffset)
            {
                SaveSegmentAndClear(lastOffset);
                return true;
            }

            return false;
        }

        private void SaveSegmentAndClear(long lastOffset)
        {
            Synchronize(() => this.result.Add(_fileSegmentInfo));

            _fileSegmentInfo = default;
            
            _newVersionPosition += lastOffset + 1;
        }

        private void SaveSegmentForCurrentVersionAndClear(long lastOffset, long offset)
        {
            Synchronize(() => this.result.Add(_fileSegmentInfo));

            _fileSegmentInfo = default;
            if(offset <= lastOffset)
            {
                _newVersionPosition += lastOffset + 1;
                _oldVersionPosition += lastOffset + 1;
            }
            else if(CurrentVersion!.Position == _oldVersionPosition + 1)
            {
                _newVersionPosition += lastOffset + 1;
            }
        }

        /// <summary>
        /// When current segment source is not CurrentVersion, then CurrentVersion should not be moved by offset.
        /// </summary>
        /// <returns>Adjusted stream position</returns>
        private long AdjustCurrentVersionPosition(long offset) =>
            ReferenceEquals(_fileSegmentInfo.Source, CurrentVersion) ?
            _oldVersionPosition + offset :
            _oldVersionPosition;

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
