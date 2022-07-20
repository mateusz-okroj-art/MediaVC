using System.Threading;
using System.Threading.Tasks;

namespace MediaVC
{
    public sealed class SynchronizationObject : ISynchronizationObject
    {
        public SynchronizationObject(int initCount = 1, int maxCount = 1)
        {
            this.maxCount = maxCount;
            this.semaphore = new SemaphoreSlim(initCount, maxCount);
        }

        private readonly SemaphoreSlim semaphore;
        private readonly int maxCount;

        public async ValueTask WaitForAsync(CancellationToken cancellationToken = default) =>
            await this.semaphore.WaitAsync(cancellationToken);

        public void Release()
        {
            if(this.semaphore.CurrentCount < this.maxCount)
                _ = this.semaphore.Release();
        }

        public void Dispose() => this.semaphore.Dispose();
    }
}
