using MediaVC.Difference.FileSegments;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaVC.Difference.Strategies
{
    internal sealed class FileSegmentStrategy : IInputSourceStrategy
    {
        private readonly IEnumerable<IFileSegmentInfo> segments;
        private readonly IFileSegmentMappingInfo[] mappings;
        private long position;

        public FileSegmentStrategy(IEnumerable<IFileSegmentInfo> segments)
        {
            this.segments = segments ?? throw new ArgumentNullException(nameof(segments));

            this.mappings = new IFileSegmentMappingInfo[segments.Count()];

            InitMappings();
        }

        private void InitMappings()
        {
            /*for()
            {

            }*/
        }

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
                    throw new ArgumentOutOfRangeException();

                this.position = value;
            }
        }

        public bool Equals(IInputSourceStrategy? other) =>
            other is FileSegmentStrategy strategy && this.segments == strategy.segments;

        public int Read(byte[] buffer, int offset, int count)
        {
            if(offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));

            int bufferPosition = offset;

            int counter = 0;

            while(bufferPosition < buffer.Length - 1 && counter < count)
            {
                var mappedSegment = this.mappings
                    .Where(mapping =>
                        mapping.StartIndex < Position &&
                        mapping.StartIndex + (long)mapping.Segment.Length > Position
                    )
                    .SingleOrDefault();

                if(mappedSegment is null)
                    break;

                var source = mappedSegment.Segment.Source;
                source.Position = mappedSegment.Segment.StartPosition;

                var currentCount = Math.Min(count, (int)mappedSegment.Segment.Length);

                currentCount = source.Read(buffer, bufferPosition, currentCount);

                counter += currentCount;
                bufferPosition += currentCount;
            }

            return counter;
        }

        public byte ReadByte()
        {
            var buffer = new byte[1];

            return Read(buffer, 0, 1) != 1 ? throw new InvalidOperationException() : buffer[0];
        }
    }
}
