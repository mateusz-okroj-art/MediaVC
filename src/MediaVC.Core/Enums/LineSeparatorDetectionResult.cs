namespace MediaVC
{
    internal enum LineSeparatorDetectionResult
    {
        /// <summary>
        /// Characters is not line separator.
        /// </summary>
        NotDetected,

        /// <summary>
        /// Detected only first character
        /// </summary>
        HalfDetected,

        /// <summary>
        /// Detected all characters
        /// </summary>
        FullDetected,

        /// <summary>
        /// Detected empty line after last line separator.
        /// </summary>
        LastEmptyLine
    }
}
