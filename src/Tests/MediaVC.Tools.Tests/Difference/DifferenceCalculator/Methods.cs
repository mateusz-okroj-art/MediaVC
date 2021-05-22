using System;
using Xunit;

namespace MediaVC.Tools.Tests.Difference.DifferenceCalculator
{
    public class Methods : IClassFixture<DifferenceCalculatorTestFixture>
    {
        #region Constructor

        public Methods(DifferenceCalculatorTestFixture fixture) =>
            this.fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));

        #endregion

        #region Fields

        private readonly IDifferenceCalculatorTestFixture fixture;

        #endregion

        #region Tests

        [Fact]
        public void Calculate_WhenNewFile_ShouldReturnOneSegment()
        {

        }

        #endregion
    }
}
