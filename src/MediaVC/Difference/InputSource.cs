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
        #region Constructors

        public InputSource(FileStream file)
        {
            throw new NotImplementedException();
        }

        public InputSource(IEnumerable<IFileSegmentInfo> segments)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Properties

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

        #endregion

        #region Methods

        public override void Flush() => throw new InvalidOperationException();

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value) => throw new InvalidOperationException();

        public override void Write(byte[] buffer, int offset, int count) => throw new InvalidOperationException();

        byte IInputSource.ReadByte()
        {
            throw new NotImplementedException();
        }

        public Memory<byte> ReadBytes(long count)
        {
            throw new NotImplementedException();
        }

        public bool Equals(InputSource other) => Strategy.Equals(other.Strategy);

        #endregion
    }
}
