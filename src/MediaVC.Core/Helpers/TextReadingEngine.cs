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

        public async ValueTask<char?> ReadAsync(CancellationToken cancellationToken = default)
        {
            if(this.source.Position >= this.source.Length)
                return null;

            if(SelectedEncoding is not null)
            {
                var count = Math.Min(3, (int)(this.source.Length - this.source.Position));
                for(var i = count; i > 0; --i)
                {
                    var bytes = new byte[i];

                    this.source.Read(bytes, 0, i);

                    var countedChars = SelectedEncoding.GetCharCount(bytes);

                    if(countedChars == 1)
                        return SelectedEncoding.GetChars(bytes)[0];
                    else
                        this.source.Position -= i;
                }
            }
            else
            {
                if(this.source.Position == this.source.Length - 1)
                {
                    var byte1 = await this.source.ReadByteAsync(cancellationToken);

                    return byte1 > 127
                        ? throw new InvalidOperationException("Byte is not valid UTF-8 segment.")
                        : Encoding.UTF8.GetChars(new byte[] { byte1 })[0];
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
                    else
                    {
                        return byte1 > 127
                        ? throw new InvalidOperationException("Byte is not segment of char.")
                        : Encoding.UTF8.GetChars(new byte[] { byte1 })[0];
                    }

                    if(await this.source.ReadAsync(readedBytes, cancellationToken) != 5)
                        throw new InvalidOperationException("Unexpected end of stream.");

                    for(var i = 0; i < readedBytes.Length; ++i)
                    {
                        if(readedBytes.Span[i] >> 6 != 0b10)
                            throw new InvalidOperationException("Invalid UTF-8 segments format");
                    }

                    var resultBytes = new byte[readedBytes.Length + 1];
                    resultBytes[0] = byte1;

                    readedBytes.CopyTo(resultBytes.AsMemory()[1..]);
                    return Encoding.UTF8.GetChars(new byte[] { byte1 }.Concat(readedBytes.ToArray()).ToArray())[0];
                }
            }
        }
    }
}
