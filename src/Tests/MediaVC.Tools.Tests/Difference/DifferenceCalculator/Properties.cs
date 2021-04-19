using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MediaVC.Difference.FileSegments;
using MediaVC.Tools.Tests.Fixtures;

using Xunit;

namespace MediaVC.Tools.Tests.Difference.DifferenceCalculator
{
    public class Properties : IClassFixture<DifferenceCalculatorTestFixture>
    {
        #region Fields

        private readonly DifferenceCalculatorTestFixture fixture;

        #endregion

        #region Constructor

        public Properties(DifferenceCalculatorTestFixture fixture)
        {
            this.fixture = fixture;
        }

        #endregion

        #region Methods

        [Fact]
        public void Result_ShouldGetList()
        {
            Assert.IsType<List<IFileSegmentInfo>>(fixture.Calculator.Result);
        }

        [Fact]
        public void SynchronizationContext_ShouldGetAndSet()
        {
            fixture.Calculator.SynchronizationContext = SynchronizationContext.Current;
            Assert.Equal(SynchronizationContext.Current, fixture.Calculator.SynchronizationContext);
        }

        #endregion
    }
}
