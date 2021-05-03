using System;

using MediaVC.Tools.Tests.Fixtures;

using Xunit;

namespace MediaVC.Tools.Tests.Difference.RemovedSegmentsCalculator
{
    public class Methods : IClassFixture<RemovedSegmentsCalculatorTestFixture>
    {
        #region Constructor

        public Methods(RemovedSegmentsCalculatorTestFixture fixture) =>
            this.fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));

        #endregion

        #region Fields

        private readonly IRemovedSegmentsCalculatorTestFixture fixture;

        #endregion

        #region Tests

        [Fact]
        public void Calculate_WhenSegmentsHaveOneWithSource_ShouldReturnEmpty()
        {
            var result = new Tools.Difference.RemovedSegmentsCalculator(this.fixture.Test1_Segments, this.fixture.InputSource1);

            result.Calculate();

            Assert.Empty(result.Result);
        }

        [Fact]
        public void Calculate_WhenSegmentsHaveOneWithoutSource_ShouldReturnFullSource()
        {
            var result = new Tools.Difference.RemovedSegmentsCalculator(this.fixture.Test1_Segments, this.fixture.InputSource2);

            result.Calculate();

            Assert.Single(result.Result);
            Assert.Equal(this.fixture.InputSource2, result.Result[0].Source);
            Assert.Equal(0, result.Result[0].StartPosition);
            Assert.Equal(4, (long)result.Result[0].Length);
        }

        [Fact]
        public void Calculate_WhenSegmentsHaveHalfOfSource_Var1_ShouldReturnWithCurrentSource()
        {
            var result = new Tools.Difference.RemovedSegmentsCalculator(this.fixture.Test2_Segments, this.fixture.InputSource1);

            result.Calculate();

            Assert.Single(result.Result);
            Assert.Equal(this.fixture.InputSource1, result.Result[0].Source);
            Assert.Equal(2, result.Result[0].StartPosition);
            Assert.Equal(3, result.Result[0].EndPosition);
        }

        [Fact]
        public void Calculate_WhenSegmentsHaveHalfOfSource_Var2_ShouldReturnWithCurrentSource()
        {
            var result = new Tools.Difference.RemovedSegmentsCalculator(this.fixture.Test2_Segments, this.fixture.InputSource2);

            result.Calculate();

            Assert.Single(result.Result);
            Assert.Equal(this.fixture.InputSource2, result.Result[0].Source);
            Assert.Equal(0, result.Result[0].StartPosition);
            Assert.Equal(1, result.Result[0].EndPosition);
        }

        #endregion
    }
}
