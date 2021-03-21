using System;

namespace MediaVC.Difference.FileSegments
{
    public class FileSegmentInfo : IFileSegmentInfo
    {
        public long StartPosition { get; set; }

        public long EndPosition { get; set; }

        public ulong Length => (ulong)Math.Abs(EndPosition - StartPosition);

        public IInputSource Source { get; set; }
    }
}
