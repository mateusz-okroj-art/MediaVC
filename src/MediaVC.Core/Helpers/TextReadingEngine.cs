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

        public async ValueTask<Rune?> ReadAsync(CancellationToken cancellationToken = default)
        {
            if(this.source.Position >= this.source.Length)
                return null;

            if(SelectedEncoding is not null)
            {
                if(SelectedEncoding.EncodingName == "")

                var count = Math.Min(4, (int)(this.source.Length - this.source.Position));
                for(var i = count; i > 0; --i)
                {
                    var bytes = new byte[i];

                    var readedBytes = await this.source.ReadAsync(bytes.AsMemory(0,i), cancellationToken);
                    
                    if(readedBytes < i)
                    {
                        LastReadingState = TextReadingState.UnexpectedEndOfStream;
                        return null;
                    }

                    var countedChars = SelectedEncoding.GetCharCount(bytes);

                    if(countedChars == 1)
                        return SelectedEncoding.GetChars(bytes)[0];
                    else
                        this.source.Position -= i;
                }
            }
            else
            {
                if(this.source.Position <= this.source.Length - 4 && this.source.Length >= 4)
                {
                    // Check that have UTF-32 Byte Order Mark on stream start
                    var potentialBomMark = new byte[4];

                    if(await this.source.ReadAsync(potentialBomMark.AsMemory(), cancellationToken) != potentialBomMark.Length)
                    {
                        LastReadingState = TextReadingState.UnexpectedEndOfStream;
                        return null;
                    }

                    if(potentialBomMark[0] == 0xff && potentialBomMark[1] == 0xfe && potentialBomMark[2] == 0 && potentialBomMark[3] == 0)
                    {
                        SelectedEncoding = Encoding.UTF32;
                        IsLittleEndian = true;
                    }
                    else if(potentialBomMark[3] == 0xff && potentialBomMark[2] == 0xfe && potentialBomMark[1] == 0 && potentialBomMark[0] == 0)
                    {
                        SelectedEncoding = Encoding.GetEncodings().Where(encoding => encoding.Name == "UTF-32BE").Select(encoding => encoding.GetEncoding()).First();
                        IsLittleEndian = false;
                    }
                    else
                    {
                        this.source.Position -= 4;
                    }
                }

                if(this.source.Position <= this.source.Length - 3 && this.source.Length >= 3)
                {
                    // Check that have UTF-8 Byte Order Mark on stream start
                    var potentialBomMark = new byte[3];

                    if(await this.source.ReadAsync(potentialBomMark.AsMemory(), cancellationToken) != potentialBomMark.Length)
                    {
                        LastReadingState = TextReadingState.UnexpectedEndOfStream;
                        return null;
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

                if(this.source.Position <= this.source.Length - 2 && this.source.Length >= 2)
                {
                    // Check that have UTF-16 Byte Order Mark on stream start
                    var potentialBomMark = new byte[2];

                    if(await this.source.ReadAsync(potentialBomMark.AsMemory(), cancellationToken) != potentialBomMark.Length)
                    {
                        LastReadingState = TextReadingState.UnexpectedEndOfStream;
                        return null;
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

                if(this.source.Position == this.source.Length - 1)
                {
                    var byte1 = await this.source.ReadByteAsync(cancellationToken);

                    if(byte1 <= 127)
                    {
                        LastReadingState = TextReadingState.Done;
                        return Encoding.UTF8.GetChars(byte1.Yield().ToArray())[0];
                    }
                    else
                    {
                        LastReadingState = TextReadingState.TooHighValueOfSegment;
                        return null;
                    }
                }
                else
                {
                    var byte1 = await this.source.ReadByteAsync(cancellationToken);
                    Memory<byte> readedBytes;

                    // UTF-8 decoding
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
                        return Encoding.UTF8.GetChars(byte1.Yield().ToArray())[0];
                    }
                    else
                    {
                        LastReadingState = TextReadingState.TooHighValueOfSegment;
                        return null;
                    }

                    if(await this.source.ReadAsync(readedBytes, cancellationToken) != 5)
                    {
                        LastReadingState = TextReadingState.UnexpectedEndOfStream;
                        return null;
                    }

                    for(var i = 0; i < readedBytes.Length; ++i)
                    {
                        if(readedBytes.Span[i] >> 6 != 0b10)
                        {
                            LastReadingState = TextReadingState.TooHighValueOfSegment;

                        }
                    }

                    var resultBytes = new byte[readedBytes.Length + 1];
                    resultBytes[0] = byte1;

                    readedBytes.CopyTo(resultBytes.AsMemory()[1..]);
                    return Encoding.UTF8.GetChars(resultBytes)[0];
                }
            }
        }
    }
}
