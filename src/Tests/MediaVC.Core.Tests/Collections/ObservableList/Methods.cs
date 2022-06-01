using System;
using System.Reactive;

using Moq;

using Xunit;

namespace MediaVC.Core.Tests.Collections.ObservableList
{
    public class Methods
    {
        [Fact]
        public void Clear_ShouldClearListAndNotify()
        {
            var result = new ObservableList<bool>();
            var observableMock = new Mock<IObserver<Unit>>();
            observableMock.Setup(mocked => mocked.OnNext(It.IsAny<Unit>())).Verifiable();

            using(result.Cleared.Subscribe(observableMock.Object))
            {
                result.Add(true);
                result.Clear();
            }

            Assert.Empty(result);

            observableMock.Verify(mocked => mocked.OnNext(It.IsAny<Unit>()));
        }

        [Fact]
        public void Add_ShouldAddToListAndNotify()
        {
            var result = new ObservableList<int>();
            const int value = 1;

            var observableMock = new Mock<IObserver<int>>();
            observableMock.Setup(mocked => mocked.OnNext(It.IsAny<int>())).Verifiable();

            using(result.Added.Subscribe(observableMock.Object))
            {
                result.Add(value);
            }

            Assert.Equal(value, Assert.Single(result));

            observableMock.Verify(mocked => mocked.OnNext(value));
        }

        [Fact]
        public void RemoveAt_ShouldRemoveFromListAndNotify()
        {
            var result = new ObservableList<float>();
            const float value = 0.5f;

            var observableMock = new Mock<IObserver<float>>();
            observableMock.Setup(mocked => mocked.OnNext(It.IsAny<float>())).Verifiable();

            result.Add(value);

            using(result.Removed.Subscribe(observableMock.Object))
            {
                result.RemoveAt(0);
            }

            Assert.Empty(result);

            observableMock.Verify(mocked => mocked.OnNext(value));
        }

        [Fact]
        public void Remove_ShouldRemoveFromListAndNotify()
        {
            var result = new ObservableList<float>();
            const float value = 0.5f;

            var observableMock = new Mock<IObserver<float>>();
            observableMock.Setup(mocked => mocked.OnNext(It.IsAny<float>())).Verifiable();

            result.Add(value);

            using(result.Removed.Subscribe(observableMock.Object))
            {
                result.Remove(value);
            }

            Assert.Empty(result);

            observableMock.Verify(mocked => mocked.OnNext(value));
        }

        [Fact]
        public void CopyTo_ShouldCopyToNewArray()
        {
            var result = new ObservableList<bool>();

            result.Add(false);
            result.Add(true);

            var array = new bool[2];

            result.CopyTo(array, 0);

            for(byte index = 0; index < 2; ++index)
                Assert.Equal(result[index], array[index]);
        }
    }
}
