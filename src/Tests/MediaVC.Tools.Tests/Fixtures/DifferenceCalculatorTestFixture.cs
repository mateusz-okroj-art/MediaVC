using MediaVC.Difference;
using MediaVC.Tools.Tests.Fixtures;

namespace MediaVC.Tools.Tests
{
    public sealed class DifferenceCalculatorTestFixture : IDifferenceCalculatorTestFixture
    {
        #region Fields

        private readonly byte[][] dataCollection = new byte[][]
        {

        };

        #endregion

        #region Properties

        public IInputSource OneZero { get; } = new OneZeroByteReadonlyStream();

        public IInputSource ThousandFullBytes { get; } = new ThousandFullBytesReadonlyStream();

        public 

        #endregion
    }
}
