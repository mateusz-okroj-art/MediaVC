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

            Stream = stream;

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

        private readonly Memory<byte> buffer;
        private readonly bool canReadAll;

        #endregion

        #region Properties

        public Stream Stream { get; }

        #endregion

        public async ValueTask<bool> CheckIsTextAsync(CancellationToken cancellationToken = default)
        {
            if(Stream.Length < 1)
                return false;

            var memoryStrategy = new MemoryTextDetectionStrategy(this.buffer);

            if(this.canReadAll)
            {
                Stream.Position = 0;

                _ = await Stream.ReadAsync(this.buffer, cancellationToken);

                return await memoryStrategy.CheckIsTextAsync(cancellationToken);
            }
            else
            {
                var bufferA = this.buffer.Slice(0, 250_000_000);
                var bufferB = this.buffer.Slice(249_000_000, 250_000_000);

                var streamLocker = new object();

                MemoryTextDetectionStrategy detector1, detector2;

                var canContinue = false;

                bool? result = null;

                Stream.Position = 0; 

                while(Stream.Position < Stream.Length)
                {
                    if(result.HasValue)
                        return result.Value;

                    Monitor.Enter(streamLocker);

                    canContinue = await Stream.ReadAsync(bufferA, cancellationToken) == 250_000_000;

                    if(canContinue)
                    {
                        Stream.ReadAsync(bufferB, cancellationToken)
                            .GetAwaiter()
                            .OnCompleted(async () =>
                            {
                                Monitor.Exit(streamLocker);

                                detector2 = new MemoryTextDetectionStrategy(bufferB);
                                if(!await detector2.CheckIsTextAsync(cancellationToken))
                                {
                                    result = false;
                                    return;
                                }
                            });
                    }

                    if(result.HasValue)
                        return result.Value;

                    detector1 = new MemoryTextDetectionStrategy(bufferA);

                    if(!await detector1.CheckIsTextAsync(cancellationToken))
                        return false;

                }

                return result ?? true;
            }
        }
    }
}
