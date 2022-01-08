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
        public TextReadingEngine(IInputSource inputSource) =>
            this.source = inputSource ?? throw new ArgumentNullException(nameof(inputSource));

        private readonly IInputSource source;

        public Encoding? SelectedEncoding { get; set; }

        public TextReadingState LastReadingState { get; private set; }

        public bool IsLittleEndian { get; set; }

        private static Encoding UTF32BigEndianEncoding =>
            Encoding.GetEncodings()
            .Where(encoding => encoding.Name == "UTF-32BE")
            .First()
            .GetEncoding();

        public async ValueTask<Rune?> ReadAsync(CancellationToken cancellationToken = default)
        {
            var loopControllerSource = new ExternalLoopControllerSource();

            while(!loopControllerSource.IsBreakRequested)
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
                    await ScanForUTF32BOM(loopControllerSource.Controller, cancellationToken);

                    if(loopControllerSource.IsBreakRequested)
                        return null;

                    await ScanForUTF8BOM(loopControllerSource.Controller, cancellationToken);

                    if(loopControllerSource.IsBreakRequested)
                        return null;

                    await ScanForUTF16BOM(loopControllerSource.Controller, cancellationToken);

                    return !loopControllerSource.IsBreakRequested ?
                        await ReadUTF8Segments(cancellationToken) :
                        null;
                }
            }

            return null;
        }

        private async ValueTask<Rune?> ReadCharacterWithSelectedEncoding(CancellationToken cancellationToken)
        {
            var readedByte = await this.source.ReadByteAsync(cancellationToken);

            var chars = SelectedEncoding!.GetChars(readedByte.Yield().ToArray());

            if(chars.Length > 2)
                throw new FormatException("Chars are too much to create Rune.");

            return chars.Length == 1 ?
                new Rune(chars[0]) :
                new Rune(chars[0], chars[1]);
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

            if(IsLittleEndian)
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

            if(IsLittleEndian)
                bytes.Span.Reverse();

            var chars = Encoding.UTF32.GetChars(bytes.ToArray());

            return new Rune(chars[0], chars[1]);
        }

        /// <summary>
        /// Checks that stream have UTF-16 Byte Order Mark on start position
        /// </summary>
        private async ValueTask ScanForUTF16BOM(IExternalLoopController externalLoopController, CancellationToken cancellationToken = default)
        {
            if(this.source.Position <= this.source.Length - 2 && this.source.Length >= 2)
            {
                var potentialBomMark = new byte[2];

                if(await this.source.ReadAsync(potentialBomMark.AsMemory(), cancellationToken) != potentialBomMark.Length)
                {
                    LastReadingState = TextReadingState.UnexpectedEndOfStream;
                    externalLoopController.Break();
                    return;
                }

                if(potentialBomMark[0] == 0xff && potentialBomMark[1] == 0xfe)
                {
                    SelectedEncoding = Encoding.Unicode;
                    IsLittleEndian = true;
                }
                else if(potentialBomMark[1] == 0xff && potentialBomMark[0] == 0xfe)
                {
                    SelectedEncoding = Encoding.BigEndianUnicode;
                    IsLittleEndian = false;
                }
                else
                {
                    this.source.Position -= 2;
                }
            }
        }

        /// <summary>
        /// Checks that stream have UTF-8 Byte Order Mark on start position
        /// </summary>
        private async ValueTask ScanForUTF8BOM(IExternalLoopController externalLoopController, CancellationToken cancellationToken = default)
        {
            if(this.source.Position <= this.source.Length - 3 && this.source.Length >= 3)
            {
                var potentialBomMark = new byte[3];

                if(await this.source.ReadAsync(potentialBomMark.AsMemory(), cancellationToken) != potentialBomMark.Length)
                {
                    LastReadingState = TextReadingState.UnexpectedEndOfStream;
                    externalLoopController.Break();
                    return;
                }

                if(potentialBomMark[0] == 0xef && potentialBomMark[1] == 0xbb && potentialBomMark[2] == 0xbf)
                {
                    SelectedEncoding = Encoding.UTF8;
                    IsLittleEndian = true;
                }
                else
                {
                    this.source.Position -= 3;
                }
            }
        }

        /// <summary>
        /// Checks that stream have UTF-32 Byte Order Mark on start position
        /// </summary>
        private async ValueTask ScanForUTF32BOM(IExternalLoopController externalLoopController, CancellationToken cancellationToken = default)
        {
            if(this.source.Position <= this.source.Length - 4 && this.source.Length >= 4)
            {
                var potentialBomMark = new byte[4];

                if(await this.source.ReadAsync(potentialBomMark.AsMemory(), cancellationToken) != potentialBomMark.Length)
                {
                    LastReadingState = TextReadingState.UnexpectedEndOfStream;
                    externalLoopController.Break();
                    return;
                }

                if(potentialBomMark[0] == 0xff && potentialBomMark[1] == 0xfe && potentialBomMark[2] == 0 && potentialBomMark[3] == 0)
                {
                    SelectedEncoding = Encoding.UTF32;
                    IsLittleEndian = true;
                }
                else if(potentialBomMark[3] == 0xff && potentialBomMark[2] == 0xfe && potentialBomMark[1] == 0 && potentialBomMark[0] == 0)
                {
                    SelectedEncoding = UTF32BigEndianEncoding;
                    IsLittleEndian = false;
                }
                else
                {
                    this.source.Position -= 4;
                }
            }
        }
    }
}
