using System.Threading;

using MediaVC.Difference;

using Moq;

using Xunit;

namespace MediaVC.Tools.Tests.Difference.DifferenceCalculator
{
    public class Properties
    {
        #region Methods

        [Fact]
        public void Result_ShouldGetList()
        {
            var result = new Tools.Difference.DifferenceCalculator(new Mock<IInputSource>().Object, new Mock<IInputSource>().Object);

            Assert.NotNull(result.Result);
            Assert.Empty(result.Result);
        }

        [Fact]
        public void SynchronizationContext_ShouldGetAndSet()
        {
            var result = new Tools.Difference.DifferenceCalculator(new Mock<IInputSource>().Object, new Mock<IInputSource>().Object);

            Assert.Null(result.SynchronizationContext);

            var value2 = new SynchronizationContext();
            result.SynchronizationContext = value2;

            Assert.Equal(value2, result.SynchronizationContext);
        }

        #endregion
    }
}
