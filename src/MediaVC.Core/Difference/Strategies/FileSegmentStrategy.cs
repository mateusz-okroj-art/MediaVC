using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        private readonly Memory<byte> readerBuffer = new byte[2048];
        private int bufferStartPosition = -1;

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
        /// Reads data to selected buffer.
        /// </summary>
        /// <param name="buffer">Location of the read data</param>
        /// <returns>Returns readed bytes count.</returns>
        /// <exception cref="ArgumentOutOfRangeException" />
        public int Read(byte[] buffer, int offset, int count) =>
            ReadAsync(buffer.AsMemory().Slice(offset, count)).AsTask().GetAwaiter().GetResult();

        /// <summary>
        /// Reads data to selected buffer.
        /// </summary>
        /// <param name="buffer">Location of the read data</param>
        /// <returns>Returns readed bytes count.</returns>
        public async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            var counter = 0;

            if(buffer.IsEmpty || buffer.Length < 1)
                return counter;

            var currentSegment = SelectMappedSegmentForCurrentPosition();
            while(currentSegment?.Length > 0 && counter < buffer.Length)
            {
                cancellationToken.ThrowIfCancellationRequested();

                currentSegment.Source!.Position = Position - currentSegment.MappedPosition + currentSegment.StartPositionInSource;

                counter += await currentSegment.Source.ReadAsync(buffer[counter..], cancellationToken);

                currentSegment = SelectMappedSegmentForCurrentPosition();
            }

            return counter;
        }

        private IFileSegmentInfo? SelectMappedSegmentForCurrentPosition() => this.segments.OrderBy(segment => segment.MappedPosition)
                .FirstOrDefault(segment =>
                    segment.MappedPosition < Position &&
                    segment.MappedPosition + (long)segment.Length >= Position
                );

        /// <summary>
        /// Returns next <see langword="byte"/>, if position is there any left. Otherwise throws <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException" />
        /// <exception cref="OperationCanceledException" />
        public async ValueTask<byte> ReadByteAsync(CancellationToken cancellationToken = default)
        {
            if(Position < 0 || Position >= Length)
                throw new InvalidOperationException();

            if(Position > int.MaxValue)
            {
                Memory<byte> buffer = new byte[1];

                _ = await ReadAsync(buffer, cancellationToken);

                return buffer.Span[0];
            }

            if(this.bufferStartPosition < 0 || this.bufferStartPosition > Position)
            {
                this.bufferStartPosition = (int)Position;

                var currentPosition = Position;
                _ = await ReadAsync(this.readerBuffer, cancellationToken);
                Position = currentPosition;
            }

            return this.readerBuffer.Span[(int)Position - this.bufferStartPosition];
        }

        /// <summary>
        ///   <para>Checks that selected source (for example parent, that use this strategy) is not used in any of collected segments.</para>
        ///   <para>This prevents the loopback.</para>
        /// </summary>
        /// <param name="source"></param>
        /// <returns>When detected loopback is returned <see langword="false"/>. Otherwise is <see langword="true"/>.</returns>
        public bool CheckIsNotUsedSource(IInputSource source) => !this.segments.Any(segment => ReferenceEquals(segment.Source, source));

        #endregion
    }
}
