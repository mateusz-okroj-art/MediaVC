using MediaVC.Difference;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediaVC.Helpers
{
    internal class ByteOrderMaskDetector
    {
        public ByteOrderMaskDetector(IInputSource source) =>
            this.source = source ?? throw new ArgumentNullException(nameof(source));

        private readonly IInputSource source;

        public TextReadingState LastReadingState { get; private set; }

        /// <summary>
        /// Checks that stream have UTF-16 Byte Order Mark on start position
        /// </summary>
        public async ValueTask<ByteOrder?> ScanForUTF16BOM(CancellationToken cancellationToken = default)
        {
            var startPosition = this.source.Position;

            this.source.Position = 0;

            if(this.source.Length >= 2)
            {
                return await ScanForUTF16BOMCore(startPosition, cancellationToken);
            }
            else
            {
                LastReadingState = TextReadingState.UnexpectedEndOfStream;
                this.source.Position = startPosition;
                return null;
            }
        }

        private async ValueTask<ByteOrder?> ScanForUTF16BOMCore(long startPosition, CancellationToken cancellationToken)
        {
            var potentialBomMark = new byte[2];

            if(await this.source.ReadAsync(potentialBomMark.AsMemory(), cancellationToken) != potentialBomMark.Length)
            {
                LastReadingState = TextReadingState.UnexpectedEndOfStream;
                this.source.Position = startPosition;
                return null;
            }

            if(potentialBomMark[0] == 0xff && potentialBomMark[1] == 0xfe)
            {
                LastReadingState = TextReadingState.Done;

                if(startPosition > 0)
                    this.source.Position = startPosition;

                return ByteOrder.LittleEndian;
            }
            else if(potentialBomMark[1] == 0xff && potentialBomMark[0] == 0xfe)
            {
                LastReadingState = TextReadingState.Done;

                if(startPosition > 0)
                    this.source.Position = startPosition;

                return ByteOrder.BigEndian;
            }
            else
            {
                LastReadingState = TextReadingState.Done;
                this.source.Position = startPosition;
                return null;
            }
        }

        /// <summary>
        /// Checks that stream have UTF-8 Byte Order Mark on start position
        /// </summary>
        public async ValueTask<bool> ScanForUTF8BOM(CancellationToken cancellationToken = default)
        {
            var startPosition = this.source.Position;

            this.source.Position = 0;

            if(this.source.Length >= 3)
            {
                return await ScanForUTF8BOMCore(startPosition, cancellationToken);
            }
            else
            {
                LastReadingState = TextReadingState.UnexpectedEndOfStream;
                this.source.Position = startPosition;
                return false;
            }
        }

        private async ValueTask<bool> ScanForUTF8BOMCore(long startPosition, CancellationToken cancellationToken)
        {
            var potentialBomMark = new byte[3];

            if(await this.source.ReadAsync(potentialBomMark.AsMemory(), cancellationToken) != potentialBomMark.Length)
            {
                LastReadingState = TextReadingState.UnexpectedEndOfStream;
                this.source.Position = startPosition;
                return false;
            }

            if(potentialBomMark[0] == 0xef && potentialBomMark[1] == 0xbb && potentialBomMark[2] == 0xbf)
            {
                LastReadingState = TextReadingState.Done;

                if(startPosition > 0)
                    this.source.Position = startPosition;

                return true;
            }
            else
            {
                LastReadingState = TextReadingState.Done;
                this.source.Position = startPosition;
                return false;
            }
        }

        /// <summary>
        /// Checks that stream have UTF-32 Byte Order Mark on start position
        /// </summary>
        public async ValueTask<ByteOrder?> ScanForUTF32BOM(CancellationToken cancellationToken = default)
        {
            var startPosition = this.source.Position;

            this.source.Position = 0;

            if(this.source.Length >= 4)
            {
                return await ScanForUTF32BOMCore(startPosition, cancellationToken);
            }
            else
            {
                LastReadingState = TextReadingState.UnexpectedEndOfStream;
                this.source.Position = startPosition;
                return null;
            }
        }

        private async ValueTask<ByteOrder?> ScanForUTF32BOMCore(long startPosition, CancellationToken cancellationToken)
        {
            Memory<byte> potentialBomMark = new byte[4];

            if(await this.source.ReadAsync(potentialBomMark, cancellationToken) != potentialBomMark.Length)
            {
                LastReadingState = TextReadingState.UnexpectedEndOfStream;
                this.source.Position = startPosition;
                return null;
            }

            if(potentialBomMark.Span[0] == 0xff && potentialBomMark.Span[1] == 0xfe && potentialBomMark.Span[2] == 0 && potentialBomMark.Span[3] == 0)
            {
                LastReadingState = TextReadingState.Done;

                if(startPosition > 0)
                    this.source.Position = startPosition;

                return ByteOrder.LittleEndian;
            }
            else if(potentialBomMark.Span[3] == 0xff && potentialBomMark.Span[2] == 0xfe && potentialBomMark.Span[1] == 0 && potentialBomMark.Span[0] == 0)
            {
                LastReadingState = TextReadingState.Done;

                if(startPosition > 0)
                    this.source.Position = startPosition;

                return ByteOrder.BigEndian;
            }
            else
            {
                LastReadingState = TextReadingState.Done;
                this.source.Position = startPosition;
                return null;
            }
        }
    }
}
