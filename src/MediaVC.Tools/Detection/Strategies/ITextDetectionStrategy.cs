using System.Threading;
using System.Threading.Tasks;

namespace MediaVC.Tools.Detection.Strategies
{
    internal interface ITextDetectionStrategy
    {
        ValueTask<bool> CheckIsTextAsync(CancellationToken cancellationToken = default);
    }
}
