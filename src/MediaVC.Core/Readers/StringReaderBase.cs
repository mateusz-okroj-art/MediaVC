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
    public abstract class StringReaderBase : IDisposable, IEquatable<StringReaderBase>, IEqualityComparer<StringReaderBase>
    {
        protected StringReaderBase(IInputSource source)
        {
            this.source = source;
            this.readingEngine = new TextReadingEngine(source);
        }

        internal readonly IInputSource source;
        private protected readonly TextReadingEngine readingEngine;
        protected readonly SynchronizationObject syncObject = new();

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

        internal async Task<string?> ReadToEndInternalAsync(bool endOnLineEnding = false, CancellationToken cancellationToken = default)
        {
            try
            {
                await this.syncObject.WaitForAsync(cancellationToken);

                var stringBuilder = new StringBuilder();

                var isFirstRune = new Ref<bool>(true);
                Rune? result;
                while(this.source.Position < this.source.Length && (result = await this.readingEngine.ReadAsync(cancellationToken)).HasValue)
                {
                    var resultRef = new Ref<Rune>(result.Value);
                    if(await ReadToEndInternalCoreAsync(endOnLineEnding, isFirstRune, stringBuilder, resultRef, cancellationToken))
                        break;
                }

                return isFirstRune && stringBuilder.Length < 1 ? null : stringBuilder.ToString();
            }
            finally
            {
                this.syncObject.Release();
            }
        }

        private async ValueTask<bool> DetectCRLFNewLineSeparatorAsync(Ref<Rune> result, long currentPosition, CancellationToken cancellationToken)
        {
            if(result == new Rune('\r'))
            {
                var nextRune = await this.readingEngine.ReadAsync(cancellationToken);
                if(nextRune.HasValue)
                {
                    if(nextRune.Value != new Rune('\n'))
                        this.source.Position = currentPosition;

                    return true;
                }
            }

            return false;
        }

        private async ValueTask<bool> DetectLFCRNewLineSeparatorAsync(Ref<Rune> result, long currentPosition, CancellationToken cancellationToken)
        {
            if(result == new Rune('\n'))
            {
                var nextRune = await this.readingEngine.ReadAsync(cancellationToken);
                if(nextRune.HasValue)
                {
                    if(nextRune.Value != new Rune('\r'))
                        this.source.Position = currentPosition;

                    return true;
                }
            }

            return false;
        }

        private async ValueTask<bool> ReadToEndInternalCoreAsync(bool endOnLineEnding, Ref<bool> isFirstRune, StringBuilder stringBuilder, Ref<Rune> result, CancellationToken cancellationToken)
        {
            var currentPosition = this.source.Position;

            if(endOnLineEnding)
            {
                if(await DetectLFCRNewLineSeparatorAsync(result, currentPosition, cancellationToken))
                    return true;

                if(await DetectCRLFNewLineSeparatorAsync(result, currentPosition, cancellationToken))
                    return true;
            }

            _ = stringBuilder.Append(result.Value.ToString());
            isFirstRune.Value = false;
            return false;
        }

        public void Dispose() => this.syncObject.Dispose();

        public override int GetHashCode() => this.source.GetHashCode();

        public bool Equals(StringReaderBase? other) => ReferenceEquals(this.source, other?.source);

        public override bool Equals(object? obj) => Equals(obj as StringReaderBase);

        public bool Equals(StringReaderBase? x, StringReaderBase? y) =>
            x is null && y is null || (x?.Equals(y) ?? false);

        public int GetHashCode([DisallowNull] StringReaderBase obj) => obj.GetHashCode();
    }
}