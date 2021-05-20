using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using MediaVC.Tools.Detection.Strategies;

namespace MediaVC.Tools.Detection
{
    /// <summary>
    /// Checks, that selected file is text-only
    /// </summary>
    public sealed class TextDetector : ITextDetector
    {
        #region Constructor

        /// <summary>
        /// Prepares detecting from Stream
        /// </summary>
        /// <param name="streamToDetect"></param>
        public TextDetector(Stream streamToDetect) =>
            this.strategy = new StreamTextDetectionStrategy(streamToDetect);

        /// <summary>
        /// Prepares detecting from buffer
        /// </summary>
        /// <param name="dataToDetect"></param>
        public TextDetector(ReadOnlyMemory<byte> dataToDetect) =>
            this.strategy = new MemoryTextDetectionStrategy(dataToDetect);

        #endregion

        #region Fields

        private readonly ITextDetector strategy;

        #endregion

        #region Methods

        public ValueTask<bool> CheckIsTextAsync(CancellationToken cancellationToken = default) =>
            this.strategy.CheckIsTextAsync(cancellationToken);

        #endregion
    }
}
