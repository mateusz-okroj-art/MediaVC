using System;
using System.Collections.Generic;
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
            var tasks = new List<Task>(10);

            using var utf8_file = new FileStream("TestData/utf-8 without bom.bin", FileMode.Open, FileAccess.Read);
            UTF8_Bytes = new byte[utf8_file.Length];
            tasks.Add(utf8_file.ReadAsync(UTF8_Bytes).AsTask());

            using var utf8bom_file = new FileStream("TestData/utf-8 with bom.bin", FileMode.Open, FileAccess.Read);
            UTF8BOM_Bytes = new byte[utf8bom_file.Length];
            tasks.Add(utf8bom_file.ReadAsync(UTF8BOM_Bytes).AsTask());

            using var utf32be_file = new FileStream("TestData/utf-32 big-endian with bom.bin", FileMode.Open, FileAccess.Read);
            UTF32BE_Bytes = new byte[utf32be_file.Length];
            tasks.Add(utf32be_file.ReadAsync(UTF32BE_Bytes).AsTask());

            using var utf32le_file = new FileStream("TestData/utf-32 little-endian with bom.bin", FileMode.Open, FileAccess.Read);
            UTF32LE_Bytes = new byte[utf32le_file.Length];
            tasks.Add(utf32le_file.ReadAsync(UTF32LE_Bytes).AsTask());

            using var utf16le_file = new FileStream("TestData/utf-16 little-endian with bom.bin", FileMode.Open, FileAccess.Read);
            UTF16LE_Bytes = new byte[utf16le_file.Length];
            tasks.Add(utf16le_file.ReadAsync(UTF16LE_Bytes).AsTask());

            using var utf16be_file = new FileStream("TestData/utf-16 big-endian with bom.bin", FileMode.Open, FileAccess.Read);
            UTF16BE_Bytes = new byte[utf16be_file.Length];
            tasks.Add(utf16be_file.ReadAsync(UTF16BE_Bytes).AsTask());

            using var utf8_crlf_file = new FileStream("TestData/utf-8 cr-lf.bin", FileMode.Open, FileAccess.Read);
            CRLF_UTF8_Bytes = new byte[utf8_crlf_file.Length];
            tasks.Add(utf8_crlf_file.ReadAsync(CRLF_UTF8_Bytes).AsTask());

            using var utf16_lf_file = new FileStream("TestData/utf-16 lf.bin", FileMode.Open, FileAccess.Read);
            LF_UTF16_Bytes = new byte[utf16_lf_file.Length];
            tasks.Add(utf16_lf_file.ReadAsync(LF_UTF16_Bytes).AsTask());

            Task.WaitAll(tasks.ToArray());

            UTF8_Content = Encoding.UTF8.GetString(UTF8_Bytes.Span);
            UTF8BOM_Content = Encoding.UTF8.GetString(UTF8BOM_Bytes.Span);
            UTF32BE_Content = UnicodeHelper.UTF32BigEndianEncoding.GetString(UTF32BE_Bytes.Span);
            UTF32LE_Content = Encoding.UTF32.GetString(UTF32LE_Bytes.Span);
            UTF16LE_Content = Encoding.Unicode.GetString(UTF16LE_Bytes.Span);
            UTF16BE_Content = Encoding.BigEndianUnicode.GetString(UTF16BE_Bytes.Span);

            using var stream1 = new MemoryStream(CRLF_UTF8_Bytes.ToArray());
            using var reader1 = new StreamReader(stream1);

            var tempList = new List<string>();
            string line = null;

            while((line = reader1.ReadLine()) is not null)
                tempList.Add(line);

            CRLF_UTF8_Content = tempList.ToArray();

            tempList.Clear();

            using var stream2 = new MemoryStream(LF_UTF16_Bytes.ToArray());
            using var reader2 = new StreamReader(stream2);

            while((line = reader2.ReadLine()) is not null)
                tempList.Add(line);

            LF_UTF16_Content = tempList.ToArray();
        }

        public Memory<byte> UTF16LE_Bytes { get; }

        public Memory<byte> UTF16BE_Bytes { get; }

        public Memory<byte> UTF32BE_Bytes { get; }

        public Memory<byte> UTF32LE_Bytes { get; }

        public Memory<byte> UTF8_Bytes { get; }

        public Memory<byte> UTF8BOM_Bytes { get; }

        public Memory<byte> CRLF_UTF8_Bytes { get; }

        public Memory<byte> LF_UTF16_Bytes { get; }

        public string UTF16LE_Content { get; }

        public string UTF16BE_Content { get; }

        public string UTF32BE_Content { get; }

        public string UTF32LE_Content { get; }

        public string UTF8_Content { get; }

        public string[] CRLF_UTF8_Content { get; }

        public string[] LF_UTF16_Content { get; }

        public string UTF8BOM_Content { get; }
    }
}
