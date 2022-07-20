namespace MediaVC
{
    public enum TextReadingState
    {
        /// <summary>
        /// Reading of character is done for current encoding.
        /// </summary>
        Done = 0,

        /// <summary>
        /// Further reading was expected based on length calculations.
        /// </summary>
        UnexpectedEndOfStream = 1,

        /// <summary>
        /// Current byte value is too high for encoding format.
        /// </summary>
        TooHighValueOfSegment = 2,

        /// <summary>
        /// Bad format for current character subsegment
        /// </summary>
        BadFormatSegment = 3
    }
}
