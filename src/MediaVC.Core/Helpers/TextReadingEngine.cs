using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MediaVC.Difference;

namespace MediaVC.Helpers
{
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

        private static Encoding UTF32LittleEndianEncoding =>
            Encoding.GetEncodings()
            .Where(encoding => encoding.Name == "UTF-32LE")
            .First()
            .GetEncoding();

        #endregion

        #region Methods

        public async ValueTask<Rune?> ReadAsync(CancellationToken cancellationToken = default)
        {
            for(;;)
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

                    var state = this.bomDetector.LastReadingState;
                    if(state != TextReadingState.Done)
                    {
                        LastReadingState = state;
                        return null;
                    }

                    if(byteOrder.HasValue)
                    {
                        SelectedEncoding = byteOrder == ByteOrder.LittleEndian ?
                            UTF32LittleEndianEncoding :
                            Encoding.UTF32;

                        ByteOrder = byteOrder.Value;
                        continue;
                    }

                    var detected = await this.bomDetector.ScanForUTF8BOM(cancellationToken);

                    if(state != TextReadingState.Done)
                    {
                        LastReadingState = state;
                        return null;
                    }

                    if(detected)
                    {
                        SelectedEncoding = Encoding.UTF8;
                        ByteOrder = ByteOrder.LittleEndian;
                        continue;
                    }

                    byteOrder = await this.bomDetector.ScanForUTF16BOM(cancellationToken);

                    if(state != TextReadingState.Done)
                    {
                        LastReadingState = state;
                        return null;
                    }

                    if(byteOrder.HasValue)
                    {
                        SelectedEncoding = byteOrder == ByteOrder.LittleEndian ?
                            Encoding.Unicode :
                            Encoding.BigEndianUnicode;

                        ByteOrder = byteOrder.Value;
                        continue;
                    }

                    return await ReadUTF8Segments(cancellationToken);
                }
            }
        }

        private async ValueTask<Rune?> ReadCharacterWithSelectedEncoding(CancellationToken cancellationToken)
        {
            var readedByte = await this.source.ReadByteAsync(cancellationToken);

            var chars = SelectedEncoding!.GetChars(readedByte.Yield().ToArray());

            return chars.Length switch
            {
                1 => new Rune(chars[0]),
                2 => new Rune(chars[0], chars[1]),
                _ => throw new FormatException("Chars are too much to create Rune."),
            };
        }

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
                return new Rune(Encoding.UTF8.GetChars(byte1.Yield().ToArray())[0]);
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

            return resultChars.Length <= 2
                ? new Rune(resultChars[0], resultChars[1])
                : throw new FormatException("Chars are too much to create Rune.");
        }

        private async ValueTask<Rune?> ReadUTF16Segments(CancellationToken cancellationToken = default)
        {
            Memory<byte> firstReadedBytes = new byte[2];

            if(await this.source.ReadAsync(firstReadedBytes, cancellationToken) != firstReadedBytes.Length)
            {
                LastReadingState = TextReadingState.UnexpectedEndOfStream;
                return null;
            }

            if(ByteOrder == ByteOrder.LittleEndian)
            {
                if(firstReadedBytes.Span[0] >> 2 == 0b110110)
                {
                    Memory<byte> secondReadedBytes = new byte[2];
                    var readedCount = await this.source.ReadAsync(secondReadedBytes, cancellationToken);
                    
                    if(readedCount == secondReadedBytes.Length &&
                        secondReadedBytes.Span[0] >> 2 == 0b110111)
                    {
                        return new Rune(BitConverter.ToChar(firstReadedBytes.Span), BitConverter.ToChar(secondReadedBytes.Span));
                    }
                    else
                    {
                        this.source.Position -= readedCount + 1;
                    }
                }

                if(BitConverter.ToUInt16(firstReadedBytes.Span) <= ushort.MaxValue)
                {
                    return new Rune(BitConverter.ToChar(firstReadedBytes.Span));
                }
                else
                {
                    LastReadingState = TextReadingState.TooHighValueOfSegment;
                    return null;
                }
            }
            else
            {
                if(firstReadedBytes.Span[1] >> 2 == 0b110111)
                {
                    Memory<byte> secondReadedBytes = new byte[2];
                    var readedCount = await this.source.ReadAsync(secondReadedBytes, cancellationToken);

                    if(readedCount == secondReadedBytes.Length &&
                        secondReadedBytes.Span[1] >> 2 == 0b110110)
                    {
                        var allBytes = new byte[]
                        { 
                            firstReadedBytes.Span[0],
                            firstReadedBytes.Span[1],
                            secondReadedBytes.Span[0],
                            secondReadedBytes.Span[1]
                        };

                        var chars = Encoding.BigEndianUnicode.GetChars(allBytes);

                        return chars.Length == 2
                            ? new Rune(chars[0], chars[1])
                            : throw new FormatException("Chars are too much to create Rune.");
                    }
                    else
                    {
                        this.source.Position -= readedCount + 1;
                    }
                }

                if(BitConverter.ToUInt16(firstReadedBytes.Span) <= ushort.MaxValue)
                {
                    return new Rune(BitConverter.ToChar(firstReadedBytes.Span));
                }
                else
                {
                    LastReadingState = TextReadingState.TooHighValueOfSegment;
                    return null;
                }
            }
        }

        private async ValueTask<Rune?> ReadUTF32Segments(CancellationToken cancellationToken)
        {
            const int utf32CharLength = 4;
            Memory<byte> bytes = new byte[utf32CharLength];

            if(await this.source.ReadAsync(bytes, cancellationToken) != utf32CharLength)
            {
                LastReadingState = TextReadingState.UnexpectedEndOfStream;
                return null;
            }

            if(ByteOrder == ByteOrder.LittleEndian)
                bytes.Span.Reverse();

            var chars = Encoding.UTF32.GetChars(bytes.ToArray());

            return new Rune(chars[0], chars[1]);
        }

        #endregion
    }
}
