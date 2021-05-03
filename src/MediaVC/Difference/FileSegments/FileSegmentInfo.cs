using System;

namespace MediaVC.Difference.FileSegments
{
    public struct FileSegmentInfo : IFileSegmentInfo
    {
        public long StartPosition { get; set; }

        public long EndPosition { get; set; }

        public ulong Length => (ulong)Math.Abs(EndPosition - StartPosition + 1);

        public IInputSource Source { get; set; }
    }
}
