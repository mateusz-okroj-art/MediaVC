using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace MediaVC.Tools
{
    /// <summary>
    /// Checks, that selected file is text-only
    /// </summary>
    public static class TextDetector
    {
        /// <summary>
        /// Checks, that selected enumerable is text-only
        /// </summary>
        /// <param name="input">Enumerable object to be checked</param>
        public static bool CheckIsText(IEnumerable<byte> input)
        {
            var length = input?.Count();

            if (length < 1)
                return false;
            else if (length < 10_000)
                foreach (var value in input)
                {
                    if (!CheckSingleCharacter(value))
                        return false;
                }
            else
            {
                var stopped = false;
                var locker = new object();

                Parallel.ForEach(input, (value, state) =>
                {
                    if(!CheckSingleCharacter(value))
                    {
                        lock (locker) stopped = true;

                        state.Break();
                    }
                });

                return stopped;
            }

            return true;
        }

        public static bool CheckIsText(Memory<byte> input) => CheckIsText(MemoryMarshal.ToEnumerable<byte>(input));

        public static bool CheckIsText(ReadOnlyMemory<byte> input) => CheckIsText(MemoryMarshal.ToEnumerable(input));

        /// <summary>
        /// Checks, that selected bytes is text-only
        /// </summary>
        /// <param name="input">Memory block to be checked</param>
        public static bool CheckIsText(Span<byte> input) => CheckIsText((ReadOnlySpan<byte>)input);

        public static bool CheckIsText(ReadOnlySpan<byte> input)
        {
            var length = input.Length;

            if (length < 1)
                return false;
            else
                foreach (var value in input)
                {
                    if (!CheckSingleCharacter(value))
                        return false;
                }

            return true;
        }

        /// <summary>
        /// Checks, that selected stream is text-only
        /// </summary>
        public static bool CheckIsText(Stream input) => CheckIsTextAsync(input).Result;

        /// <summary>
        /// Checks, that selected stream is text-only
        /// </summary>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="IOException" />
        /// <exception cref="OperationCanceledException" />
        public static async ValueTask<bool> CheckIsTextAsync(Stream input, CancellationToken cancellationToken = default)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (!input.CanRead)
                throw new IOException("Cannot read.");

            if (input.Length < 1)
                return false;
            else
            {
                try
                {
                    input.Position = 0;
                    int value = -1;

                    while((value = input.ReadByte()) != -1)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        if (!CheckSingleCharacter((byte)value))
                            return false;
                    }
                }
                catch(IOException)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Checks, that byte is text-based
        /// </summary>
        /// <param name="character">Byte to check</param>
        private static bool CheckSingleCharacter(byte character) =>
            character is (0 or >= 8) and (<= 13 or >= 26);
    }
}
