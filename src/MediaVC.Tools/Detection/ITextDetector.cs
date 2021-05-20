using System.Threading;
using System.Threading.Tasks;

namespace MediaVC.Tools.Detection
{
    public interface ITextDetector
    {
        ValueTask<bool> CheckIsTextAsync(CancellationToken cancellationToken = default);
    }
}
