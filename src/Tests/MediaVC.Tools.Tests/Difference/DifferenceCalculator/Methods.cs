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
        public async void Calculate_WhenNewFile_ShouldReturnOneSegment()
        {
            var calculator = new Tools.Difference.DifferenceCalculator(this.fixture.OneZero);

            await calculator.CalculateAsync();

            Assert.Null(calculator.CurrentVersion);
            Assert.Equal(this.fixture.OneZero, calculator.NewVersion);

            Assert.NotNull(calculator.Result);
            Assert.Equal(1, calculator.Result.Count);

            Assert.Equal(this.fixture.OneZero, calculator.Result[0].Source);
            Assert.Equal(0, calculator.Result[0].StartPosition);
            Assert.Equal(0, calculator.Result[0].EndPosition);
            Assert.Equal(1U, calculator.Result[0].Length);
        }

        #endregion
    }
}
