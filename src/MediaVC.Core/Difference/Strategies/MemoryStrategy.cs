﻿using System;
using System.Linq;
using System.Runtime.InteropServices;
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
                        throw new ArgumentOutOfRangeException(nameof(value));

                    this.position = value;
                }
            }
        }

        public bool Equals(IInputSourceStrategy? other) =>
            other?.Length == 0 && Length == 0 ||
               other?.Length == Length && other?.GetHashCode() == GetHashCode();

        public override bool Equals(object? obj) => obj is IInputSourceStrategy otherStrategy ? Equals(otherStrategy) : Equals(obj);

        public override int GetHashCode()
        {
            var span = MemoryMarshal.ToEnumerable(this.memory);

            var hashes = span.Select(value => value.GetHashCode());
            if(!hashes.Any())
            {
                return 0;
            }
            else if(hashes.Count() == 1)
            {
                return hashes.First();
            }
            else
            {
                var result = HashCode.Combine(hashes.First(), hashes.ElementAt(1));
                foreach(var value in hashes.Skip(2))
                    result = HashCode.Combine(result, hashes.ElementAt(0));

                return result;
            }
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            ArgumentNullException.ThrowIfNull(buffer);

            var slicedBuffer = buffer.AsMemory().Slice(offset, count);
            return ReadAsync(slicedBuffer).AsTask().GetAwaiter().GetResult();
        }

        public async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if(buffer.IsEmpty)
                return 0;

            if(this.position >= Length)
                throw new InvalidOperationException();

            return await ReadCoreAsync(buffer, cancellationToken);
        }

        private async ValueTask<int> ReadCoreAsync(Memory<byte> buffer, CancellationToken cancellationToken)
        {
            var currentMemory = this.memory[(int)position..];
            var readedCount = Math.Min(buffer.Length, currentMemory.Length);
            await Task.Run(() => currentMemory[..readedCount].CopyTo(buffer), cancellationToken);

            this.position += readedCount;

            return readedCount;
        }

        public ValueTask<byte> ReadByteAsync(CancellationToken cancellationToken = default)
        {
            if(this.position >= Length)
                throw new InvalidOperationException();

            var result = this.memory.Span[(int)this.position];

            cancellationToken.ThrowIfCancellationRequested();

            ++this.position;

            return ValueTask.FromResult(result);
        }
    }
}
