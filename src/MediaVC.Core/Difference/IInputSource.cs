using System;

namespace MediaVC.Difference
{
    public interface IInputSource : IDisposable
    {
        long Position { get; set; }

        long Length { get; }

        byte ReadByte();

        int Read(byte[] buffer, int offset, int count);

        int Read(Memory<byte> buffer);
    }
}
