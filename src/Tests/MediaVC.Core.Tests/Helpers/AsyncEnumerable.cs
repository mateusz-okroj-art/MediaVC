using System;
using System.Collections.Generic;
using System.Threading;

using Moq;

using Xunit;

namespace MediaVC.Core.Tests.Helpers
{
    public class AsyncEnumerable
    {
        [Fact]
        public void Constructor_WhenArgumentIsNotNull_ShouldReturnValid()
        {
            var mocked = Mock.Of<IAsyncEnumerator<object>>();
            var cancel = new CancellationTokenSource();

            var result = new AsyncEnumerable<object>(mocked);

            Assert.Equal(mocked, result.GetAsyncEnumerator());
            Assert.IsAssignableFrom<IAsyncEnumerator<object>>(result.GetAsyncEnumerator(cancel.Token));
        }

        [Fact]
        public void Constructor_WhenArgumentIsNull_ShouldThrow()
        {
            IAsyncEnumerator<object> argument = null;

            Assert.Throws<ArgumentNullException>(() => new AsyncEnumerable<object>(argument));
        }
    }
}
