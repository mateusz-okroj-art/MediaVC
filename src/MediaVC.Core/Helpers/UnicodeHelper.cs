using System.Runtime.CompilerServices;

namespace MediaVC.Helpers
{
    public static class UnicodeHelper
    {
        /// <summary>
        /// Validates char value for UTF16
        /// </summary>
        /// <param name="value">UTF16 char value</param>
        /// <remarks>Source: https://github.com/dotnet/corefx/blob/master/src/Common/src/CoreLib/System/Text/UnicodeUtility.cs</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSurrogateCodePoint(uint value) => value is 0xD800U or 0xDFFFU or > 0xD800U and < 0xDFFFU;
    }
}
