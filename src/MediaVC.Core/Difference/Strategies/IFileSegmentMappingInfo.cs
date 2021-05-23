namespace MediaVC.Difference.Strategies
{
    internal interface IFileSegmentMappingInfo
    {
        IFileSegmentInfo Segment { get; set; }

        long StartIndex { get; set; }

        bool CheckPositionIsInRange(long position);
    }
}