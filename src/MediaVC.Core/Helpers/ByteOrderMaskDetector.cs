using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediaVC.Difference;

namespace MediaVC.Helpers
{
    internal class ByteOrderMaskDetector
    {
        public ByteOrderMaskDetector(IInputSource source) =>
            this.source = source ?? throw new ArgumentNullException(nameof(source));

        private readonly IInputSource source;

        /// <summary>
        /// Checks that stream have UTF-16 Byte Order Mark on start position
        /// </summary>
        public async ValueTask ScanForUTF16BOM(IExternalLoopController externalLoopController, CancellationToken cancellationToken = default)
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
        public async ValueTask ScanForUTF8BOM(IExternalLoopController externalLoopController, CancellationToken cancellationToken = default)
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
        public async ValueTask ScanForUTF32BOM(IExternalLoopController externalLoopController, CancellationToken cancellationToken = default)
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
                    SelectedEncoding = UTF32LittleEndianEncoding;
                    IsLittleEndian = true;
                }
                else if(potentialBomMark[3] == 0xff && potentialBomMark[2] == 0xfe && potentialBomMark[1] == 0 && potentialBomMark[0] == 0)
                {
                    SelectedEncoding = Encoding.UTF32;
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
