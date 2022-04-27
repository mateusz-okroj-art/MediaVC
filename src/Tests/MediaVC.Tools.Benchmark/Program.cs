using static System.Console;

namespace MediaVC.Tools.Benchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            WriteLine("MediaVC Tools Benchmark Tests");
            WriteLine();

            DifferenceCalculator.Run();
        }
    }
}
