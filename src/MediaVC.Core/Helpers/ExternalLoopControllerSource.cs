namespace MediaVC
{
    public class ExternalLoopControllerSource : IExternalLoopControllerSource
    {
        public ExternalLoopControllerSource() => Controller = new ExternalLoopController(this);

        public IExternalLoopController Controller { get; }

        public bool IsBreakRequested { get; internal set; } = false;
    }
}
