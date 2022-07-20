namespace MediaVC.Difference
{
    /// <summary>
    /// Provides information about current progress of <see cref="DifferenceCalculator"/>
    /// </summary>
    public interface IDifferenceCalculatorProgress
    {
        void ReportLeftMainPosition(long position);

        void ReportRightMainPosition(long position);

        void ReportLeftOffsetedPosition(long position);

        void ReportRightOffsetedPosition(long position);

        void ReportProcessState(ProcessState state);
    }
}