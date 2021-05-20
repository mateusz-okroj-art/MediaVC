﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaVC.Tools.Tests.Detection.TextDetector
{
    public sealed class TextDetectorFixture : ITextDetectorFixture
    {
        #region Constructor

        public TextDetectorFixture()
        {
            var dataText = new byte[100_000];
            _ = Parallel.For(0, 100_000, index => dataText[index] = (byte)(index % 128));

            Memory_Text2 = new Memory<byte>(dataText);
            Stream_Text2 = new MemoryStream(dataText);

            var dataNonText = new byte[100_001];

            Memory_NonText2 = new Memory<byte>(dataNonText);
            Stream_NonText2 = new MemoryStream(dataNonText);
        }

        #endregion

        #region Properties

        public Memory<byte> Memory_Text1 { get; } = new Memory<byte>(new byte[1]);

        public Memory<byte> Memory_Text2 { get; }

        public Memory<byte> Memory_NonText1 { get; } = new Memory<byte>(new byte[]{ 255 });

        public Memory<byte> Memory_NonText2 { get; }

        public Stream Stream_Text1 { get; } = new MemoryStream(new byte[1]);

        public Stream Stream_Text2 { get; }

        public Stream Stream_NonText1 { get; } = new MemoryStream(new byte[] { 128 });

        public Stream Stream_NonText2 { get; }

        #endregion

        #region Methods

        public void Dispose()
        {
            Stream_Text1.Close();
            Stream_Text2.Close();
            Stream_NonText1.Close();
            Stream_NonText2.Close();
        }

        #endregion
    }
}
