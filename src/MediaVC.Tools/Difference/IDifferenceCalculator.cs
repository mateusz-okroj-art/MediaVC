using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediaVC.Difference;

namespace MediaVC.Tools.Difference
{
    public interface IDifferenceCalculator
    {
        ValueTask CalculateAsync();

        ValueTask CalculateAsync(CancellationToken cancellation, IProgress<float>? progress);

        IInputSource? CurrentVersion { get; }

        IInputSource NewVersion { get; }

        IObservableEnumerable<IFileSegmentInfo> Result { get; }

        IObservableEnumerable<IFileSegmentInfo> Removed { get; }

        SynchronizationContext? SynchronizationContext { get; }
    }
}
