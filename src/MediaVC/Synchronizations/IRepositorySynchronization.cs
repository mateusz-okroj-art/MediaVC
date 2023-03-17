using System;

namespace MediaVC.Synchronizations
{
    public interface IRepositorySynchronization
    {
        IAsyncDisposable EnterAsync();
    }
}
