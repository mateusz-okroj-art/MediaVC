using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace MediaVC.Tools.Detection.Strategies
{
    internal class MemoryTextDetectionStrategy : ITextDetector
    {
        #region Constructor

        public MemoryTextDetectionStrategy(ReadOnlyMemory<byte> memory)
        {
            if(memory.IsEmpty)
                throw new ArgumentException("Argument is empty.");

            Memory = memory;
        }

        #endregion

        #region Properties

        public ReadOnlyMemory<byte> Memory { get; }

        #endregion

        #region Methods

        public async ValueTask<bool> CheckIsTextAsync(CancellationToken cancellationToken = default) =>
            await Task.Run(() =>
            {
                var input = MemoryMarshal.ToEnumerable(Memory);

                var length = input.Count();

                var isText = true;

                if(length < 1)
                {
                    isText = false;
                }
                else if(length < 100_000)
                {
                    foreach(var value in input)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        if(!CheckSingleCharacter(value))
                        {
                            isText = false;
                            break;
                        }
                    }
                }
                else
                {
                    var locker = new object();

                    _ = Parallel.ForEach(input, (value, state) =>
                    {
                        if(cancellationToken.IsCancellationRequested)
                        {
                            state.Break();

                            return;
                        }

                        if(!CheckSingleCharacter(value))
                        {
                            lock(locker)
                                isText = false;

                            state.Break();
                        }
                    });

                    if(cancellationToken.IsCancellationRequested)
                        throw new OperationCanceledException();
                }

                return isText;
            });

        private static bool CheckSingleCharacter(byte character) =>
            character is >= 0 and <= 127;

        #endregion
    }
}