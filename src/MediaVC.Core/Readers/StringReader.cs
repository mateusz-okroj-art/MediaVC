using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MediaVC.Difference;
using MediaVC.Enumerators;
using MediaVC.Helpers;

namespace MediaVC.Readers
{
    /// <summary>
    /// Implements <see cref="TextReader"/> functions for reading from <see cref="IInputSource"/>.
    /// </summary>
    public sealed class StringReader : TextReader, IStringReader, IEquatable<StringReader>
    {
        /// <summary>
        /// Implements <see cref="TextReader"/> functions for reading from <see cref="IInputSource"/>.
        /// </summary>
        /// <param name="source">Source for text reading</param>
        /// <exception cref="ArgumentNullException" />
        public StringReader(IInputSource source)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            this.readingEngine = new TextReadingEngine(source);
        }

        private readonly IInputSource source;
        private readonly TextReadingEngine readingEngine;
        private readonly SynchronizationObject syncObject = new();

        #region Methods

        #region Overrides TextReader

        #region Obsoletes

        /// <exception cref="InvalidOperationException"/>
        [Obsolete]
        public override int Peek() => throw new InvalidOperationException();

        /// <exception cref="InvalidOperationException"/>
        [Obsolete]
        public override int ReadBlock(char[] buffer, int index, int count) => throw new InvalidOperationException();

        /// <exception cref="InvalidOperationException"/>
        [Obsolete]
        public override int ReadBlock(Span<char> buffer) => throw new InvalidOperationException();

        /// <exception cref="InvalidOperationException"/>
        [Obsolete]
        public override Task<int> ReadBlockAsync(char[] buffer, int index, int count) => throw new InvalidOperationException();

        /// <exception cref="InvalidOperationException"/>
        [Obsolete]
        public override ValueTask<int> ReadBlockAsync(Memory<char> buffer, CancellationToken cancellationToken = default) => throw new InvalidOperationException();

        #endregion

        public override int Read()
        {
            this.syncObject.WaitForAsync().AsTask().Wait();

            var result = this.readingEngine.ReadAsync().AsTask().GetAwaiter().GetResult();

            return result ?? -1;
        }

        public override int Read(char[] buffer, int index, int count) =>
            Read(buffer.AsSpan().Slice(index, count));

        public override int Read(Span<char> buffer)
        {
            this.syncObject.WaitForAsync().AsTask().Wait();

            var counter = 0;

            for(var i = 0; i < buffer.Length; ++i)
            {
                var result = this.readingEngine.ReadAsync().AsTask().GetAwaiter().GetResult();

                if(result.HasValue)
                    buffer[i] = result.Value;
                else
                    break;

                ++counter;
            }

            this.syncObject.Release();

            return counter;
        }

        public async ValueTask<char> ReadAsync(CancellationToken cancellationToken = default)
        {
            await this.syncObject.WaitForAsync(cancellationToken);

            var result = await this.readingEngine.ReadAsync(cancellationToken);

            this.syncObject.Release();

            return result ?? throw new InvalidOperationException();
        }

        public async override Task<int> ReadAsync(char[] buffer, int index, int count) =>
            await ReadAsync(buffer.AsMemory().Slice(index, count));

        public async override ValueTask<int> ReadAsync(Memory<char> buffer, CancellationToken cancellationToken = default)
        {
            await this.syncObject.WaitForAsync(cancellationToken);

            var counter = 0;

            for(var i = 0; i < buffer.Length; ++i)
            {
                var result = await this.readingEngine.ReadAsync(cancellationToken);

                if(result.HasValue)
                    buffer.Span[i] = result.Value;
                else
                    break;

                ++counter;
            }

            this.syncObject.Release();

            return counter;
        }

        public override Task<string?> ReadLineAsync() => ReadToEndInternalAsync(true);

        internal async Task<string?> ReadToEndInternalAsync(bool endOnLineEnding = false, CancellationToken cancellationToken = default)
        {
            await this.syncObject.WaitForAsync(cancellationToken);

            var stringBuilder = new StringBuilder();

            for(;;)
            {
                var result = await this.readingEngine.ReadAsync(cancellationToken);

                if(result.HasValue && !endOnLineEnding || result.HasValue && char.GetUnicodeCategory(result.Value) != UnicodeCategory.LineSeparator)
                    _ = stringBuilder.Append(result.Value);
                else
                    break;
            }

            this.syncObject.Release();

            return stringBuilder.Length > 0 ? stringBuilder.ToString() : null;
        }

        public override string? ReadLine() =>
            ReadLineAsync()
            .GetAwaiter()
            .GetResult();

        public override async Task<string> ReadToEndAsync() =>
            await ReadToEndInternalAsync() ?? throw new InvalidOperationException();

        public override string ReadToEnd() =>
            ReadToEndAsync()
            .GetAwaiter()
            .GetResult();

        #endregion

        public IAsyncEnumerable<string> Lines => new AsyncEnumerable<string>(new StringReaderEnumerator(this));

        public bool Equals(StringReader? other) => ReferenceEquals(this.source, other?.source);

        public override bool Equals(object? obj) => Equals(obj as StringBuilder);

        public override int GetHashCode() => this.source.GetHashCode();

        protected override void Dispose(bool disposing)
        {
            if(disposing)
                this.syncObject.Dispose();

            base.Dispose(disposing);
        }

        #endregion
    }
}
