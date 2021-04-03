using System;
using System.IO;
using System.Threading.Tasks;

namespace MediaVC.Tools
{
    /// <summary>
    /// Checks, that selected file is text-only
    /// </summary>
    public static class TextDetector
    {
        /// <summary>
        /// Checks, that selected bytes is text-only
        /// </summary>
        /// <param name="input">Array to be checked</param>
        public static bool CheckIsText(byte[] input)
        {

        }

        /// <summary>
        /// Checks, that selected bytes is text-only
        /// </summary>
        /// <param name="input">Memory block to be checked</param>
        public static bool CheckIsText(Memory<byte> input)
        {
            
        }

        /// <summary>
        /// Checks, that selected stream is text-only
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool CheckIsText(Stream input) => CheckIsTextAsync(input).Result;

        public static async ValueTask<bool> CheckIsTextAsync(Stream input)
        {

        }

        private static bool CheckSingleCharacter(byte character)
        {

        }
    }
}
