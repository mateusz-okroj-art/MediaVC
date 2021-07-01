using System;

using MediaVC.Difference;

namespace MediaVC.Tools.Tests
{
    public interface IDifferenceCalculatorTestFixture : IDisposable
    {
        IInputSource OneZero { get; }
        IInputSource MFullBytes { get; }
    }
}