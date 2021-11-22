using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using MediaVC.Difference;

using static System.Console;

namespace MediaVC.Tools.Benchmark
{
    internal static class DifferenceCalculator
    {
        private const int TestedFileLength = 500_000;

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

                WriteLine("Test of SHA512 algorithm");
                C
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
            var tempDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));

            Parallel.For(0, files.Length, index =>
            {
                var filename = Path.Combine(tempDirectory, $"{Guid.NewGuid()}.tmp");

                var file = File.Create(filename, 2000, FileOptions.DeleteOnClose);
                file.SetLength(TestedFileLength);

                files[index] = file;

                for(long pos = 0; pos < 500_000; pos += 16)
                {
                    var bytes = Guid.NewGuid().ToByteArray();

                    file.Position = pos;

                    var percentage = MathF.Round((float)pos / 500_000 * 100, 1);
                    WriteLine($"File {index + 1}: {percentage}%");

                    file.Write(bytes, 0, bytes.Length);
                }

                file.Flush();
            });

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

            WriteLine($"Time: {stopwatch.Elapsed.TotalSeconds} s");
        }

        private static void Calc2(FileStream file)
        {
            file.Position = 0;

            var bytes = new byte[file.Length];
            file.Read(bytes, 0, bytes.Length);

            var stopwatch = new Stopwatch();

            stopwatch.Start();
            System.Security.Cryptography.SHA512.HashData(bytes);
            stopwatch.Stop();

            Console.WriteLine($"Time: {stopwatch.Elapsed.Seconds}");
        }
    }
}
