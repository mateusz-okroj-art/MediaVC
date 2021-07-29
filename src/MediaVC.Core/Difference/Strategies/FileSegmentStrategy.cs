using System;
using System.Collections.Generic;
using System.Linq;

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
            other is FileSegmentStrategy strategy ?
            GetHashCode() == strategy.GetHashCode() :
            Equals(this, other);

        public override int GetHashCode()
        {
            if(this.segments?.Count() < 1)
                return base.GetHashCode();

            var query = from segment in this.segments
                    where segment is not null
                    select segment;

            
            var hashes = query.AsParallel().Select(segment => segment.GetHashCode()).ToArray();

            if(hashes.Length <= 0)
                return 0;
            else if(hashes.Length == 1)
                return hashes[0];
            else
            {
                var result = HashCode.Combine(hashes[0], hashes[1]);

                foreach(var hash in hashes.Skip(2))
                    result = HashCode.Combine(result, hash);

                return result;
            }
        }

        public int Read(byte[] buffer, int offset, int count) =>
            Read(buffer.AsMemory(), offset, count);

        public int Read(Memory<byte> buffer, int offset, int count)
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

            ++this.position;

            return source.ReadByte();
        }

        #endregion
    }
}
