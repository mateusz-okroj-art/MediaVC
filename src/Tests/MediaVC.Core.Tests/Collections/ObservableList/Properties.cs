using Xunit;

namespace MediaVC.Core.Tests.Collections.ObservableList
{
    public class Properties
    {
        [Fact]
        public void Locker_ShouldReadWrite()
        {
            var result = new ObservableList<object>();

            Assert.IsType<object>(result.Locker);

            var newValue = new object();
            result.Locker = newValue;

            Assert.Equal(newValue, result.Locker);
        }

        [Fact]
        public void IsReadonly_ShouldReturnFalse()
        {
            var result = new ObservableList<object>();

            Assert.False(result.IsReadOnly);
        }
    }
}
