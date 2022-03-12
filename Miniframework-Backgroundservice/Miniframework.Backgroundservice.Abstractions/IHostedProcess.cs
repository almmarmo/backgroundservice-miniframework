using System.Threading.Tasks;

namespace Miniframework.Backgroundservice.Abstractions
{
    public interface IHostedProcess
    {
        IStopwatchProcess Stopwatch { get; }

        void Run();
        Task RunAsync();
    }
}