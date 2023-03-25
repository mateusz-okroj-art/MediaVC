using System;
using System.Threading;

namespace MediaVC.Synchronizations
{
    public interface IRepositorySynchronization
    {
        IAsyncDisposable EnterAsync(CancellationToken cancellationToken);
    }
}
