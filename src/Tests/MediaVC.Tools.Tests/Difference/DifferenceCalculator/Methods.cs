using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediaVC.Difference;

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
        public async Task Calculate_WhenCancellationRequested()
        {
            var calculator = new Tools.Difference.DifferenceCalculator(this.fixture.OneZero);
            var cancellationSource = new CancellationTokenSource();
            cancellationSource.Cancel();

            _ = await Assert.ThrowsAsync<OperationCanceledException>(() => calculator.CalculateAsync(cancellationSource.Token).AsTask());
        }

        [Fact]
        public async Task Calculate_WhenNewFile_Variant1_ShouldReturnOneSegment()
        {
            var calculator = new Tools.Difference.DifferenceCalculator(this.fixture.OneZero);
            var cancelationTokenSource = new CancellationTokenSource();

            if(!Debugger.IsAttached)
                cancelationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            await calculator.CalculateAsync(cancellationToken: cancelationTokenSource.Token);

            Assert.Null(calculator.CurrentVersion);
            Assert.True(ReferenceEquals(this.fixture.OneZero, calculator.NewVersion), "References is not equal.");

            Assert.NotNull(calculator.Result);
            var first = Assert.Single(calculator.Result);
            
            Assert.True(ReferenceEquals(this.fixture.OneZero, first.Source), "References is not equal.");
            Assert.Equal(0, first.StartPositionInSource);
            Assert.Equal(0, first.EndPositionInSource);
            Assert.Equal(1U, first.Length);

            Assert.NotNull(calculator.Removed);
            Assert.Empty(calculator.Removed);
        }

        [Fact]
        public async Task Calculate_WhenNewFile_Variant2_ShouldReturnOneSegment()
        {
            var calculator = new Tools.Difference.DifferenceCalculator(this.fixture.ThousandFullBytes);
            var cancelationTokenSource = new CancellationTokenSource();

            if(!Debugger.IsAttached)
                cancelationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            await calculator.CalculateAsync(cancellationToken: cancelationTokenSource.Token);

            Assert.Null(calculator.CurrentVersion);
            Assert.True(ReferenceEquals(this.fixture.ThousandFullBytes, calculator.NewVersion), "References is not equal.");

            Assert.NotNull(calculator.Result);
            var first = Assert.Single(calculator.Result);

            Assert.True(ReferenceEquals(this.fixture.ThousandFullBytes, first.Source), "References is not equal.");
            Assert.Equal(0, first.StartPositionInSource);
            Assert.Equal(this.fixture.ThousandFullBytes.Length - 1, first.EndPositionInSource);
            Assert.Equal((ulong)this.fixture.ThousandFullBytes.Length, first.Length);

            Assert.NotNull(calculator.Removed);
            Assert.Empty(calculator.Removed);
        }

        [Fact]
        public async Task Calculate_WhenNewFile_Variant3_ShouldReturnOneSegment()
        {
            var calculator = new Tools.Difference.DifferenceCalculator(InputSource.Empty, this.fixture.OneZero);
            var cancelationTokenSource = new CancellationTokenSource();

            if(!Debugger.IsAttached)
                cancelationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            await calculator.CalculateAsync(cancellationToken: cancelationTokenSource.Token);

            Assert.True(ReferenceEquals(InputSource.Empty, calculator.CurrentVersion), "References is not equal.");
            Assert.True(ReferenceEquals(this.fixture.OneZero, calculator.NewVersion), "References is not equal.");

            Assert.NotNull(calculator.Result);
            var first = Assert.Single(calculator.Result);

            Assert.True(ReferenceEquals(this.fixture.OneZero, first.Source), "References is not equal.");
            Assert.Equal(0, first.StartPositionInSource);
            Assert.Equal(this.fixture.OneZero.Length - 1, first.EndPositionInSource);
            Assert.Equal(this.fixture.OneZero.Length, (long)first.Length);

            Assert.NotNull(calculator.Removed);
            Assert.Empty(calculator.Removed);
        }

        [Fact]
        public async Task Calculate_WhenVersionEqual_Variant1_ShouldReturnOneSegment()
        {
            var calculator = new Tools.Difference.DifferenceCalculator(this.fixture.OneZero, this.fixture.OneZero);
            var cancelationTokenSource = new CancellationTokenSource();

            if(!Debugger.IsAttached)
                cancelationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            await calculator.CalculateAsync(cancellationToken: cancelationTokenSource.Token);

            Assert.True(ReferenceEquals(this.fixture.OneZero, calculator.CurrentVersion), "References is not equal.");
            Assert.True(ReferenceEquals(this.fixture.OneZero, calculator.NewVersion), "References is not equal.");

            Assert.NotNull(calculator.Result);
            var result = Assert.Single(calculator.Result);

            Assert.True(ReferenceEquals(this.fixture.OneZero, result.Source), "References is not equal.");
            Assert.Equal(0, result.StartPositionInSource);
            Assert.Equal(0, result.EndPositionInSource);
            Assert.Equal((ulong)1, result.Length);
            Assert.Equal(0, result.MappedPosition);

            Assert.NotNull(calculator.Removed);
            Assert.Empty(calculator.Removed);
        }

        [Fact]
        public async Task Calculate_WhenVersionEqual_Variant2_ShouldReturnOneSegment()
        {
            var calculator = new Tools.Difference.DifferenceCalculator(this.fixture.ThousandFullBytes, this.fixture.ThousandFullBytes);
            var cancelationTokenSource = new CancellationTokenSource();

            if(!Debugger.IsAttached)
                cancelationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            await calculator.CalculateAsync(cancellationToken: cancelationTokenSource.Token);

            Assert.True(ReferenceEquals(this.fixture.ThousandFullBytes, calculator.CurrentVersion), "References is not equal.");
            Assert.True(ReferenceEquals(this.fixture.ThousandFullBytes, calculator.NewVersion), "References is not equal.");

            Assert.NotNull(calculator.Result);
            var result = Assert.Single(calculator.Result);

            Assert.True(ReferenceEquals(this.fixture.ThousandFullBytes, result.Source), "References is not equal.");
            Assert.Equal(0, result.StartPositionInSource);
            Assert.Equal(this.fixture.ThousandFullBytes.Length - 1, result.EndPositionInSource);
            Assert.Equal((ulong)this.fixture.ThousandFullBytes.Length, result.Length);
            Assert.Equal(0, result.MappedPosition);

            Assert.NotNull(calculator.Removed);
            Assert.Empty(calculator.Removed);
        }

        [Fact]
        public async Task Calculate_WhenFileCleared_Variant1_ShouldReturnOneSegment()
        {
            var calculator = new Tools.Difference.DifferenceCalculator(this.fixture.ThousandFullBytes, InputSource.Empty);
            var cancelationTokenSource = new CancellationTokenSource();

            if(!Debugger.IsAttached)
                cancelationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            await calculator.CalculateAsync(cancellationToken: cancelationTokenSource.Token);

            Assert.NotNull(calculator.Result);
            Assert.Empty(calculator.Result);

            Assert.NotNull(calculator.Removed);
            var result = Assert.Single(calculator.Removed);

            Assert.True(ReferenceEquals(this.fixture.ThousandFullBytes, result.Source), "References is not equal.");
            Assert.Equal(0, result.StartPositionInSource);
            Assert.Equal(this.fixture.ThousandFullBytes.Length - 1, result.EndPositionInSource);
            Assert.Equal((ulong)this.fixture.ThousandFullBytes.Length, result.Length);
        }

        [Fact]
        public async Task Calculate_WhenFileIsDifferent_Variant1()
        {
            var calculator = new Tools.Difference.DifferenceCalculator(this.fixture.ExampleSources[0], this.fixture.ExampleSources[1]);
            var cancelationTokenSource = new CancellationTokenSource();

            if(!Debugger.IsAttached)
                cancelationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            await calculator.CalculateAsync(cancellationToken: cancelationTokenSource.Token);

            Assert.True(ReferenceEquals(this.fixture.ExampleSources[0], calculator.CurrentVersion), "References is not equal.");
            Assert.True(ReferenceEquals(this.fixture.ExampleSources[1], calculator.NewVersion), "References is not equal.");

            Assert.NotNull(calculator.Result);
            Assert.Equal(2, calculator.Result.Count());

            var result = calculator.Result.ElementAt(0);
            Assert.True(ReferenceEquals(this.fixture.ExampleSources[0], result.Source), "References is not equal.");
            Assert.Equal(0, result.StartPositionInSource);
            Assert.Equal(3L, result.EndPositionInSource);
            Assert.Equal(4L, (long)result.Length);

            result = calculator.Result.ElementAt(1);
            Assert.True(ReferenceEquals(this.fixture.ExampleSources[1], result.Source), "References is not equal.");
            Assert.Equal(4L, result.StartPositionInSource);
            Assert.Equal(7L, result.EndPositionInSource);
            Assert.Equal(4L, (long)result.Length);

            Assert.NotNull(calculator.Removed);
            Assert.Empty(calculator.Removed);
        }

        [Fact]
        public async Task Calculate_WhenFileIsDifferent_Variant2()
        {
            var calculator = new Tools.Difference.DifferenceCalculator(this.fixture.ExampleSources[0], this.fixture.ExampleSources[2]);
            var cancelationTokenSource = new CancellationTokenSource();

            if(!Debugger.IsAttached)
                cancelationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            await calculator.CalculateAsync(cancellationToken: cancelationTokenSource.Token);

            Assert.True(ReferenceEquals(this.fixture.ExampleSources[0], calculator.CurrentVersion), "References is not equal.");
            Assert.True(ReferenceEquals(this.fixture.ExampleSources[2], calculator.NewVersion), "References is not equal.");

            Assert.NotNull(calculator.Result);
            Assert.Equal(2, calculator.Result.Count());

            var result = calculator.Result.ElementAt(0);
            Assert.True(ReferenceEquals(calculator.NewVersion, result.Source), "References is not equal.");
            Assert.Equal(0L, result.StartPositionInSource);
            Assert.Equal(3L, result.EndPositionInSource);
            Assert.Equal(0, result.MappedPosition);
            Assert.Equal(4L, (long)result.Length);

            result = calculator.Result.ElementAt(1);
            Assert.True(ReferenceEquals(this.fixture.ExampleSources[0], result.Source), "References is not equal.");
            Assert.Equal(0, result.StartPositionInSource);
            Assert.Equal(3L, result.EndPositionInSource);
            Assert.Equal(4L, result.MappedPosition);
            Assert.Equal(4L, (long)result.Length);

            Assert.NotNull(calculator.Removed);
            Assert.Empty(calculator.Removed);
        }

        [Fact]
        public async Task Calculate_WhenFileIsDifferent_Variant3()
        {
            var calculator = new Tools.Difference.DifferenceCalculator(this.fixture.ExampleSources[1], this.fixture.ExampleSources[2]);
            var cancelationTokenSource = new CancellationTokenSource();

            if(!Debugger.IsAttached)
                cancelationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            await calculator.CalculateAsync(cancellationToken: cancelationTokenSource.Token);

            Assert.True(ReferenceEquals(this.fixture.ExampleSources[1], calculator.CurrentVersion), "References is not equal.");
            Assert.True(ReferenceEquals(this.fixture.ExampleSources[2], calculator.NewVersion), "References is not equal.");

            Assert.NotNull(calculator.Result);
            Assert.Equal(2, calculator.Result.Count());

            var result = calculator.Result.ElementAt(0);
            Assert.True(ReferenceEquals(this.fixture.ExampleSources[2], result.Source), "References is not equal.");
            Assert.Equal(0L, result.StartPositionInSource);
            Assert.Equal(3L, result.EndPositionInSource);
            Assert.Equal(0, result.MappedPosition);
            Assert.Equal(4L, (long)result.Length);

            result = calculator.Result.ElementAt(1);
            Assert.True(ReferenceEquals(this.fixture.ExampleSources[1], result.Source), "References is not equal.");
            Assert.Equal(0L, result.StartPositionInSource);
            Assert.Equal(3L, result.EndPositionInSource);
            Assert.Equal(4L, result.MappedPosition);
            Assert.Equal(4L, (long)result.Length);

            Assert.NotNull(calculator.Removed);

            result = Assert.Single(calculator.Removed);
            Assert.True(ReferenceEquals(this.fixture.ExampleSources[1], result.Source), "References is not equal.");
            Assert.Equal(4L, result.StartPositionInSource);
            Assert.Equal(7L, result.EndPositionInSource);
            Assert.Equal(4L, (long)result.Length);
        }

        [Fact]
        public async Task Calculate_WhenFileIsDifferent_Variant4()
        {
            var calculator = new Tools.Difference.DifferenceCalculator(this.fixture.ExampleSources[1], this.fixture.ExampleSources[3]);
            var cancelationTokenSource = new CancellationTokenSource();

            if(!Debugger.IsAttached)
                cancelationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            await calculator.CalculateAsync(cancellationToken: cancelationTokenSource.Token);

            Assert.True(ReferenceEquals(this.fixture.ExampleSources[1], calculator.CurrentVersion), "References is not equal.");
            Assert.True(ReferenceEquals(this.fixture.ExampleSources[3], calculator.NewVersion), "References is not equal.");

            Assert.NotNull(calculator.Result);
            Assert.Equal(3, calculator.Result.Count());

            var result = calculator.Result.ElementAt(0);
            Assert.True(ReferenceEquals(this.fixture.ExampleSources[1], result.Source), "References is not equal.");
            Assert.Equal(0L, result.StartPositionInSource);
            Assert.Equal(1L, result.EndPositionInSource);
            Assert.Equal(2L, (long)result.Length);
            Assert.Equal(0, result.MappedPosition);

            result = calculator.Result.ElementAt(1);
            Assert.True(ReferenceEquals(this.fixture.ExampleSources[3], result.Source), "References is not equal.");
            Assert.Equal(2L, result.StartPositionInSource);
            Assert.Equal(2L, result.EndPositionInSource);
            Assert.Equal(1L, (long)result.Length);
            Assert.Equal(2, result.MappedPosition);

            result = calculator.Result.ElementAt(2);
            Assert.True(ReferenceEquals(this.fixture.ExampleSources[1], result.Source), "References is not equal.");
            Assert.Equal(2L, result.StartPositionInSource);
            Assert.Equal(3L, result.EndPositionInSource);
            Assert.Equal(2L, (long)result.Length);
            Assert.Equal(3, result.MappedPosition);

            Assert.NotNull(calculator.Removed);

            result = Assert.Single(calculator.Removed);
            Assert.True(ReferenceEquals(this.fixture.ExampleSources[1], result.Source), "References is not equal.");
            Assert.Equal(4L, result.StartPositionInSource);
            Assert.Equal(7L, result.EndPositionInSource);
            Assert.Equal(4L, (long)result.Length);
        }

        #endregion
    }
}
