using System;
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

        public async ValueTask<char?> ReadAsync(CancellationToken cancellationToken = default)
        {
            if(source.Position >= source.Length)
                return null;

            if(source.Position == source.Length - 1)
            {
                var byte1 = await source.ReadByteAsync(cancellationToken);

                return byte1 > 127
                    ? throw new InvalidOperationException("Byte is not segment of char.")
                    : Encoding.UTF8.GetChars(new byte[] { byte1 })[0];
            }
            else
            {
                var byte1 = await source.ReadByteAsync(cancellationToken);

                if(byte1 >> 5 == 0b111) // UTF-8 - first segment of three part char
                {

                }
                else if(byte1 >> 6 == 0b11) // UTF-8 - first segment of two part char
                {

                }
                else
                {
                    return byte1 > 127
                    ? throw new InvalidOperationException("Byte is not segment of char.")
                    : Encoding.UTF8.GetChars(new byte[] { byte1 })[0];
                }
            }

        }
    }
}
