using System;
using System.IO;

namespace MediaVC.Models
{
    public class DifferenceInfo
    {
        public Guid Id { get; set; }

        public byte[][] Checksums { get; set; }

        public Stream Content { get; set; } = Stream.Null;
    }
}
