using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace MediaVC.Difference
{
    public interface IStringReader : IDisposable
    {
        IAsyncEnumerable<string> Lines { get; }

        /// <summary>
        /// Reads the next character from the text reader and advances the character position by one character.
        /// </summary>
        /// <returns>
        ///   <para>The next character from the text reader, or -1 if no more characters are available.</para>
        ///   <para>The default implementation returns -1.</para>
        /// </returns>
        /// <exception cref="IOException" />
        int Read();

        /// <summary>
        /// Reads a specified maximum number of characters from the current reader and writes the data to a buffer, beginning at the specified index.
        /// </summary>
        /// <param name="buffer">When this method returns, contains the specified character array with the values between index and (index + count - 1) replaced by the characters read from the current source.</param>
        /// <param name="index">The position in buffer at which to begin writing.</param>
        /// <param name="count">The maximum number of characters to read. If the end of the reader is reached before the specified number of characters is read into the buffer, the method returns.</param>
        /// <returns>
        ///   <para>The number of characters that have been read. The number will be less than or equal to count, depending on whether the data is available within the reader.</para>
        ///   <para>This method returns 0 (zero) if it is called when no more characters are left to read.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="IOException" />
        int Read(char[] buffer, int index, int count);

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
        int Read(Span<char> buffer);

        /// <summary>
        /// Reads a specified maximum number of characters from the current text reader asynchronously and writes the data to a buffer, beginning at the specified index.
        /// </summary>
        /// <param name="buffer">When this method returns, contains the specified character array with the values between index and (index + count - 1) replaced by the characters read from the current source.</param>
        /// <param name="index">The position in buffer at which to begin writing.</param>
        /// <param name="count">The maximum number of characters to read. If the end of the text is reached before the specified number of characters is read into the buffer, the current method returns.</param>
        /// <returns>
        ///   <para>A task that represents the asynchronous read operation. The value of the int parameter contains the total number of bytes read into the buffer.</para>
        ///   <para>The result value can be less than the number of bytes requested if the number of bytes currently available is less than the requested number, or it can be 0 (zero) if the end of the text has been reached.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="IOException" />
        Task<int> ReadAsync(char[] buffer, int index, int count);

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
        //
        // Podsumowanie:
        //     Reads a line of characters asynchronously and returns the data as a string.
        //
        // Zwraca:
        //     A task that represents the asynchronous read operation. The value of the TResult
        //     parameter contains the next line from the text reader, or is null if all of the
        //     characters have been read.
        //
        // Wyjątki:
        //   T:System.ArgumentOutOfRangeException:
        //     The number of characters in the next line is larger than System.Int32.MaxValue.
        //
        //   T:System.ObjectDisposedException:
        //     The text reader has been disposed.
        //
        //   T:System.InvalidOperationException:
        //     The reader is currently in use by a previous read operation.
        Task<string?> ReadLineAsync();

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
        Task<string> ReadToEndAsync();
    }
}