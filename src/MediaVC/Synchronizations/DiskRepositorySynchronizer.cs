using System;

namespace MediaVC.Synchronizations
{
    internal class DiskRepositorySynchronizer : IRepositorySynchronization
    {
        public DiskRepositorySynchronizer() { }

        public IAsyncDisposable EnterAsync() => throw new NotImplementedException();
    }
}
