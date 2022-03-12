namespace Miniframework.Backgroundservice.Abstractions
{
    public interface IStopwatchProcess
    {
        decimal ElapsedSeconds { get; }
        bool IsRunning { get; }

        void Reset();
        void Start();
        void Stop();
    }
}