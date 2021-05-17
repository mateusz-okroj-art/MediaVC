using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            if(this.canReadAll)
            {
                Stream.Position = 0;

                _ = await Stream.ReadAsync(this.buffer, cancellationToken);

                var memoryStrategy = new MemoryTextDetectionStrategy(this.buffer);
                return await memoryStrategy.CheckIsTextAsync(cancellationToken);
            }
            else
            {
                var bufferA = this.buffer.Slice(0, 250_000_000);
                var bufferB = this.buffer.Slice(249_000_000, 250_000_000);

                var streamLocker = new object();

                MemoryTextDetectionStrategy detector1, detector2;
                IList<Task> activeActions = new List<Task>();

                var canContinue = false;

                bool? result = null;

                Stream.Position = 0;

                do
                {
                    if(result.HasValue)
                        return result.Value;

                    Monitor.Enter(streamLocker);

                    canContinue = await Stream.ReadAsync(bufferA, cancellationToken) == 250_000_000;

                    if(canContinue)
                    {
                        var streamTask = Stream.ReadAsync(bufferB, cancellationToken).AsTask();
                        activeActions.Add(streamTask);

                        streamTask
                            .GetAwaiter()
                            .OnCompleted(async () =>
                            {
                                Monitor.Exit(streamLocker);

                                detector2 = new MemoryTextDetectionStrategy(bufferB);

                                var detectionTask = detector2.CheckIsTextAsync(cancellationToken).AsTask();
                                activeActions.Add(detectionTask);

                                if(!await detectionTask)
                                    result = false;

                                _ = activeActions.Remove(detectionTask);
                                _ = activeActions.Remove(streamTask);
                            });
                    }
                    else
                        Monitor.Exit(streamLocker);

                    if(result.HasValue)
                        return result.Value;

                    detector1 = new MemoryTextDetectionStrategy(bufferA);

                    if(!await detector1.CheckIsTextAsync(cancellationToken))
                        return false;
                }
                while(Stream.Position < Stream.Length);

                Task.WaitAll(activeActions.ToArray(), cancellationToken);

                return result ?? true;
            }
        }
    }
}
