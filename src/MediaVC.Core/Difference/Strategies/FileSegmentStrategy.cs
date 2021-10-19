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
            Length = this.segments.Select(segment => (long)segment.Length).Sum();
        }

        #endregion

        #region Fields

        private readonly IEnumerable<IFileSegmentInfo> segments;
        private long position;

        #endregion

        #region Properties

        public long Length { get; }

        public long Position
        {
            get => this.position;
            set
            {
                if(value >= Length)
                    throw new ArgumentOutOfRangeException(nameof(Position), $"{nameof(Position)} is out of length.");

                this.position = value;
            }
        }

        #endregion

        #region Methods

        /*private IFileSegmentMappingInfo? GetSegmentForCurrentPosition() =>
            this.mappings
                    .SingleOrDefault(mapping => mapping?.CheckPositionIsInRange(Position) ?? false);

        private void SetPositionOnMappedSegment(ref IFileSegmentMappingInfo mappingInfo) =>
            mappingInfo.Segment.Source.Position = Position -
                                                  mappingInfo.StartIndex +
                                                  mappingInfo.Segment.StartPosition;*/

        public bool Equals(IInputSourceStrategy? other) =>
            other is FileSegmentStrategy strategy ?
            GetHashCode() == strategy.GetHashCode() :
            Equals(other as object);

        public override bool Equals(object? obj) =>
            obj is IInputSourceStrategy strategy ?
            Equals(strategy) :
            Equals(this, obj);

        public override int GetHashCode()
        {
            if(this.segments?.Count() < 1)
                return 0;

            var query = from segment in this.segments
                    where segment is not null
                    select segment;
            
            var hashes = query.AsParallel().Select(segment => segment.GetHashCode()).ToArray();

            if(hashes is null || hashes.Length <= 0)
            {
                return 0;
            }
            else if(hashes.Length == 1)
            {
                return hashes[0];
            }
            else
            {
                var result = HashCode.Combine(hashes[0], hashes[1]);

                foreach(var hash in hashes.Skip(2))
                    result = HashCode.Combine(result, hash);

                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException" />
        public int Read(byte[] buffer, int offset, int count) =>
            Read(buffer.AsMemory().Slice(offset, count));

        public int Read(Memory<byte> buffer)
        {
            /*var counter = 0;

            if(buffer.IsEmpty || buffer.Length < 1)
                return counter;

            for(var bufferPosition = 0; bufferPosition < buffer.Length;)
            {
                var mappedSegment = GetSegmentForCurrentPosition();

                if(mappedSegment is null)
                    break;

                SetPositionOnMappedSegment(ref mappedSegment);

                var subBuffer = buffer[bufferPosition..];

                var readedBytesCount = mappedSegment.Segment.Source.Read(subBuffer);

                bufferPosition += readedBytesCount;
                Position += readedBytesCount;
                counter += readedBytesCount;
            }

            return counter;*/
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException" />
        public byte ReadByte()
        {
            /*var mappedSegment = this.mappings
                    .SingleOrDefault(mapping => mapping?.CheckPositionIsInRange(Position) ?? false);

            if(mappedSegment is null)
                throw new InvalidOperationException();

            SetPositionOnMappedSegment(ref mappedSegment);

            ++this.position;

            return mappedSegment.Segment.Source.ReadByte();*/
            throw new NotImplementedException();
        }

        #endregion
    }
}
