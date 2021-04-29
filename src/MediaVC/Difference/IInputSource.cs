using System;

namespace MediaVC.Difference
{
    public interface IInputSource
    {
        long Position { get; set; }

        long Length { get; }

        byte ReadByte();

        int Read(byte[] buffer, int offset, int count);

        int Read(Span<byte> buffer, int offset, int count);
    }
}
