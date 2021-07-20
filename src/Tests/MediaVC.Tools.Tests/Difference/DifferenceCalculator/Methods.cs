using System;

using MediaVC.Difference;

using Xunit;

namespace MediaVC.Tools.Tests.Difference.DifferenceCalculator
{
    public class Methods : IClassFixture<DifferenceCalculatorTestFixture>
    {
        #region Constructor

        public Methods(DifferenceCalculatorTestFixture fixture)
        {
            this.fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        #endregion

        #region Fields

        private readonly IDifferenceCalculatorTestFixture fixture;

        #endregion

        #region Tests

        [Fact]
        public async void Calculate_WhenNewFile_Variant1_ShouldReturnOneSegment()
        {
            var calculator = new Tools.Difference.DifferenceCalculator(this.fixture.OneZero);

            await calculator.CalculateAsync();

            Assert.Null(calculator.CurrentVersion);
            Assert.Equal(this.fixture.OneZero, calculator.NewVersion);

            Assert.NotNull(calculator.Result);
            Assert.Single(calculator.Result);

            Assert.Equal(this.fixture.OneZero, calculator.Result[0].Source);
            Assert.Equal(0, calculator.Result[0].StartPosition);
            Assert.Equal(0, calculator.Result[0].EndPosition);
            Assert.Equal(1U, calculator.Result[0].Length);
        }

        [Fact]
        public async void Calculate_WhenNewFile_Variant2_ShouldReturnOneSegment()
        {
            var calculator = new Tools.Difference.DifferenceCalculator(this.fixture.MFullBytes);

            await calculator.CalculateAsync();

            Assert.Null(calculator.CurrentVersion);
            Assert.Equal(this.fixture.MFullBytes, calculator.NewVersion);

            Assert.NotNull(calculator.Result);
            Assert.Single(calculator.Result);

            Assert.Equal(this.fixture.MFullBytes, calculator.Result[0].Source);
            Assert.Equal(0, calculator.Result[0].StartPosition);
            Assert.Equal(this.fixture.MFullBytes.Length-1, calculator.Result[0].EndPosition);
            Assert.Equal((ulong)this.fixture.MFullBytes.Length, calculator.Result[0].Length);
        }

        [Fact]
        public async void Calculate_WhenVersionEqual_Variant1_ShouldReturnEmpty()
        {
            var calculator = new Tools.Difference.DifferenceCalculator(this.fixture.OneZero, this.fixture.OneZero);

            await calculator.CalculateAsync();

            Assert.Equal(this.fixture.OneZero, calculator.CurrentVersion);
            Assert.Equal(this.fixture.OneZero, calculator.NewVersion);

            Assert.NotNull(calculator.Result);
            Assert.Empty(calculator.Result);
        }

        [Fact]
        public async void Calculate_WhenVersionEqual_Variant2_ShouldReturnEmpty()
        {
            var calculator = new Tools.Difference.DifferenceCalculator(this.fixture.MFullBytes, this.fixture.MFullBytes);

            await calculator.CalculateAsync();

            Assert.Equal(this.fixture.MFullBytes, calculator.CurrentVersion);
            Assert.Equal(this.fixture.MFullBytes, calculator.NewVersion);

            Assert.NotNull(calculator.Result);
            Assert.Empty(calculator.Result);
        }

        #endregion
    }
}
