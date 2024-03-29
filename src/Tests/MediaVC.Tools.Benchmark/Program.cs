﻿using System.Runtime.CompilerServices;

using static System.Console;

[assembly: InternalsVisibleTo("MediaVC.Tests")]

namespace MediaVC.Tools.Benchmark
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            WriteLine("MediaVC Tools Benchmark Tests");
            WriteLine();

            if(args.Length != 1)
            {
                WriteLine("Bad argument!");
                return;
            }

            switch(args[0].Trim())
            {
                case "1":
                    DifferenceCalculator.Run();
                    break;

                case "2":
                    ChecksumCalculator.Run();
                    break;
            }
        }
    }
}
