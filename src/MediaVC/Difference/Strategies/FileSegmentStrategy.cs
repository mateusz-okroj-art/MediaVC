using System;
using System.Collections.Generic;
using System.Linq;

using MediaVC.Difference.FileSegments;

namespace MediaVC.Difference.Strategies
{
    internal sealed class FileSegmentStrategy : IInputSourceStrategy
    {
        #region Constructor

        public FileSegmentStrategy(IEnumerable<IFileSegmentInfo> segments)
        {
            this.segments = segments ?? throw new ArgumentNullException(nameof(segments));

            this.mappings = new IFileSegmentMappingInfo[segments.Count()];

            InitMappings();
        }

        #endregion

        #region Fields

        private readonly IEnumerable<IFileSegmentInfo> segments;
        private readonly IFileSegmentMappingInfo[] mappings;
        private long position;

        #endregion

        #region Properties

        public long Length => this.mappings
            .TakeLast(1)
            .Select(item => item.StartIndex + (long)item.Segment.Length)
            .FirstOrDefault();

        public long Position
        {
            get => this.position;
            set
            {
                if(value >= Length)
                    throw new ArgumentOutOfRangeException("Position is out of length.");

                this.position = value;
            }
        }

        #endregion

        #region Methods

        private void InitMappings()
        {
            var calculatedPosition = 0L;

            for(var i = 0; i < this.mappings.Length; ++i)
            {
                this.mappings[i] = new FileSegmentMappingInfo
                {
                    StartIndex = calculatedPosition,
                    Segment = this.segments.ElementAt(i)
                };

                calculatedPosition += (long)this.mappings[i].Segment.Length;
            }
        }

        private IFileSegmentMappingInfo? GetSegmentForCurrentPosition() =>
            this.mappings
                    .Where(mapping => mapping.CheckPositionIsInRange(Position))
                    .SingleOrDefault();

        public bool Equals(IInputSourceStrategy? other) =>
            other is FileSegmentStrategy strategy && this.segments == strategy.segments;

        public int Read(byte[] buffer, int offset, int count) =>
            Read(buffer.AsSpan(), offset, count);

        public int Read(Span<byte> buffer, int offset, int count)
        {
            if(offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));

            var bufferPosition = offset;

            var counter = 0;

            while(bufferPosition < buffer.Length - 1 && counter < count)
            {
                var mappedSegment = GetSegmentForCurrentPosition();

                if(mappedSegment is null)
                    break;

                var source = mappedSegment.Segment.Source;
                source.Position = mappedSegment.Segment.StartPosition;

                var currentCount = Math.Min(count, (int)mappedSegment.Segment.Length);

                currentCount = source.Read(buffer, bufferPosition, currentCount);

                counter += currentCount;
                bufferPosition += currentCount;

                Position += currentCount;
            }

            return counter;
        }

        public byte ReadByte()
        {
            var mappedSegment = this.mappings
                    .Where(mapping => mapping.CheckPositionIsInRange(Position))
                    .SingleOrDefault();

            if(mappedSegment is null)
                throw new InvalidOperationException();

            var source = mappedSegment.Segment.Source;

            source.Position = mappedSegment.Segment.StartPosition +
                                                    Position -
                                                    mappedSegment.StartIndex;

            return source.ReadByte();
        }

        #endregion
    }
}
