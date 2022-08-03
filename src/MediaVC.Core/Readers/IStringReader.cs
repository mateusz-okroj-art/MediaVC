using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text;

namespace MediaVC.Difference
{
    public interface IStringReader : IDisposable
    {
        IAsyncEnumerable<string?> Lines { get; }

        /// <summary>
        /// Reads the next character from the text reader and advances the character position by one character.
        /// </summary>
        /// <returns>
        ///   <para>The next character from the text reader, or <see langword="null"/> if no more characters are available.</para>
        /// </returns>
        /// <exception cref="IOException" />
        Rune? Read();

        /// <summary>
        /// Reads the characters from the current reader and writes the data to the specified buffer.
        /// </summary>
        /// <param name="buffer">When this method returns, contains the specified span of characters replaced by the characters read from the current source.</param>
        /// <returns>
        ///   <para>The number of characters that have been read. The number will be less than or equal to the buffer length, depending on whether the data is available within the reader.</para>
        ///   <para>This method returns 0 (zero) if it is called when no more characters are left to read.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="IOException" />
        int Read(Span<Rune> buffer);

        /// <summary>
        /// Asynchronously reads the characters from the current stream into a memory block.
        /// </summary>
        /// <param name="buffer">When this method returns, contains the specified memory block of characters replaced by the characters read from the current source.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is System.Threading.CancellationToken.None.</param>
        /// <returns>
        ///   <para>A value task that represents the asynchronous read operation.</para>
        ///   <para>The value of the type parameter contains the number of characters that have been read, or 0 if at the end of the stream and no data was read. The number will be less than or equal to the buffer length, depending on whether the data is available within the stream.</para>
        /// </returns>
        ValueTask<int> ReadAsync(Memory<char> buffer, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads the next character from the text reader and advances the character position by one character.
        /// </summary>
        /// <returns>
        ///   <para>The next character from the text reader, or <see langword="null"/> if no more characters are available.</para>
        /// </returns>
        /// <exception cref="IOException" />
        /// <exception cref="OperationCanceledException" />
        ValueTask<Rune?> ReadAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads a line of characters from the text reader and returns the data as a string.
        /// </summary>
        /// <returns>The next line from the reader, or null if all characters have been read.</returns>
        /// <exception cref="IOException" />
        /// <exception cref="OutOfMemoryException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        string? ReadLine();

        /// <summary>
        /// Reads a line of characters asynchronously and returns the data as a string.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation. The value of the TResult parameter contains the next line from the text reader, or is null if all of the characters have been read.</returns>
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        /// <exception cref="OperationCanceledException" />
        Task<string?> ReadLineAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads all characters from the current position to the end of the text reader and returns them as one string.
        /// </summary>
        /// <returns>A string that contains all characters from the current position to the end of the text reader.</returns>
        /// <exception cref="IOException" />
        /// <exception cref="OutOfMemoryException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        string ReadToEnd();

        /// <summary>
        /// Reads all characters from the current position to the end of the text reader asynchronously and returns them as one string.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation. The value of the TResult parameter contains a string with the characters from the current position to the end of the text reader.</returns>
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        /// <exception cref="OperationCanceledException" />
        Task<string> ReadToEndAsync(CancellationToken cancellationToken = default);
    }
}