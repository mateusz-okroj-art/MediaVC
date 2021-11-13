using System;

using MediaVC.Difference;

namespace MediaVC.Tools.Benchmark
{
    internal class ConsoleProgressReporter : IDifferenceCalculatorProgress
    {
        private long leftLength;
        private long rightLength;

        public void ReportLeftMainPosition(long position) => Console.WriteLine($"File 1: {position} / {leftLength}");

        public void ReportRightMainPosition(long position) => Console.WriteLine($"File 2: {position} / {rightLength}");

        public void ReportLeftLength(long length) => this.leftLength = length;

        public void ReportRightLength(long length) => this.rightLength = length;

        public void ReportLeftOffsetedPosition(long position) => Console.WriteLine($"File 1 (offseted): {position} / {leftLength}");

        public void ReportRightOffsetedPosition(long position) => Console.WriteLine($"File 2 (offseted): {position} / {rightLength}");

        public void ReportProcessState(ProcessState state) => Console.WriteLine($"State: {state}");
    }
}
