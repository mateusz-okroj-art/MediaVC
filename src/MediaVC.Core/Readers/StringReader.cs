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

        #region Methods

        #region Overrides TextReader

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

        public override int Read()
        {

        }

        public override int Read(char[] buffer, int index, int count)
        {
            
        }

        public override int Read(Span<char> buffer)
        {

        }

        public ValueTask<char> ReadAsync(CancellationToken cancellationToken = default) { }

        public override Task<int> ReadAsync(char[] buffer, int index, int count)
        {

        }

        public override ValueTask<int> ReadAsync(Memory<char> buffer, CancellationToken cancellationToken = default)
        {

        }

        public override Task<string?> ReadLineAsync()
        {

        }

        public override string? ReadLine()
        {

        }

        #endregion

        public IAsyncEnumerable<string> Lines => new AsyncEnumerable<string>(new StringReaderEnumerator(this));

        public bool Equals(StringReader? other) => ReferenceEquals(this.source, other?.source);

        public override bool Equals(object? obj) => Equals(obj as StringBuilder);

        public override int GetHashCode() => this.source.GetHashCode();

        #endregion
    }
}
