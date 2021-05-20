using System;
using System.IO;

namespace MediaVC.Tools.Tests.Detection.TextDetector
{
    public interface ITextDetectorFixture : IDisposable
    {
        Memory<byte> Memory_Text1 { get; }
        Memory<byte> Memory_Text2 { get; }
        Memory<byte> Memory_NonText1 { get; }
        Memory<byte> Memory_NonText2 { get; }

        Stream Stream_Text1 { get; }
        Stream Stream_Text2 { get; }
        Stream Stream_NonText1 { get; }
        Stream Stream_NonText2 { get; }
    }
}