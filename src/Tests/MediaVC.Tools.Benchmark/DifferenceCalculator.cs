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
        public static void Run()
        {
            var files = Array.Empty<FileStream>();

            try
            {
                WriteLine("1) Difference scanning of large files");
                WriteLine("   Generating large files...");
                files = GenerateFiles();

                WriteLine("Test with two files.");

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

            for(byte count = 0; count < 2; ++count)
            {
                var filename = Path.Combine(tempDirectory, $"{Guid.NewGuid()}.tmp");

                var file = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                file.SetLength(2_000_000_000);

                files[count] = file;
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

            stopwatch.Start();

            calculator.CalculateAsync(CancellationToken.None, progressReporter).AsTask().Wait();

            stopwatch.Stop();

            WriteLine($"Time: {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
