using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MediaVC.Difference
{
    /// <summary>
    /// Provides access to input source of data
    /// </summary>
    public interface IInputSource : IAsyncDisposable, IAsyncEnumerable<byte>
    {
        /// <summary>
        /// Current position in stream in bytes
        /// </summary>
        long Position { get; set; }

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        long Length { get; }

        /// <summary>
        /// Reads one byte.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Value</returns>
        /// <exception cref="OperationCanceledException" />
        /// <exception cref="InvalidOperationException" />
        ValueTask<byte> ReadByteAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///  When overridden in a derived class, reads a sequence of bytes from the current
        ///  stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified
        ///  byte array with the values between offset and (offset + count - 1) replaced by
        ///  the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read
        ///  from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>The total number of bytes read into the buffer. This can be less than the number
        ///  of bytes requested if that many bytes are not currently available, or zero (0)
        ///  if the end of the stream has been reached.</returns>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="System.IO.IOException" />
        int Read(byte[] buffer, int offset, int count);

        /// <summary>
        ///  When overridden in a derived class, reads a sequence of bytes from the current
        ///  stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified
        ///  byte array with the values between offset and (offset + count - 1) replaced by
        ///  the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read
        ///  from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>The total number of bytes read into the buffer. This can be less than the number
        ///  of bytes requested if that many bytes are not currently available, or zero (0)
        ///  if the end of the stream has been reached.</returns>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="System.IO.IOException" />
        ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);
    }
}
