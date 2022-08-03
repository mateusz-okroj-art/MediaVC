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
    public sealed class StringReader : StringReaderBase, IStringReader
    {
        /// <summary>
        /// Implements <see cref="TextReader"/> functions for reading from <see cref="IInputSource"/>.
        /// </summary>
        /// <param name="source">Source for text reading</param>
        /// <exception cref="ArgumentNullException" />
        public StringReader(IInputSource source) : base(source ?? throw new ArgumentNullException(nameof(source)))
        {}

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

        public Task<string?> ReadLineAsync(CancellationToken cancellationToken = default) =>
            ReadToEndInternalAsync(true, cancellationToken);

        public string? ReadLine() =>
            ReadLineAsync()
            .GetAwaiter()
            .GetResult();

        public async Task<string> ReadToEndAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await ReadToEndInternalAsync(false, cancellationToken) ?? throw new InvalidOperationException();
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

        public IAsyncEnumerable<string?> Lines => new AsyncEnumerable<string?>(new StringReaderEnumerator(this));

        #endregion
    }
}
