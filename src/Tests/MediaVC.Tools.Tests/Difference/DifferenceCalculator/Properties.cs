using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xunit;
using Moq;
using MediaVC.Difference;
using MediaVC.Difference.FileSegments;

namespace MediaVC.Tools.Tests.Difference.DifferenceCalculator
{
    public class Properties
    {
        #region Methods

        [Fact]
        public void Result_ShouldGetList()
        {
            var result = new Tools.Difference.DifferenceCalculator(new Mock<IInputSource>().Object, new Mock<IInputSource>().Object);

            Assert.IsType<List<IFileSegmentInfo>>(result.Result);
            Assert.Empty(result.Result);
        }

        [Fact]
        public void SynchronizationContext_ShouldGetAndSet()
        {
            var result = new Tools.Difference.DifferenceCalculator(new Mock<IInputSource>().Object, new Mock<IInputSource>().Object);

            Assert.Equal(SynchronizationContext.Current, result.SynchronizationContext);
        }

        #endregion
    }
}
