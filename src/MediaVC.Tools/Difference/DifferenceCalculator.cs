using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediaVC.Difference;
using MediaVC.Difference.FileSegments;

namespace MediaVC.Tools.Difference
{
    public class DifferenceCalculator : IDifferenceCalculator
    {
        public IInputSource CurrentVersion { get; }
        public IInputSource NewVersion { get; }
        public IList<IFileSegmentInfo> Result { get; }

        

        public ValueTask CalculateAsync()
        {
            throw new NotImplementedException();
        }
    }
}
