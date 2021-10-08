using MediaVC.Difference;
using MediaVC.Tools.Tests.Fixtures;

namespace MediaVC.Tools.Tests
{
    public sealed class DifferenceCalculatorTestFixture : IDifferenceCalculatorTestFixture
    {
        #region Constructor

        public DifferenceCalculatorTestFixture()
        {
            ExampleSources = new IInputSource[4];

            new InputSource()
        }

        #endregion

        #region Fields

        private readonly byte[][] dataCollection = new byte[][]
        {
            new byte[]
            {
                255, 255, 255, 255
            },
            new byte[]
            {
                255, 255, 255, 255,
                0, 0, 0, 0
            },
            new byte[]
            {
                0, 0, 0, 0,
                255, 255, 255, 255
            },
            new byte[]
            {
                255, 255,
                0,
                255, 255
            }
        };

        #endregion

        #region Properties

        public IInputSource OneZero { get; } = new OneZeroByteReadonlyStream();

        public IInputSource ThousandFullBytes { get; } = new ThousandFullBytesReadonlyStream();

        public IInputSource[] ExampleSources { get; }

        #endregion
    }
}
