using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediaVC.Difference;

namespace MediaVC.Enumerators
{
    internal class InputSourceEnumerator : IAsyncEnumerator<byte>
    {
        public InputSourceEnumerator(IInputSource inputSource, CancellationToken cancellationToken)
        {
            this.inputSource = inputSource ?? throw new ArgumentNullException(nameof(inputSource));
            this.cancellationToken = cancellationToken;
        }

        private bool isInitialized = false;

        private readonly IInputSource inputSource;
        private readonly CancellationToken cancellationToken;

        public byte Current { get; private set; }

        public async ValueTask<bool> MoveNextAsync()
        {
            cancellationToken.ThrowIfCancellationRequested();

            if(!isInitialized)
            {
                if(this.inputSource.Length < 1)
                    return false;

                this.inputSource.Position = 0;
                this.isInitialized = true;
                Current = await this.inputSource.ReadByteAsync(this.cancellationToken);
            }
            else
            {
                if(this.inputSource.Length <= this.inputSource.Position + 1)
                    return false;

                ++this.inputSource.Position;

                Current = await this.inputSource.ReadByteAsync(this.cancellationToken);
            }

            return true;
        }

        public async ValueTask DisposeAsync() => await Task.CompletedTask;
    }
}
