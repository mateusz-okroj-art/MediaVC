using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MediaVC.Core.Tests.Fixtures
{
    public class StringReaderFixture
    {
        public StringReaderFixture()
        {
            using var file1 = new FileStream("TestData/utf-8 without bom.bin", FileMode.Open, FileAccess.Read);
            UTF8_Bytes = new byte[file1.Length];
            var task1 = file1.ReadAsync(UTF8_Bytes);

            using var file2 = new FileStream("TestData/utf-32 big-endian with bom.bin", FileMode.Open, FileAccess.Read);
            UTF32BE_Bytes = new byte[file2.Length];
            var task2 = file2.ReadAsync(UTF32BE_Bytes);

            using var file3 = new FileStream("TestData/utf-16 little-endian with bom.bin", FileMode.Open, FileAccess.Read);
            UTF16LE_Bytes = new byte[file3.Length];
            var task3 = file3.ReadAsync(UTF16LE_Bytes);

            using var file4 = new FileStream("TestData/utf-16 big-endian with bom.bin", FileMode.Open, FileAccess.Read);
            UTF16BE_Bytes = new byte[file4.Length];
            var task4 = file4.ReadAsync(UTF16BE_Bytes);

            Task.WaitAll(task1.AsTask(), task2.AsTask(), task3.AsTask(), task4.AsTask());

            UTF8_Content = Encoding.UTF8.GetString(UTF8_Bytes.Span);
            UTF32BE_Content = Encoding.UTF32.GetString(UTF32BE_Bytes.Span);
            UTF16LE_Content = Encoding.Unicode.GetString(UTF16LE_Bytes.Span);
            UTF16BE_Content = Encoding.BigEndianUnicode.GetString(UTF16BE_Bytes.Span);
        }

        public Memory<byte> UTF16LE_Bytes { get; }

        public Memory<byte> UTF16BE_Bytes { get; }

        public Memory<byte> UTF32BE_Bytes { get; }

        public Memory<byte> UTF8_Bytes { get; }

        public string UTF16LE_Content { get; }

        public string UTF16BE_Content { get; }

        public string UTF32BE_Content { get; }

        public string UTF8_Content { get; }
    }
}
