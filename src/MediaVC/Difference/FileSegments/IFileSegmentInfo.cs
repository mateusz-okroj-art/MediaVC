using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaVC.Difference.FileSegments
{
    public interface IFileSegmentInfo
    {
        long StartPosition { get; set; }

        long EndPosition { get; set; }

        IInputSource Source { get; set; }

        ulong Length { get; }
    }
}
