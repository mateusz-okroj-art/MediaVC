using MediaVC.Difference.FileSegments;

namespace MediaVC.Difference.Strategies
{
    internal struct FileSegmentMappingInfo : IFileSegmentMappingInfo
    {
        public IFileSegmentInfo Segment { get; set; }

        public long StartIndex { get; set; }
    }
}
