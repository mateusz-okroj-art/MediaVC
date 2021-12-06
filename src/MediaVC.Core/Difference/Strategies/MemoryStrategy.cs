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
        public long Length { get; }
        public long Position { get; set; }

        public bool Equals(IInputSourceStrategy? other) => throw new NotImplementedException();
        public int Read(byte[] buffer, int offset, int count) => throw new NotImplementedException();
        public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public ValueTask<byte> ReadByteAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }
}
