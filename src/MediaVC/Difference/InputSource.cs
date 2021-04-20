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
            if (file is null)
                throw new ArgumentNullException(nameof(file));

            Strategy = new FileStreamStrategy(file);
        }

        public InputSource(IEnumerable<IFileSegmentInfo> segments)
        {
            if (segments is null)
                throw new ArgumentNullException(segments);

            Strategy = new FileSegmentStrategy(segments);
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



        public override int Read(byte[] buffer, int offset, int count) =>
            Strategy.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin)
        {
            var calculatedPosition = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => Position + offset,
                SeekOrigin.End => Length - offset - 1,
                _ => throw new NotImplementedException(),
            };

            Position = calculatedPosition;

            return calculatedPosition;
        }

        byte IInputSource.ReadByte()
        {
            throw new NotImplementedException();
        }

        public Memory<byte> ReadBytes(long count)
        {
            throw new NotImplementedException();
        }

        public bool Equals(InputSource? other) => Strategy.Equals(other.Strategy);

        #region Obsoletes

        [Obsolete]
        public override void Flush() => throw new InvalidOperationException();

        [Obsolete]
        public override void SetLength(long value) => throw new InvalidOperationException();

        [Obsolete]
        public override void Write(byte[] buffer, int offset, int count) => throw new InvalidOperationException();

        #endregion

        #endregion
    }
}
