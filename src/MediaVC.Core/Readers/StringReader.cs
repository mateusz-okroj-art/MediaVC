using System;
using System.Collections.Generic;
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
    public sealed class StringReader : IStringReader, IEquatable<StringReader>
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

        #region Fields

        internal readonly IInputSource source;
        private readonly TextReadingEngine readingEngine;
        private readonly SynchronizationObject syncObject = new();

        #endregion

        #region Properties

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

        #endregion

        #region Methods

        public Rune? Read()
        {
            try
            {
                this.syncObject.WaitForAsync().AsTask().Wait();

                return this.readingEngine.ReadAsync().AsTask().GetAwaiter().GetResult();
            }
            finally
            {
                this.syncObject.Release();
            }
        }

        public int Read(Span<Rune> buffer)
        {
            try
            {
                this.syncObject.WaitForAsync().AsTask().Wait();

                var counter = 0;

                for(; counter < buffer.Length; ++counter)
                {
                    var readedValue = this.readingEngine.ReadAsync().AsTask().GetAwaiter().GetResult();

                    if(!readedValue.HasValue)
                        break;
                }

                return counter;
            }
            finally
            {
                this.syncObject.Release();
            }
        }

        public async ValueTask<int> ReadAsync(Memory<char> buffer, CancellationToken cancellationToken = default)
        {
            try
            {
                await this.syncObject.WaitForAsync(cancellationToken);

                var counter = 0;
                for(; counter < buffer.Length; ++counter)
                {
                    var readedValue = await this.readingEngine.ReadAsync(cancellationToken);

                    if(!readedValue.HasValue)
                        break;
                }

                return counter;
            }
            finally
            {
                this.syncObject.Release();
            }
        }

        public async ValueTask<Rune?> ReadAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await this.syncObject.WaitForAsync(cancellationToken);

                return await this.readingEngine.ReadAsync(cancellationToken);
            }
            finally
            {
                this.syncObject.Release();
            }
        }

        public Task<string?> ReadLineAsync() => ReadToEndInternalAsync(true);

        internal async Task<string?> ReadToEndInternalAsync(bool endOnLineEnding = false, CancellationToken cancellationToken = default)
        {
            await this.syncObject.WaitForAsync(cancellationToken);

            var stringBuilder = new StringBuilder();

            for(;;)
            {
                if(endOnLineEnding)
                {
                    if(await this.readingEngine.TryReadLineSeparator(cancellationToken))
                        break;
                }
                var result = await this.readingEngine.ReadAsync(cancellationToken);

                if(result.HasValue)
                    _ = stringBuilder.Append(result.Value.ToString());
                else
                    break;
            }

            this.syncObject.Release();

            return stringBuilder.Length > 0 ? stringBuilder.ToString() : null;
        }

        public string? ReadLine() =>
            ReadLineAsync()
            .GetAwaiter()
            .GetResult();

        public async Task<string> ReadToEndAsync() =>
            await ReadToEndInternalAsync() ?? throw new InvalidOperationException();

        public string ReadToEnd() =>
            ReadToEndAsync()
            .GetAwaiter()
            .GetResult();

        public IAsyncEnumerable<string> Lines => new AsyncEnumerable<string>(new StringReaderEnumerator(this));

        public bool Equals(StringReader? other) => ReferenceEquals(this.source, other?.source);

        public override bool Equals(object? obj) => Equals(obj as StringReader);

        public override int GetHashCode() => this.source.GetHashCode();

        public void Dispose() => this.syncObject.Dispose();

        #endregion
    }
}
