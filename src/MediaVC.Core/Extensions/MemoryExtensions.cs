using System;
using System.Collections.Generic;

namespace MediaVC
{
    public static class MemoryExtensions
    {
        /// <summary>
        /// Splits one memory segment to many with selected max length.
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="source">Source segment</param>
        /// <param name="segmentMaxLength">Max length per segment</param>
        /// <returns>Separated segments</returns>
		public static IEnumerable<Memory<T>> Split<T>(this Memory<T> source, int segmentMaxLength)
		{
			if(segmentMaxLength < 1)
                yield break;

            for(var position = 0; position<source.Length; position += segmentMaxLength)
            {
                yield return position + segmentMaxLength >= source.Length?
                    source[position..] :
                    source.Slice(position, segmentMaxLength);
            }
        }

        /// <summary>
        /// Splits one memory segment to many with selected max length.
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="source">Source segment</param>
        /// <param name="segmentMaxLength">Max length per segment</param>
        /// <returns>Separated segments</returns>
		public static IEnumerable<ReadOnlyMemory<T>> Split<T>(this ReadOnlyMemory<T> source, int segmentMaxLength)
        {
            if(segmentMaxLength < 1)
                yield break;

            for(var position = 0; position < source.Length; position += segmentMaxLength)
            {
                yield return position + segmentMaxLength >= source.Length ?
                    source[position..] :
                    source.Slice(position, segmentMaxLength);
            }
        }
    }
}