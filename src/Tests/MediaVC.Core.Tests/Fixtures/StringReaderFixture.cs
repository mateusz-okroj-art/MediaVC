using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using MediaVC.Helpers;

namespace MediaVC.Core.Tests.Fixtures
{
    public class StringReaderFixture
    {
        public StringReaderFixture()
        {
            using var utf8_file = new FileStream("TestData/utf-8 without bom.bin", FileMode.Open, FileAccess.Read);
            UTF8_Bytes = new byte[utf8_file.Length];
            var task1 = utf8_file.ReadAsync(UTF8_Bytes);

            using var utf8bom_file = new FileStream("TestData/utf-8 with bom.bin", FileMode.Open, FileAccess.Read);
            UTF8BOM_Bytes = new byte[utf8bom_file.Length];
            var task2 = utf8bom_file.ReadAsync(UTF8BOM_Bytes);

            using var utf32be_file = new FileStream("TestData/utf-32 big-endian with bom.bin", FileMode.Open, FileAccess.Read);
            UTF32BE_Bytes = new byte[utf32be_file.Length];
            var task3 = utf32be_file.ReadAsync(UTF32BE_Bytes);

            using var utf32le_file = new FileStream("TestData/utf-32 little-endian with bom.bin", FileMode.Open, FileAccess.Read);
            UTF32LE_Bytes = new byte[utf32le_file.Length];
            var task4 = utf32le_file.ReadAsync(UTF32LE_Bytes);

            using var utf16le_file = new FileStream("TestData/utf-16 little-endian with bom.bin", FileMode.Open, FileAccess.Read);
            UTF16LE_Bytes = new byte[utf16le_file.Length];
            var task5 = utf16le_file.ReadAsync(UTF16LE_Bytes);

            using var utf16be_file = new FileStream("TestData/utf-16 big-endian with bom.bin", FileMode.Open, FileAccess.Read);
            UTF16BE_Bytes = new byte[utf16be_file.Length];
            var task6 = utf16be_file.ReadAsync(UTF16BE_Bytes);

            Task.WaitAll(task1.AsTask(), task2.AsTask(), task3.AsTask(), task4.AsTask(), task5.AsTask());

            UTF8_Content = Encoding.UTF8.GetString(UTF8_Bytes.Span);
            UTF8BOM_Content = Encoding.UTF8.GetString(UTF8BOM_Bytes.Span);
            UTF32BE_Content = UnicodeHelper.UTF32BigEndianEncoding.GetString(UTF32BE_Bytes.Span);
            UTF32LE_Content = Encoding.UTF32.GetString(UTF32LE_Bytes.Span);
            UTF16LE_Content = Encoding.Unicode.GetString(UTF16LE_Bytes.Span);
            UTF16BE_Content = Encoding.BigEndianUnicode.GetString(UTF16BE_Bytes.Span);
        }

        public Memory<byte> UTF16LE_Bytes { get; }

        public Memory<byte> UTF16BE_Bytes { get; }

        public Memory<byte> UTF32BE_Bytes { get; }

        public Memory<byte> UTF32LE_Bytes { get; }

        public Memory<byte> UTF8_Bytes { get; }

        public Memory<byte> UTF8BOM_Bytes { get; }

        public string UTF16LE_Content { get; }

        public string UTF16BE_Content { get; }

        public string UTF32BE_Content { get; }

        public string UTF32LE_Content { get; }

        public string UTF8_Content { get; }

        public string UTF8BOM_Content { get; }
    }
}
