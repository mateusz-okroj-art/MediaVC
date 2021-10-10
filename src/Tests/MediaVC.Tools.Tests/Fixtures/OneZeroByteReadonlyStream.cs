﻿
using System;

using MediaVC.Difference;

namespace MediaVC.Tools.Tests.Fixtures
{
    internal sealed class OneZeroByteReadonlyStream : IInputSource
    {
        private long position;

        public long Position
        {
            get => this.position;
            set
            {
                if(value != 0)
                    throw new ArgumentOutOfRangeException();

                this.position = value;
            }
        }

        public long Length => 1;

        public void Dispose() { }

        public int Read(byte[] buffer, int offset, int count) =>
            buffer is not null ?
            Read(buffer.AsMemory().Slice(offset, count)) :
            throw new ArgumentNullException(nameof(buffer));

        public int Read(Memory<byte> buffer)
        {
            if(buffer.IsEmpty || buffer.Length < 1)
                return 0;

            buffer.Span[0] = 0;

            return 1;
        }

        public byte ReadByte()
        {
            if(Position != 0)
                throw new InvalidOperationException("Stream is on end.");

            ++this.position;

            return 0;
        }
    }
}
