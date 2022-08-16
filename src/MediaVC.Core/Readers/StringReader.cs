using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MediaVC.Difference;
using MediaVC.Enumerators;

namespace MediaVC.Readers
{
    /// <summary>
    /// Implements <see cref="TextReader"/> functions for reading from <see cref="IInputSource"/>.
    /// </summary>
    public sealed class StringReader : StringReaderBase, IStringReader, IDisposable, IEquatable<StringReader>
    {
        /// <summary>
        /// Implements <see cref="TextReader"/> functions for reading from <see cref="IInputSource"/>.
        /// </summary>
        /// <param name="source">Source for text reading</param>
        /// <exception cref="ArgumentNullException" />
        public StringReader(IInputSource source) : base(source ?? throw new ArgumentNullException(nameof(source)))
        {}

        public IAsyncEnumerable<string?> Lines => new AsyncEnumerable<string?>(new StringReaderEnumerator(this));

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

                    buffer[counter] = readedValue.Value;
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

        public async Task<string?> ReadLineAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await this.syncObject.WaitForAsync(cancellationToken);

                var result = await ReadLineCoreAsync(cancellationToken);

                if((result?.Length ?? 0) == 0 && this.source.Position == this.source.Length && this.isLineSeparatorLastReaded)
                {
                    this.isLineSeparatorLastReaded = false;
                    return string.Empty;
                }
                else
                {
                    return (this.source.Position == this.source.Length && (result?.Length ?? 0) == 0) ? null : result;
                }
            }
            finally
            {
                this.syncObject.Release();
            }
        }

        public string? ReadLine() => ReadLineAsync()
                                        .GetAwaiter()
                                        .GetResult();

        public async Task<string> ReadToEndAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await ReadToEndInternalAsync(cancellationToken) ?? throw new InvalidOperationException();
            }
            catch(TaskCanceledException exc)
            {
                throw new OperationCanceledException("Operation was canceled.", exc, cancellationToken);
            }
        }

        public string ReadToEnd() =>
            ReadToEndAsync()
            .GetAwaiter()
            .GetResult();

        internal void Reset() => this.readingEngine.Reset();

        void IDisposable.Dispose() => Dispose(true);

        public bool Equals(StringReader? other) => base.Equals(other);

        public override bool Equals(object? obj) => Equals(obj as StringReader);

        public override int GetHashCode() => this.source.GetHashCode();

        #endregion
    }
}
