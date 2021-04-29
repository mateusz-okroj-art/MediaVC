using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediaVC.Difference;
using MediaVC.Difference.FileSegments;

namespace MediaVC.Tools.Difference
{
    public interface IDifferenceCalculator
    {
        ValueTask CalculateAsync();

        ValueTask CalculateAsync(CancellationToken cancellation, IProgress<float>? progress);

        IInputSource? CurrentVersion { get; }

        IInputSource NewVersion { get; }

        IList<IFileSegmentInfo> Result { get; }

        SynchronizationContext SynchronizationContext { get; }
    }
}
