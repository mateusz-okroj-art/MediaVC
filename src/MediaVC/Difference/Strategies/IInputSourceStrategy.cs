using System;

namespace MediaVC.Difference.Strategies
{
    internal interface IInputSourceStrategy : IEquatable<IInputSourceStrategy>
    {
        long Length { get; }

        long Position { get; set; }

        int Read(byte[] buffer, int offset, int count);

        int Read(Span<byte> buffer, int offset, int count);

        byte ReadByte();
    }
}