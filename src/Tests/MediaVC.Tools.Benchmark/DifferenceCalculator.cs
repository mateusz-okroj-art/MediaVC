using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MediaVC.Difference;

using static System.Console;

namespace MediaVC.Tools.Benchmark
{
    internal static class DifferenceCalculator
    {
        private const int TestedFileLength = 200_000_000;

        public static void Run()
        {
            FileStream[] files = null;

            try
            {
                WriteLine("1) Difference scanning of large files");
                WriteLine("   Generating large files...");
                files = GenerateFiles();

                WriteLine("Test with two files.");
                Calc1(files[0], files[1]);
            }
            finally
            {
                if(files is not null)
                {
                    foreach(var file in files)
                        file?.Dispose();
                }
            }
        }

        private static FileStream[] GenerateFiles()
        {
            var files = new FileStream[2];
            var tempDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "temp");

            for(var index = 0; index < files.Length; ++index)
            {
                var filename = Path.Combine(tempDirectory, $"{Guid.NewGuid()}.tmp");

                var file = File.Create(filename, 2000, FileOptions.DeleteOnClose);
                file.SetLength(TestedFileLength);
                file.Lock(0, TestedFileLength);

                files[index] = file;

                //var streamLocker = new object();

                /*var lastPercentage = 0.0;

                Parallel.For(0, 125_000_001, posIndex =>
                {
                    var bytes = Guid.NewGuid().ToByteArray();

                    lock(streamLocker)
                    {
                        file.Position = 16 * posIndex;

                        var percentage = Math.Round(((double)posIndex / 125_000_000) * 100, 1);

                        if(Math.Abs(lastPercentage - percentage) > 0.1)
                        {
                            WriteLine($"File {index + 1}: {percentage}%");
                            lastPercentage = percentage;
                        }

                        file.Write(bytes, 0, bytes.Length);
                    }
                });*/

                file.Flush();
                file.Unlock(0, TestedFileLength);
            }

            return files;
        }

        private static void Calc1(FileStream file1, FileStream file2)
        {
            var currentVersionSource = new InputSource(file1);
            var newVersionSource = new InputSource(file2);

            var progressReporter = new ConsoleProgressReporter();
            var calculator = new Difference.DifferenceCalculator(currentVersionSource, newVersionSource);
            var stopwatch = new Stopwatch();

            progressReporter.ReportLeftLength(file1.Length);
            progressReporter.ReportRightLength(file2.Length);

            stopwatch.Start();

            calculator.CalculateAsync(CancellationToken.None, progressReporter).AsTask().Wait();

            stopwatch.Stop();

            WriteLine($"Time: {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
