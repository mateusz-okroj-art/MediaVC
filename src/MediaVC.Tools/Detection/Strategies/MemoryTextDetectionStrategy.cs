using System;
using System.Linq;
using System.Runtime.InteropServices;
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

        #region Methods

        public async ValueTask<bool> CheckIsTextAsync(CancellationToken cancellationToken = default)
        {
            var input = MemoryMarshal.ToEnumerable<byte>(Memory);

            var length = input.Count();

            if(length < 1)
                return false;
            else if(length < 10_000)
                foreach(var value in input)
                {
                    if(!CheckSingleCharacter(value))
                        return false;
                }
            else
            {
                var stopped = false;
                var locker = new object();

                _ = Parallel.ForEach(input, (value, state) =>
                {
                    if(!CheckSingleCharacter(value))
                    {
                        lock(locker)
                            stopped = true;

                        state.Break();
                    }
                });

                return stopped;
            }

            return true;
        }

        private static bool CheckSingleCharacter(byte character) =>
            character is (0 or >= 8) and (<= 13 or >= 26);

        #endregion
    }
}