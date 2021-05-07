using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediaVC.Tools.Detection.Strategies
{
    internal class MemoryTextDetectionStrategy : ITextDetectionStrategy
    {
        #region Constructor

        public MemoryTextDetectionStrategy(Memory<byte> memory)
        {
            if(memory.IsEmpty)
                throw new ArgumentException("Argument is empty.");

            Memory = memory;
        }

        #endregion

        #region Properties

        public Memory<byte> Memory { get; }

        #endregion

        public async ValueTask<bool> CheckIsTextAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}