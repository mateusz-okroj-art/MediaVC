﻿using System;
using System.Diagnostics;

using static System.Console;

namespace MediaVC.Tools.Benchmark
{
    internal static class ChecksumCalculator
    {
        private const int testedDataLength = 500_000_000;

        public static void Run()
        {
            WriteLine("Checksum calculating.");
            WriteLine("   Generating data...");
            var data = GenerateData();

            WriteLine("Calculating...");
            Calc(data.Span);
        }

        private static Memory<byte> GenerateData()
        {
            Memory<byte> data = new byte[testedDataLength];
            var index = System.Security.Cryptography.RandomNumberGenerator.GetInt32(data.Length);
            data.Span[index] = 255;

            return data;
        }

        private static void Calc(ReadOnlySpan<byte> data)
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var result = System.Security.Cryptography.SHA512.HashData(data);
            stopwatch.Stop();

            WriteLine($"Length: {data.Length}");
            WriteLine($"Time: {stopwatch.Elapsed.Seconds} s");

            WriteLine("Result:");
            foreach(var b in result)
                Write($"{b} ");
        }
    }
}
