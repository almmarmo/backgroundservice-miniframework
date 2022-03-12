using Miniframework.Backgroundservice;
using System;

namespace ConsoleTests
{
    class Program
    {
        static void Main(string[] args)
        {
            HostedProcess process = new HostedProcess(args);
            process.UseAppsettings()
                .UseLogging()
                .UseHealthCheck("hostedProcess")
                .AddHostedService<TestingHostedProcess>()
                .Run();
        }
    }
}
