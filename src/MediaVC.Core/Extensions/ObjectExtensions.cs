using System;
using System.Collections.Generic;

namespace MediaVC
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Generates new enumerator with single value.
        /// </summary>
        /// <param name="value">Value for return.</param>
        /// <returns>New enumerator with single value.</returns>
        /// <exception cref="ArgumentNullException" />
        public static IEnumerable<T> Yield<T>(this T value)
        {
            ArgumentNullException.ThrowIfNull(value);

            yield return value;
        }
    }
}
