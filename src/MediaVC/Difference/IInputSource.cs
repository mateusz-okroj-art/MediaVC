using System;

using MediaVC.Difference.Strategies;

namespace MediaVC.Difference
{
    public interface IInputSource
    {
        long Position { get; set; }

        long Length { get; }

        byte ReadByte();

        Memory<byte> ReadBytes(long count);
    }
}
