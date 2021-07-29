using MediaVC.Difference;
using MediaVC.Tools.Tests.Fixtures;

namespace MediaVC.Tools.Tests
{
    public sealed class DifferenceCalculatorTestFixture : IDifferenceCalculatorTestFixture
    {
        #region Properties

        public IInputSource OneZero { get; } = new OneZeroByteReadonlyStream();

        public IInputSource ThousandFullBytes { get; } = new ThousandFullBytesReadonlyStream();

        #endregion
    }
}
