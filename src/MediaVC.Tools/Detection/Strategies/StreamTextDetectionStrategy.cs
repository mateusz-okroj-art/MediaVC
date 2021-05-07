using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MediaVC.Tools.Detection.Strategies
{
    internal class StreamTextDetectionStrategy : ITextDetectionStrategy
    {
        #region Constructor

        public StreamTextDetectionStrategy(Stream stream)
        {
            if(stream is null)
                throw new ArgumentNullException(nameof(stream));

            if(!stream.CanRead)
                throw new IOException("Stream is not readable.");

            this.stream = stream;

            var bufferLength = (int)Math.Min(stream.Length, 500_000_000);

            if(bufferLength <= 500_000_000)
            {
                this.buffer = new byte[bufferLength];
                this.canReadAll = true;
            }
            else
            {
                this.buffer = new byte[500_000_000];
                this.canReadAll = false;
            }
        }

        #endregion

        #region Fields

        private readonly Stream stream;
        private readonly Memory<byte> buffer;
        private readonly bool canReadAll;

        #endregion

        public async ValueTask<bool> CheckIsTextAsync(CancellationToken cancellationToken = default)
        {
            if(stream.Length < 1)
                return false;

            var memoryStrategy = new MemoryTextDetectionStrategy(this.buffer);

            if(this.canReadAll)
            {
                this.stream.Position = 0;

                await this.stream.ReadAsync(this.buffer, cancellationToken);

                return await memoryStrategy.CheckIsTextAsync(cancellationToken);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
