using System;
using System.IO;
using System.Threading.Tasks;

using MediaVC.Difference;
using MediaVC.Difference.Strategies;
using MediaVC.Tools.Tests.Fixtures;

namespace MediaVC.Tools.Tests
{
    public sealed class DifferenceCalculatorTestFixture : IDifferenceCalculatorTestFixture, IAsyncDisposable
    {
        #region Constructor

        public DifferenceCalculatorTestFixture()
        {
            ExampleSources = new InputSource[examplesCount];

            for(var index = 0; index < examplesCount; ++index)
            {
                this.examplesMemoryStreams[index] = new MemoryStream(this.dataCollection[index]);

                ExampleSources[index] = new InputSource(new StreamStrategy(this.examplesMemoryStreams[index]));
            }
        }

        #endregion

        #region Fields

        private const int examplesCount = 4;
        private readonly byte[][] dataCollection = new byte[][]
        {
            new byte[]
            {
                255, 255, 255, 255
            },
            new byte[]
            {
                255, 255, 255, 255,
                0, 0, 0, 0
            },
            new byte[]
            {
                0, 0, 0, 0,
                255, 255, 255, 255
            },
            new byte[]
            {
                255, 255,
                0,
                255, 255
            }
        };

        private readonly MemoryStream[] examplesMemoryStreams = new MemoryStream[examplesCount];

        #endregion

        #region Properties

        public IInputSource OneZero { get; } = new OneZeroByteReadonlyStream();

        public IInputSource ThousandFullBytes { get; } = new ThousandFullBytesReadonlyStream();

        public InputSource[] ExampleSources { get; }

        public async ValueTask DisposeAsync()
        {
            for(var index = 0; index < examplesCount; ++index)
            {
                await ExampleSources[index].DisposeAsync();
                await this.examplesMemoryStreams[index].DisposeAsync();
            }
        }

        #endregion
    }
}
