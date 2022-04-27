using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaVC.Tests.Extensions.Helpers
{
    public static class RandomGenerator
    {
        /// <summary>
        /// Generates random value in range.
        /// </summary>
        /// <returns>Generated random value</returns>
        /// <remarks><paramref name="to"/> must be equal or greather than <paramref name="from"/>.</remarks>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static Tvalue Generate<Tvalue>(Tvalue from, Tvalue to)
            where Tvalue : struct, IComparable<Tvalue>
        {
            if(from.CompareTo(to) > 0)
                throw new ArgumentOutOfRangeException(nameof(from));

            var random = new Random();

            var difference = (dynamic)to - from;

            if(difference == 0)
                return from;

            var percent = random.Next(0, 100);
            return (Tvalue)(from + difference * (percent / 100.0));
        }
    }
}
