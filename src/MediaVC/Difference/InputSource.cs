using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using MediaVC.Difference.FileSegments;
using MediaVC.Difference.Strategies;

namespace MediaVC.Difference
{
    public sealed class InputSource : Stream, IInputSource, IEquatable<InputSource>
    {
        public InputSource(FileStream file)
        {

        }

        public InputSource(IEnumerable<IFileSegmentInfo> segments)
        {

        }

        internal IInputSourceStrategy Strategy { get; }

        public override bool CanRead { get; } = true;

        public override bool CanSeek { get; } = true;

        public override bool CanWrite { get; } = false;

        public override long Length => Strategy.Length;

        public override long Position
        {
            get => Strategy.Position;
            set => Strategy.Position = value;
        }


        public override void Flush() => throw new InvalidOperationException();

        public override int Read(byte[] buffer, int offset, int count)
        {

        }

        public override long Seek(long offset, SeekOrigin origin)
        {

        }

        public override void SetLength(long value) => throw new InvalidOperationException();

        public override void Write(byte[] buffer, int offset, int count) => throw new InvalidOperationException();

        byte IInputSource.ReadByte()
        {
            
        }

        public Memory<byte> ReadBytes(long count)
        {
            
        }

        public bool Equals(InputSource other) => Strategy.Equals(other.Strategy);
    }
}
