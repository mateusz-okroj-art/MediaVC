using System.Collections.Generic;

using MediaVC.Difference;

namespace MediaVC.Tools.Tests.Fixtures
{
    public interface IRemovedSegmentsCalculatorTestFixture
    {
        IInputSource InputSource1 { get; }
        IInputSource InputSource2 { get; }

        IEnumerable<IFileSegmentInfo> Test1_Segments { get; }
        IEnumerable<IFileSegmentInfo> Test2_Segments { get; }

        void Dispose();
    }
}