using System.Threading;
using System.Threading.Tasks;

using MediaVC.Difference;

namespace MediaVC.Tools.Difference
{
    public interface IDifferenceCalculator
    {
        ValueTask CalculateAsync(IDifferenceCalculatorProgress? progress = null, CancellationToken cancellationToken = default);

        IInputSource? CurrentVersion { get; }

        IInputSource NewVersion { get; }

        IObservableEnumerable<IFileSegmentInfo> Result { get; }

        IObservableEnumerable<IFileSegmentInfo> Removed { get; }

        SynchronizationContext? SynchronizationContext { get; }
    }
}
