using System;
using System.Diagnostics;

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
            var calculator = new Tools.Difference.DifferenceCalculator(this.fixture.ThousandFullBytes);

            await calculator.CalculateAsync();

            Assert.Null(calculator.CurrentVersion);
            Assert.Equal(this.fixture.ThousandFullBytes, calculator.NewVersion);

            Assert.NotNull(calculator.Result);
            Assert.Single(calculator.Result);

            Assert.Equal(this.fixture.ThousandFullBytes, calculator.Result[0].Source);
            Assert.Equal(0, calculator.Result[0].StartPosition);
            Assert.Equal(this.fixture.ThousandFullBytes.Length-1, calculator.Result[0].EndPosition);
            Assert.Equal((ulong)this.fixture.ThousandFullBytes.Length, calculator.Result[0].Length);
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
            var calculator = new Tools.Difference.DifferenceCalculator(this.fixture.ThousandFullBytes, this.fixture.ThousandFullBytes);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            await calculator.CalculateAsync();

            stopwatch.Stop();

            Assert.Equal(this.fixture.ThousandFullBytes, calculator.CurrentVersion);
            Assert.Equal(this.fixture.ThousandFullBytes, calculator.NewVersion);

            Assert.NotNull(calculator.Result);
            Assert.Empty(calculator.Result);

            Debug.WriteLine($"Calculating difference for 1000 bytes: {stopwatch.ElapsedMilliseconds} ms.");
        }

        #endregion
    }
}
