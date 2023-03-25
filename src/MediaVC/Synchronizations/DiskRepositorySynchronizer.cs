using System;
using System.Threading;

namespace MediaVC.Synchronizations
{
    internal class DiskRepositorySynchronizer : IRepositorySynchronization
    {
        public DiskRepositorySynchronizer() { }

        public IAsyncDisposable EnterAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
    }
}
