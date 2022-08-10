using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MediaVC.Difference;
using MediaVC.Helpers;

namespace MediaVC.Readers
{
    /// <summary>
    /// Base class for <see cref="StringReader"/>.
    /// </summary>
    public abstract class StringReaderBase : IDisposable
    {
        protected StringReaderBase(IInputSource source)
        {
            this.source = source;
            this.readingEngine = new TextReadingEngine(source);
        }

        internal readonly IInputSource source;
        internal readonly TextReadingEngine readingEngine;
        protected readonly SynchronizationObject syncObject = new();
        private bool disposedValue;
        private bool detectedLineSeparator = false;

        /// <summary>
        /// Represents last reading state
        /// </summary>
        public TextReadingState LastReadingState => this.readingEngine.LastReadingState;

        public Encoding? SelectedEncoding
        {
            get => this.readingEngine.SelectedEncoding;
            set => this.readingEngine.SelectedEncoding = value;
        }

        public LineEnding LineEnding => this.readingEngine.LineEnding;

        protected async Task<string?> ReadLineCoreAsync(CancellationToken cancellation = default)
        {
            //try
            //{
            //    await this.syncObject.WaitForAsync(cancellation);

            //    var stringBuilder = new StringBuilder();

            //    Rune? result;
            //    while(this.source.Position < this.source.Length)
            //    {
            //        result = await this.readingEngine.ReadAsync(cancellation);

            //        if(!result.HasValue)
            //        {
            //            return this.source.Position == 0 ? null : stringBuilder.ToString();
            //        }

            //        var detectionResult = await DetectLFCRNewLineSeparatorAsync(currentRune, currentPosition, cancellationToken);

            //        if(detectionResult == LineSeparatorDetectionResult.LastEmptyLine)
            //            this.detectedEmptyLastLine = true;

            //        if(detectionResult != LineSeparatorDetectionResult.NotDetected)
            //            return true;

            //        detectionResult = await DetectCRLFNewLineSeparatorAsync(currentRune, currentPosition, cancellationToken);

            //        if(detectionResult == LineSeparatorDetectionResult.LastEmptyLine)
            //            this.detectedEmptyLastLine = true;

            //        if(detectionResult != LineSeparatorDetectionResult.NotDetected)
            //            return true;
            //    }
            //}
            //finally
            //{
            //    this.syncObject.Release();
            //}
            throw new NotImplementedException();//TODO
        }

        internal async Task<string?> ReadToEndInternalAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await this.syncObject.WaitForAsync(cancellationToken);

                var stringBuilder = new StringBuilder();

                var isFirstRune = true;
                Rune? result;
                while(this.source.Position < this.source.Length)
                {
                    result = await this.readingEngine.ReadAsync(cancellationToken);

                    if(result is null)
                    {
                        return isFirstRune ? null : stringBuilder.ToString();
                    }

                    stringBuilder.Append(result.ToString());

                    isFirstRune = false;
                }

                return stringBuilder.ToString();
            }
            finally
            {
                this.syncObject.Release();
            }
        }

        private async ValueTask<LineSeparatorDetectionResult> DetectCRLFNewLineSeparatorAsync(Rune result, long currentPosition, CancellationToken cancellationToken)
        {
            if(result == new Rune('\r'))
            {
                var nextRune = await this.readingEngine.ReadAsync(cancellationToken);
                if(nextRune.HasValue)
                {
                    if(nextRune.Value != new Rune('\n'))
                    {
                        this.source.Position = currentPosition;
                        return LineSeparatorDetectionResult.FullDetected;
                    }

                    return LineSeparatorDetectionResult.HalfDetected;
                }

                return LineSeparatorDetectionResult.LastEmptyLine;
            }

            return LineSeparatorDetectionResult.NotDetected;
        }

        private async ValueTask<LineSeparatorDetectionResult> DetectLFCRNewLineSeparatorAsync(Rune result, long currentPosition, CancellationToken cancellationToken)
        {
            if(result == new Rune('\n'))
            {
                var nextRune = await this.readingEngine.ReadAsync(cancellationToken);
                if(nextRune.HasValue)
                {
                    if(nextRune.Value != new Rune('\r'))
                    {
                        this.source.Position = currentPosition;
                        return LineSeparatorDetectionResult.FullDetected;
                    }

                    return LineSeparatorDetectionResult.HalfDetected;
                }

                return LineSeparatorDetectionResult.LastEmptyLine;
            }

            return LineSeparatorDetectionResult.NotDetected;
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!disposedValue)
            {
                if(disposing)
                {
                    this.syncObject.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}