using System;

namespace MediaVC.Difference
{
    public struct FileSegmentInfo : IFileSegmentInfo
    {
        public long StartPosition { get; set; }

        public long EndPosition { get; set; }

        public ulong Length => IsPositionsValid() ? (ulong)(EndPosition - StartPosition + 1) : 0;

        public IInputSource Source { get; set; }

        private bool IsPositionsValid() =>
            StartPosition >= 0 && StartPosition <= EndPosition && EndPosition >= StartPosition;
    }
}
