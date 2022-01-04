namespace MediaVC
{
    public interface IExternalLoopControllerSource
    {
        IExternalLoopController Controller { get; }

        bool IsBreakRequested { get; }
    }
}