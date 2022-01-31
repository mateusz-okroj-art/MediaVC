using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaVC.Core.Tests.Fixtures
{
    internal class StringReaderFixture
    {
        public StringReaderFixture()
        {
            using var file1 = new FileStream("utf-8 without bom.bin", FileMode.Open, FileAccess.Read);
            UTF8_Bytes = new byte[file1.Length];
            var task1 = file1.ReadAsync(UTF8_Bytes);

        }

        public Memory<byte> UTF16LE_Bytes { get; }

        public Memory<byte> UTF16BE_Bytes { get; }

        public Memory<byte> UTF32BE_Bytes { get; }

        public Memory<byte> UTF8_Bytes { get; }

        public Memory<Rune> UTF16LE_Runes { get; }

        public Memory<Rune> UTF16BE_Runes { get; }

        public Memory<Rune> UTF32BE_Runes { get; }

        public Memory<Rune> UTF8_Runes { get; }
    }
}
