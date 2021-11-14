namespace MediaVC.Difference
{
    public struct FileSegmentInfo : IFileSegmentInfo
    {
        public long StartPositionInSource { get; set; }

        public long EndPositionInSource { get; set; }

        public long MappedPosition { get; set; }

        public readonly ulong Length => IsPositionsValid() ? (ulong)(EndPositionInSource - StartPositionInSource + 1) : 0;

        public IInputSource Source { get; set; }

        private readonly bool IsPositionsValid() =>
            StartPositionInSource >= 0 && StartPositionInSource <= EndPositionInSource && EndPositionInSource >= StartPositionInSource;
    }
}
