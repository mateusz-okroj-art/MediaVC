namespace MediaVC.Difference
{
    public interface IFileSegmentInfo
    {
        long StartPosition { get; set; }

        long EndPosition { get; set; }

        IInputSource Source { get; set; }

        ulong Length { get; }
    }
}
