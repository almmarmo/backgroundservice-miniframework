using Miniframework.Backgroundservice.Abstractions;
using System.Diagnostics;

namespace Miniframework.Backgroundservice
{
    public class StopwatchProcess : IStopwatchProcess
    {
        private Stopwatch stopwatch;

        public StopwatchProcess()
        {
            stopwatch = new Stopwatch();
        }

        public decimal ElapsedSeconds { get { return stopwatch.ElapsedMilliseconds / 1000; } }

        public bool IsRunning { get { return stopwatch.IsRunning; } }

        public void Start()
        {
            stopwatch.Start();
        }

        public void Stop()
        {
            stopwatch.Stop();
        }

        public void Reset()
        {
            stopwatch.Reset();
        }
    }
}
