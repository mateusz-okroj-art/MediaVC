using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediaVC.Enumerators;

namespace MediaVC.Difference.Concurrency
{
    public sealed class InputSourceProxy : IInputSource
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceConcurrentProvider"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public InputSourceProxy(IInputSourceConcurrentProvider sourceConcurrentProvider) =>
            this.sourceConcurrentProvider = sourceConcurrentProvider ?? throw new ArgumentNullException(nameof(sourceConcurrentProvider));

        private readonly IInputSourceConcurrentProvider sourceConcurrentProvider;
        private long position;

        #region Properties

        public long Position
        {
            get => this.position;
            set
            {
                if(value != this.position)
                {
                    if(value >= Length)
                        throw new ArgumentOutOfRangeException(nameof(value));

                    this.position = value;
                }
            }
        }

        public long Length => this.sourceConcurrentProvider.Length;

        #endregion

        public int Read(byte[] buffer, int offset, int count)
        {
            long previousPosition = -1;

            try
            {
                this.sourceConcurrentProvider.SynchronizationObject.WaitForAsync().AsTask().GetAwaiter().GetResult();

                previousPosition = this.sourceConcurrentProvider.Position;

                this.sourceConcurrentProvider.Position = Position;
                return this.sourceConcurrentProvider.Read(buffer, offset, count);
            }
            finally
            {
                this.sourceConcurrentProvider.Position = previousPosition;
                this.sourceConcurrentProvider.SynchronizationObject.Release();
            }
        }

        public async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            long previousPosition = -1;

            try
            {
                await this.sourceConcurrentProvider.SynchronizationObject.WaitForAsync(cancellationToken);

                previousPosition = this.sourceConcurrentProvider.Position;

                this.sourceConcurrentProvider.Position = Position;
                return await this.sourceConcurrentProvider.ReadAsync(buffer, cancellationToken);
            }
            finally
            {
                this.sourceConcurrentProvider.Position = previousPosition;
                this.sourceConcurrentProvider.SynchronizationObject.Release();
            }
        }

        public async ValueTask<byte> ReadByteAsync(CancellationToken cancellationToken = default)
        {
            long previousPosition = -1;

            try
            {
                await this.sourceConcurrentProvider.SynchronizationObject.WaitForAsync(cancellationToken);

                previousPosition = this.sourceConcurrentProvider.Position;

                this.sourceConcurrentProvider.Position = Position;
                return await this.sourceConcurrentProvider.ReadByteAsync(cancellationToken);
            }
            finally
            {
                this.sourceConcurrentProvider.Position = previousPosition;
                this.sourceConcurrentProvider.SynchronizationObject.Release();
            }
        }

        public IAsyncEnumerator<byte> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new InputSourceEnumerator(this, cancellationToken);
    }
}
