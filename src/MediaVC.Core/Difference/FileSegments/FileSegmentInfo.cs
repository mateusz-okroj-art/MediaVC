namespace MediaVC.Difference
{
    public struct FileSegmentInfo : IFileSegmentInfo
    {
        public long StartPosition { get; set; }

        public long EndPosition { get; set; }

        public readonly ulong Length => IsPositionsValid() ? (ulong)(EndPosition - StartPosition + 1) : 0;

        public IInputSource Source { get; set; }

        private readonly bool IsPositionsValid() =>
            StartPosition >= 0 && StartPosition <= EndPosition && EndPosition >= StartPosition;
    }
}
