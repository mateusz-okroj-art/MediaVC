using System;
using System.Diagnostics;
using System.Reactive;
using System.Threading.Tasks;

using MediaVC.Difference;

using Microsoft.Reactive.Testing;

using Moq;

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
        public async Task Calculate_WhenNewFile_Variant1_ShouldReturnOneSegment()
        {
            var calculator = new Tools.Difference.DifferenceCalculator(this.fixture.OneZero);

            await calculator.CalculateAsync();

            Assert.Null(calculator.CurrentVersion);
            Assert.Equal(this.fixture.OneZero, calculator.NewVersion);

            Assert.NotNull(calculator.Result);
            Assert.Single(calculator.Result);

            Assert.Equal(this.fixture.OneZero, calculator.Result[0].Source);
            Assert.Equal(0, calculator.Result[0].StartPositionInSource);
            Assert.Equal(0, calculator.Result[0].EndPositionInSource);
            Assert.Equal(1U, calculator.Result[0].Length);
        }

        [Fact]
        public async Task Calculate_WhenNewFile_Variant2_ShouldReturnOneSegment()
        {
            var calculator = new Tools.Difference.DifferenceCalculator(this.fixture.ThousandFullBytes);

            await calculator.CalculateAsync();

            Assert.Null(calculator.CurrentVersion);
            Assert.Equal(this.fixture.ThousandFullBytes, calculator.NewVersion);

            Assert.NotNull(calculator.Result);
            Assert.Single(calculator.Result);

            Assert.Equal(this.fixture.ThousandFullBytes, calculator.Result[0].Source);
            Assert.Equal(0, calculator.Result[0].StartPositionInSource);
            Assert.Equal(this.fixture.ThousandFullBytes.Length-1, calculator.Result[0].EndPositionInSource);
            Assert.Equal((ulong)this.fixture.ThousandFullBytes.Length, calculator.Result[0].Length);
        }

        [Fact]
        public async Task Calculate_WhenNewFile_Variant3_ShouldReturnOneSegment()
        {
            var calculator = new Tools.Difference.DifferenceCalculator(InputSource.Empty, this.fixture.OneZero);

            await calculator.CalculateAsync();

            Assert.Equal(InputSource.Empty, calculator.CurrentVersion);
            Assert.Equal(this.fixture.OneZero, calculator.NewVersion);

            Assert.NotNull(calculator.Result);
            Assert.Single(calculator.Result);

            Assert.Equal(this.fixture.OneZero, calculator.Result[0].Source);
            Assert.Equal(0, calculator.Result[0].StartPositionInSource);
            Assert.Equal(this.fixture.OneZero.Length-1, calculator.Result[0].EndPositionInSource);
            Assert.Equal(this.fixture.OneZero.Length, (long)calculator.Result[0].Length);
        }

        [Fact]
        public async Task Calculate_WhenVersionEqual_Variant1_ShouldReturnEmpty()
        {
            var calculator = new Tools.Difference.DifferenceCalculator(this.fixture.OneZero, this.fixture.OneZero);

            await calculator.CalculateAsync();

            Assert.Equal(this.fixture.OneZero, calculator.CurrentVersion);
            Assert.Equal(this.fixture.OneZero, calculator.NewVersion);

            Assert.NotNull(calculator.Result);
            Assert.Empty(calculator.Result);
        }

        [Fact]
        public async Task Calculate_WhenVersionEqual_Variant2_ShouldReturnEmpty()
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

        [Fact]
        public async Task Calculate_WhenFileCleared_Variant1_ShouldReturnEmpty()
        {
            var calculator = new Tools.Difference.DifferenceCalculator(this.fixture.ThousandFullBytes, InputSource.Empty);

            await calculator.CalculateAsync();

            Assert.NotNull(calculator.Result);
            Assert.Empty(calculator.Result);
        }

        [Fact]
        public async Task Calculate_WhenFileIsDifferent_Variant1()
        {
            var calculator = new Tools.Difference.DifferenceCalculator(this.fixture.ExampleSources[0], this.fixture.ExampleSources[1]);

            var observer1Mock = new Mock<IObserver<Unit>>(MockBehavior.Loose);
            observer1Mock.Setup(mocked => mocked.OnNext(It.IsAny<Unit>())).Verifiable();

            var observer2Mock = new Mock<IObserver<Unit>>(MockBehavior.Loose);
            observer2Mock.Setup(mocked => mocked.OnNext(It.IsAny<Unit>())).Verifiable();

            using(calculator.ResultCleared.Subscribe(observer1Mock.Object))
            {
                using(calculator.ResultAdded.Subscribe(observer2Mock.Object))
                {
                    await calculator.CalculateAsync();

                    Assert.Equal(this.fixture.ExampleSources[0], calculator.CurrentVersion);
                    Assert.Equal(this.fixture.ExampleSources[1], calculator.NewVersion);

                    Assert.NotNull(calculator.Result);
                    Assert.Equal(1, calculator.Result.Count);

                    var result = calculator.Result[0];

                    Assert.Equal(this.fixture.ExampleSources[1], result.Source);
                    Assert.Equal(4L, result.StartPosition);
                    Assert.Equal(7L, result.EndPosition);
                    Assert.Equal(4L, (long)result.Length);

                    observer1Mock.Verify(mocked => mocked.OnNext(It.IsAny<Unit>()));
                    observer1Mock.Verify(mocked => mocked.OnNext(It.IsAny<Unit>()));
                }
            }
        }

        [Fact]
        public async Task Calculate_WhenFileIsDifferent_Variant2()
        {
            var calculator = new Tools.Difference.DifferenceCalculator(this.fixture.ExampleSources[0], this.fixture.ExampleSources[2]);

            var observer1Mock = new Mock<IObserver<Unit>>(MockBehavior.Loose);
            observer1Mock.Setup(mocked => mocked.OnNext(It.IsAny<Unit>())).Verifiable();

            var observer2Mock = new Mock<IObserver<Unit>>(MockBehavior.Loose);
            observer2Mock.Setup(mocked => mocked.OnNext(It.IsAny<Unit>())).Verifiable();

            using(calculator.ResultCleared.Subscribe(observer1Mock.Object))
            {
                using(calculator.ResultAdded.Subscribe(observer2Mock.Object))
                {
                    await calculator.CalculateAsync();

                    Assert.Equal(this.fixture.ExampleSources[0], calculator.CurrentVersion);
                    Assert.Equal(this.fixture.ExampleSources[2], calculator.NewVersion);

                    Assert.NotNull(calculator.Result);
                    Assert.Equal(1, calculator.Result.Count);

                    var result = calculator.Result[0];

                    Assert.Equal(this.fixture.ExampleSources[2], result.Source);
                    Assert.Equal(4L, result.StartPosition);
                    Assert.Equal(7L, result.EndPosition);
                    Assert.Equal(4L, (long)result.Length);

                    observer1Mock.Verify(mocked => mocked.OnNext(It.IsAny<Unit>()));
                    observer1Mock.Verify(mocked => mocked.OnNext(It.IsAny<Unit>()));
                }
            }
        }

        [Fact]
        public async Task Calculate_WhenFileIsDifferent_Variant3()
        {
            var calculator = new Tools.Difference.DifferenceCalculator(this.fixture.ExampleSources[1], this.fixture.ExampleSources[2]);

            var observer1Mock = new Mock<IObserver<Unit>>(MockBehavior.Loose);
            observer1Mock.Setup(mocked => mocked.OnNext(It.IsAny<Unit>())).Verifiable();

            var observer2Mock = new Mock<IObserver<Unit>>(MockBehavior.Loose);
            observer2Mock.Setup(mocked => mocked.OnNext(It.IsAny<Unit>())).Verifiable();

            using(calculator.ResultCleared.Subscribe(observer1Mock.Object))
            {
                using(calculator.ResultAdded.Subscribe(observer2Mock.Object))
                {
                    await calculator.CalculateAsync();

                    Assert.Equal(this.fixture.ExampleSources[1], calculator.CurrentVersion);
                    Assert.Equal(this.fixture.ExampleSources[2], calculator.NewVersion);

                    Assert.NotNull(calculator.Result);
                    Assert.Equal(1, calculator.Result.Count);

                    var result = calculator.Result[0];

                    Assert.Equal(this.fixture.ExampleSources[2], result.Source);
                    Assert.Equal(0L, result.StartPosition);
                    Assert.Equal(7L, result.EndPosition);
                    Assert.Equal(8L, (long)result.Length);

                    observer1Mock.Verify(mocked => mocked.OnNext(It.IsAny<Unit>()));
                    observer1Mock.Verify(mocked => mocked.OnNext(It.IsAny<Unit>()));
                }
            }
        }

        /*[Fact] ENDLESS LOOP IN TESTED METHOD
        public async Task Calculate_WhenFileIsDifferent_Variant4()
        {
            var calculator = new Tools.Difference.DifferenceCalculator(this.fixture.ExampleSources[1], this.fixture.ExampleSources[3]);

            var observer1Mock = new Mock<IObserver<Unit>>(MockBehavior.Loose);
            observer1Mock.Setup(mocked => mocked.OnNext(It.IsAny<Unit>())).Verifiable();

            var observer2Mock = new Mock<IObserver<Unit>>(MockBehavior.Loose);
            observer2Mock.Setup(mocked => mocked.OnNext(It.IsAny<Unit>())).Verifiable();

            using(calculator.ResultCleared.Subscribe(observer1Mock.Object))
            {
                using(calculator.ResultAdded.Subscribe(observer2Mock.Object))
                {
                    await calculator.CalculateAsync();

                    Assert.Equal(this.fixture.ExampleSources[1], calculator.CurrentVersion);
                    Assert.Equal(this.fixture.ExampleSources[3], calculator.NewVersion);

                    Assert.NotNull(calculator.Result);
                    Assert.Equal(1, calculator.Result.Count);

                    var result = calculator.Result[0];

                    Assert.Equal(this.fixture.ExampleSources[2], result.Source);
                    Assert.Equal(0L, result.StartPosition);
                    Assert.Equal(7L, result.EndPosition);
                    Assert.Equal(8L, (long)result.Length);

                    observer1Mock.Verify(mocked => mocked.OnNext(It.IsAny<Unit>()));
                    observer1Mock.Verify(mocked => mocked.OnNext(It.IsAny<Unit>()));
                }
            }
        }*/

        #endregion
    }
}
