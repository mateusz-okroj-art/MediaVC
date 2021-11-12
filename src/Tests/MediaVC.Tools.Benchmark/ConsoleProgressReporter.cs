using System;

namespace MediaVC.Tools.Benchmark
{
    internal class ConsoleProgressReporter : IProgress<float>
    {
        public void Report(float value) => Console.WriteLine($"{value * 100}%");
    }
}
