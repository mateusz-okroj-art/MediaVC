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
        private long position = -1;

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
                throw new NotImplementedException();
            }
        }

        public bool Equals(IInputSourceStrategy? other) =>
            other is FileSegmentStrategy strategy && this.segments == strategy.segments;

        public int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public byte ReadByte()
        {
            throw new NotImplementedException();
        }
    }
}
