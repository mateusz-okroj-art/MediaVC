using System;

namespace MediaVC.Difference
{
    /// <summary>
    /// Provides information about segment of data.
    /// </summary>
    public interface IFileSegmentInfo : IEquatable<IFileSegmentInfo>
    {
        /// <summary>
        /// Start position of segment in source.
        /// </summary>
        long StartPositionInSource { get; set; }

        /// <summary>
        /// Last position of segment in source.
        /// </summary>
        long EndPositionInSource { get; set; }

        /// <summary>
        /// Source of data.
        /// </summary>
        IInputSource? Source { get; set; }

        /// <summary>
        /// Difference between positions in source.
        /// </summary>
        ulong Length { get; }

        /// <summary>
        /// <para>When <see cref="FileSegmentInfo"/> is grouped in other <see cref="InputSource"/> this value is position of segment in new created data source.</para>
        /// <para>Otherwise can be -1 (value is not respected).</para>
        /// </summary>
        long MappedPosition { get; set; }
    }
}
