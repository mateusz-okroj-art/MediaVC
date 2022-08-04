using System;
using System.Security.Cryptography;

using Moq;

using Xunit;

namespace MediaVC.Core.Tests.Helpers
{
    public class Ref
    {
        [Fact]
        public void Constructor_ShouldSetValueAndNotifySubscribers()
        {
            var argument = RandomNumberGenerator.GetInt32(1, int.MaxValue);

            var result = new Ref<int>(argument);

            Assert.Equal(argument, result.Value);
        }

        [Fact]
        public void Value_WhenSet_ShouldStoreValueAndNotifyObservers()
        {
            var result = new Ref<int>();

            var argument = RandomNumberGenerator.GetInt32(1, int.MaxValue);

            var observerMock = new Mock<IObserver<int?>>(MockBehavior.Strict);
            observerMock.Setup(mocked => mocked.OnNext(argument))
                .Verifiable();

            using(result.Changed.Subscribe(observerMock.Object))
            {
                result.Value = argument;
                Assert.Equal(argument, result.Value);
                observerMock.VerifyAll();
            }
        }
    }
}
