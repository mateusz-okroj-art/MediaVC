using MediaVC.Difference;

namespace MediaVC.Tools.Tests
{
    public interface IDifferenceCalculatorTestFixture
    {
        IInputSource OneZero { get; }

        IInputSource ThousandFullBytes { get; }
    }
}