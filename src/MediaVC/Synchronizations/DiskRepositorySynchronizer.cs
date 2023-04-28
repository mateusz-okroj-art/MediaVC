using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace MediaVC.Synchronizations
{
    internal sealed class DiskRepositorySynchronizer : IRepositorySynchronization
    {
        public DiskRepositorySynchronizer(DirectoryInfo directory)
        {
            this.directory = directory;

            if(CheckIsLocked())
                throw new ExternallyLockedRepositoryException();
        }

        private readonly DirectoryInfo directory;
        private FileStream? locker_file;
        private object locker_semaphore = new();

        private Subject<Unit> unlocked = new Subject<Unit>();
        private Subject<Unit> locked = new Subject<Unit>();

        private bool CheckIsLocked()
        {
            return directory.EnumerateFiles().Any(file => file.Name == ".locker");
        }

        public async IAsyncDisposable EnterAsync(CancellationToken cancellationToken)
        {
            if(locker_file is not null)
            {
               Monitor.
            }
        }
    }
}
