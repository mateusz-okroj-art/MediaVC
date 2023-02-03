namespace MediaVC.Difference.Concurrency
{
    public interface IInputSourceConcurrentProvider : IInputSource
    {
        SynchronizationObject SynchronizationObject { get; }
    }
}
