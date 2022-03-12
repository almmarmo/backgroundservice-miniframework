namespace Miniframework.Backgroundservice.Abstractions
{
    public interface IStopwatchProcess
    {
        decimal ElapsedSeconds { get; }
        bool IsRunning { get; }

        void Restart();
        void Start();
        void Stop();
    }
}