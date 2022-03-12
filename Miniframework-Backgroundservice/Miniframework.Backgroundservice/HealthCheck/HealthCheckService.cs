using Microsoft.Extensions.Diagnostics.HealthChecks;
using Miniframework.Backgroundservice.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Miniframework.Backgroundservice.HealthCheck
{
    public class HealthCheckService : IHealthCheck
    {
        private readonly HealthCheckOptions options;
        private readonly IHostedProcess hostedProcess;

        public HealthCheckService(HealthCheckOptions options, IHostedProcess hostedProcess)
        {
            this.options = options;
            this.hostedProcess = hostedProcess;
        }
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if(options.ProcessTimeoutSeconds.HasValue && hostedProcess.Stopwatch.ElapsedSeconds > options.ProcessTimeoutSeconds)
                return Task.FromResult(new HealthCheckResult(HealthStatus.Unhealthy));
            else
                return Task.FromResult(new HealthCheckResult(HealthStatus.Healthy));
        }
    }
}
