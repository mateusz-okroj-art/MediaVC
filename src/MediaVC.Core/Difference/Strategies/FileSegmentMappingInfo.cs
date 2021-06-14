namespace MediaVC.Difference.Strategies
{
    internal struct FileSegmentMappingInfo : IFileSegmentMappingInfo
    {
        public IFileSegmentInfo Segment { get; set; }

        public long StartIndex { get; set; }

        public readonly bool CheckPositionIsInRange(long position) =>
            StartIndex <= position && StartIndex + (long)Segment.Length > position;
    }
}