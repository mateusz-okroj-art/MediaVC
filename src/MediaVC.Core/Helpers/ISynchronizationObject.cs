using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediaVC
{
    public interface ISynchronizationObject : IDisposable
    {
        /// <summary>
        /// Releases current synchronization object, if required.
        /// </summary>
        void Release();

        /// <summary>
        /// Waits for availability of synchronization object.
        /// </summary>
        ValueTask WaitForAsync(CancellationToken cancellationToken = default);
    }
}