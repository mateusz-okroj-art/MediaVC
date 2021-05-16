using System;
using System.IO;
using System.Threading.Tasks;

using MediaVC.Tools.Detection.Strategies;

namespace MediaVC.Tools.Detection
{
    /// <summary>
    /// Checks, that selected file is text-only
    /// </summary>
    public sealed class TextDetector
    {
        #region Constructor

        public TextDetector(Stream streamToDetect) =>
            this.strategy = new StreamTextDetectionStrategy(streamToDetect);

        public TextDetector(Memory<byte> dataToDetect) =>
            this.strategy = new MemoryTextDetectionStrategy(dataToDetect);

        #endregion

        #region Fields

        private readonly ITextDetectionStrategy strategy;

        #endregion

        #region Methods

        public ValueTask<bool> CheckIsTextAsync() => this.strategy.CheckIsTextAsync();

        #endregion
    }
}
