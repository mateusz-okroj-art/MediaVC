using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediaVC.Difference;
using MediaVC.Difference.FileSegments;

namespace MediaVC.Tools.Difference
{
    public interface IDifferenceCalculator
    {
        ValueTask CalculateAsync();

        IInputSource CurrentVersion { get; }

        IInputSource NewVersion { get; }

        IList<IFileSegmentInfo> Result { get; }
    }
}
