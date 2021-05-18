using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaVC.Tools.Tests.Detection.TextDetector
{
    public sealed class TextDetectorFixture : ITextDetectorFixture
    {
        public Memory<byte> Memory_Text1 { get; } = new Memory<byte>(new byte[1]);


    }
}
