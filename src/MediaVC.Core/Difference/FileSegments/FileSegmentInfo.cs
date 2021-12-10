using System;

namespace MediaVC.Difference
{
    public struct FileSegmentInfo : IFileSegmentInfo
    {

        public long StartPositionInSource { get; set; } = -1;

        public long EndPositionInSource { get; set; } = -1;

        public long MappedPosition { get; set; } = -1;

        public readonly ulong Length => IsPositionsValid() ? (ulong)(EndPositionInSource - StartPositionInSource + 1) : 0;

        public IInputSource? Source { get; set; }

        private readonly bool IsPositionsValid() =>
            StartPositionInSource >= 0 && StartPositionInSource <= EndPositionInSource && EndPositionInSource >= StartPositionInSource;

        public override int GetHashCode() =>
            HashCode.Combine(StartPositionInSource.GetHashCode(), EndPositionInSource.GetHashCode(), Source?.GetHashCode());

        public override bool Equals(object? obj) => 
            obj is IFileSegmentInfo fileSegmentInfo ? Equals(fileSegmentInfo) : base.Equals(obj);

        public bool Equals(IFileSegmentInfo? other) =>
            other is not null &&
            StartPositionInSource == other.StartPositionInSource &&
            EndPositionInSource == other.EndPositionInSource &&
            (Source?.Equals(other.Source) ?? false);

        public static bool operator ==(FileSegmentInfo left, FileSegmentInfo right) => left.Equals(right);

        public static bool operator !=(FileSegmentInfo left, FileSegmentInfo right) => !(left == right);
    }
}
