namespace MediaVC.Difference
{
    public interface IFileSegmentInfo
    {
        long StartPositionInSource { get; set; }

        long EndPositionInSource { get; set; }

        IInputSource Source { get; set; }

        ulong Length { get; }

        long MappedPosition { get; set; }
    }
}
