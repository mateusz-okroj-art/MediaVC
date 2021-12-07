using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MediaVC.Difference.Strategies
{
    internal class MemoryStrategy : IInputSourceStrategy
    {
        public MemoryStrategy(ReadOnlyMemory<byte> memory)
        {
            if(memory.IsEmpty)
                throw new ArgumentException("Memory is empty.");

            this.memory = memory;
        }

        private readonly ReadOnlyMemory<byte> memory;
        private long position = 0;

        public long Length => this.memory.Length;

        public long Position
        {
            get => this.position;
            set
            {
                if(value != this.position)
                {
                    if(value >= Length)
                        throw new ArgumentOutOfRangeException();

                    this.position = value;
                }
            }
        }

        public bool Equals(IInputSourceStrategy? other) => throw new NotImplementedException();

        public int Read(byte[] buffer, int offset, int count) => throw new NotImplementedException();

        public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        public ValueTask<byte> ReadByteAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }
}
