using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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

        #endregion

        #region Fields

        private readonly ITextDetectionStrategy strategy;

        #endregion

        #region Methods

        public ValueTask<bool> CheckIsTextAsync() => this.strategy.CheckIsTextAsync();

        #endregion

        /// <summary>
        /// Checks, that selected enumerable is text-only
        /// </summary>
        /// <param name="input">Enumerable object to be checked</param>
        public static bool CheckIsText(IEnumerable<byte> input)
        {
            
    }
}
