using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Miniframework.Backgroundservice.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleTests
{
    public class TestingHostedProcess : BackgroundService
    {
        private readonly ILogger<TestingHostedProcess> logger;
        private readonly IHostedProcess hostedProcess;

        public TestingHostedProcess(ILogger<TestingHostedProcess> logger, IHostedProcess hostedProcess)
        {
            this.logger = logger;
            this.hostedProcess = hostedProcess;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                hostedProcess.Stopwatch.Restart();

                logger.LogInformation("Someting is processing...");

                Thread.Sleep(30000);
                logger.LogDebug($"Elapsed: {hostedProcess.Stopwatch.ElapsedSeconds}");
            }
        }
    }
}
