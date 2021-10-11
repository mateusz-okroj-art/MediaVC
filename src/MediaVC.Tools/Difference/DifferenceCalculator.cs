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

        public ValueTask CalculateAsync() => CalculateAsync(default, null);

        /// <summary>
        /// Calculates differences between sources
        /// </summary>
        /// <param name="cancellation"></param>
        /// <exception cref="OperationCanceledException" />
        public async ValueTask CalculateAsync(CancellationToken cancellation, IProgress<float>? progress) =>
            await Task.Run(() => Calculate(cancellation, progress));

        private void Calculate(CancellationToken cancellationToken, IProgress<float>? progress)
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
                for(long newVersionPosition = 0; newVersionPosition < NewVersion.Length; ++newVersionPosition)
                {

                }
            }
            else if(CurrentVersion?.Length < 1 && NewVersion.Length > 0)
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
