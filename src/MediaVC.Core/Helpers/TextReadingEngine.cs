using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MediaVC.Difference;

namespace MediaVC.Helpers
{
    /// <summary>
    /// Core of <see cref="StringReader"/>
    /// </summary>
    internal sealed class TextReadingEngine
    {
        public TextReadingEngine(IInputSource inputSource)
        {
            this.source = inputSource ?? throw new ArgumentNullException(nameof(inputSource));
            this.bomDetector = new ByteOrderMaskDetector(inputSource);
        }

        private readonly IInputSource source;
        private readonly ByteOrderMaskDetector bomDetector;

        #region Properties

        public Encoding? SelectedEncoding { get; set; }

        public TextReadingState LastReadingState { get; private set; }

        public ByteOrder ByteOrder { get; set; }

        public LineEnding LineEnding { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Reads single <see cref="Rune"/>.
        /// </summary>
        public async ValueTask<Rune?> ReadAsync(CancellationToken cancellationToken = default)
        {
            while(true)
            {
                if(this.source.Position >= this.source.Length)
                {
                    LastReadingState = TextReadingState.UnexpectedEndOfStream;
                    return null;
                }

                if(SelectedEncoding is not null)
                {
                    return SelectedEncoding.HeaderName switch
                    {
                        "utf-8" or "ascii" => await ReadUTF8Segments(cancellationToken),
                        "utf-16" or "utf-16BE" => await ReadUTF16Segments(cancellationToken),
                        "utf-32" or "utf-32BE" => await ReadUTF32Segments(cancellationToken),
                        _ => await ReadCharacterWithSelectedEncoding(cancellationToken),
                    };
                }
                else
                {
                    var byteOrder = await this.bomDetector.ScanForUTF32BOM(cancellationToken);

                    if(await DetectBOMsAsync(byteOrder, cancellationToken))
                        continue;

                    return await ReadUTF8Segments(cancellationToken);
                }
            }
        }

        /// <summary>
        /// Scans Unicode Byte Order Mask on start position.
        /// </summary>
        private async ValueTask<bool> DetectBOMsAsync(ByteOrder? byteOrder, CancellationToken cancellationToken)
        {
            if(byteOrder.HasValue)
            {
                SelectedEncoding = byteOrder == ByteOrder.LittleEndian ?
                    Encoding.UTF32 :
                    UnicodeHelper.UTF32BigEndianEncoding;

                ByteOrder = byteOrder.Value;
                return true;
            }

            var detected = await this.bomDetector.ScanForUTF8BOM(cancellationToken);

            if(detected)
            {
                SelectedEncoding = Encoding.UTF8;
                ByteOrder = ByteOrder.LittleEndian;
                return true;
            }

            byteOrder = await this.bomDetector.ScanForUTF16BOM(cancellationToken);

            if(byteOrder.HasValue)
            {
                SelectedEncoding = byteOrder == ByteOrder.LittleEndian ?
                    Encoding.Unicode :
                    Encoding.BigEndianUnicode;

                ByteOrder = byteOrder.Value;
                return true;
            }

            return false;
        }

        private async ValueTask<Rune?> ReadCharacterWithSelectedEncoding(CancellationToken cancellationToken)
        {
            var readedByte = await this.source.ReadByteAsync(cancellationToken);

            var chars = SelectedEncoding!.GetChars(new[] { readedByte });

            return chars.Length switch
            {
                1 => new Rune(chars[0]),
                2 => new Rune(chars[0], chars[1]),
                _ => throw new FormatException("Chars are too much to create Rune."),
            };
        }

        /// <summary>
        /// Reads single <see cref="Rune"/> from UTF-8 bytes.
        /// </summary>
        /// <exception cref="IOException"></exception>
        /// <exception cref="FormatException"></exception>
        private async ValueTask<Rune?> ReadUTF8Segments(CancellationToken cancellationToken)
        {
            var byte1 = await this.source.ReadByteAsync(cancellationToken);
            Memory<byte> readedBytes;

            if(byte1 >> 1 == 0b1111110)
            {
                readedBytes = new byte[5];
            }
            else if(byte1 >> 2 == 0b111110)
            {
                readedBytes = new byte[4];
            }
            else if(byte1 >> 3 == 0b11110)
            {
                readedBytes = new byte[3];
            }
            else if(byte1 >> 4 == 0b1110)
            {
                readedBytes = new byte[2];
            }
            else if(byte1 >> 5 == 0b110)
            {
                readedBytes = new byte[1];
            }
            else if(byte1 <= 127)
            {
                LastReadingState = TextReadingState.Done;
                return new Rune(Encoding.UTF8.GetChars(new[] { byte1 })[0]);
            }
            else
            {
                LastReadingState = TextReadingState.TooHighValueOfSegment;
                return null;
            }

            if(await this.source.ReadAsync(readedBytes, cancellationToken) != readedBytes.Length)
            {
                LastReadingState = TextReadingState.UnexpectedEndOfStream;
                return null;
            }

            for(var i = 0; i < readedBytes.Length; ++i)
            {
                if(readedBytes.Span[i] >> 6 != 0b10)
                {
                    LastReadingState = TextReadingState.TooHighValueOfSegment;
                    return null;
                }
            }

            var resultBytes = new byte[readedBytes.Length + 1];
            resultBytes[0] = byte1;

            readedBytes.CopyTo(resultBytes.AsMemory()[1..]);
            var resultChars = Encoding.UTF8.GetChars(resultBytes);

            if(resultChars is null || resultChars.Length < 1)
                throw new IOException("Empty values from UTF-8 decoder.");

            return resultChars.Length switch
            {
                1 => new Rune(resultChars[0]),
                2 => new Rune(resultChars[0], resultChars[1]),
                _ => throw new FormatException("Chars are too much to create Rune.")
            };
        }

        /// <summary>
        /// Reads single <see cref="Rune"/> from UTF-16 bytes.
        /// </summary>
        private async ValueTask<Rune?> ReadUTF16Segments(CancellationToken cancellationToken = default)
        {
            Memory<byte> firstReadedBytes = new byte[2];

            if(await this.source.ReadAsync(firstReadedBytes, cancellationToken) != 2)
            {
                LastReadingState = TextReadingState.UnexpectedEndOfStream;
                return null;
            }

            if(ByteOrder == ByteOrder.LittleEndian)
                return await ReadUTF16LESegments(firstReadedBytes, cancellationToken);
            else
                return await ReadUTF16BESegments(firstReadedBytes, cancellationToken);
        }

        /// <summary>
        /// Reads single <see cref="Rune"/> from UTF-16 Big Endian bytes.
        /// </summary>
        /// <param name="firstReadedBytes"></param>
        private async ValueTask<Rune?> ReadUTF16BESegments(Memory<byte> firstReadedBytes, CancellationToken cancellationToken)
        {
            firstReadedBytes.Span.Reverse();

            if(this.source.Position <= this.source.Length - 2)
            {
                Memory<byte> secondReadedBytes = new byte[2];

                if(await this.source.ReadAsync(secondReadedBytes, cancellationToken) != 2)
                {
                    LastReadingState = TextReadingState.UnexpectedEndOfStream;
                    return null;
                }

                if(firstReadedBytes.Span[1] >> 2 == 0b110110 && secondReadedBytes.Span[0] >> 2 == 0b110111)
                {
                    secondReadedBytes.Span.Reverse();

                    char char1 = BitConverter.ToChar(firstReadedBytes.Span),
                         char2 = BitConverter.ToChar(secondReadedBytes.Span);

                    return new Rune(char1, char2);
                }
                else
                {
                    this.source.Position -= 2;
                }
            }

            var singleChar = BitConverter.ToChar(firstReadedBytes.Span);

            if(UnicodeHelper.IsSurrogateCodePoint(singleChar))
            {
                LastReadingState = TextReadingState.TooHighValueOfSegment;
                return null;
            }

            return new Rune(singleChar);
        }

        /// <summary>
        /// Reads single <see cref="Rune"/> from UTF-16 Little Endian bytes.
        /// </summary>
        private async ValueTask<Rune?> ReadUTF16LESegments(Memory<byte> firstReadedBytes, CancellationToken cancellationToken)
        {
            if(this.source.Position <= this.source.Length - 2)
            {
                Memory<byte> secondReadedBytes = new byte[2];

                if(await this.source.ReadAsync(secondReadedBytes, cancellationToken) != 2)
                {
                    LastReadingState = TextReadingState.UnexpectedEndOfStream;
                    return null;
                }

                if(firstReadedBytes.Span[1] >> 2 == 0b110110 && secondReadedBytes.Span[1] >> 2 == 0b110111)
                {
                    char char1 = BitConverter.ToChar(firstReadedBytes.Span),
                         char2 = BitConverter.ToChar(secondReadedBytes.Span);

                    return new Rune(char1, char2);
                }
                else
                {
                    this.source.Position -= 2;
                }
            }
            var singleChar = BitConverter.ToChar(firstReadedBytes.Span);

            if(UnicodeHelper.IsSurrogateCodePoint(singleChar))
            {
                LastReadingState = TextReadingState.TooHighValueOfSegment;
                return null;
            }

            return new Rune(singleChar);
        }

        /// <summary>
        /// Reads single <see cref="Rune"/> from UTF-32 bytes.
        /// </summary>
        /// <exception cref="IOException"></exception>
        private async ValueTask<Rune?> ReadUTF32Segments(CancellationToken cancellationToken)
        {
            const int utf32CharLength = 4;
            Memory<byte> bytes = new byte[utf32CharLength];

            if(await this.source.ReadAsync(bytes, cancellationToken) != utf32CharLength)
            {
                LastReadingState = TextReadingState.UnexpectedEndOfStream;
                return null;
            }

            if(ByteOrder != ByteOrder.LittleEndian)
                bytes.Span.Reverse();

            var chars = Encoding.UTF32.GetChars(bytes.ToArray());

            if(chars is null || chars.Length < 1)
                throw new IOException("Empty values from UTF-32 decoder.");

            return chars?.Length switch
            {
                1 => new Rune(chars[0]),
                2 => new Rune(chars[0], chars[1]),
                _ => throw new IOException("Error while decoding UTF-32 chars.")
            };
        }

        internal void Reset()
        {
            LastReadingState = TextReadingState.Done;
        }

        #endregion
    }
}
