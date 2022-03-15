using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using System.Net;
using System;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Miniframework.Backgroundservice.HealthCheck
{
    public sealed class TcpHealthProbeService : BackgroundService
    {

        private readonly HealthCheckService _healthCheckService;
        private readonly TcpListener _listener;
        private readonly ILogger _logger;

        public TcpHealthProbeService(HealthCheckService healthCheckService, ILogger<TcpHealthProbeService> logger, HealthCheckOptions options)
        {
            _healthCheckService = healthCheckService;
            _logger = logger;

            var port = options.Port > 0 ? options.Port : 5000;
            _listener = new TcpListener(IPAddress.Any, port);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Started health check service.");

            await Task.Yield();

            _listener.Start();

            while (!stoppingToken.IsCancellationRequested)
            {
                await UpdateHeartbeatAsync(stoppingToken);

                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            _listener.Stop();
        }

        private async Task UpdateHeartbeatAsync(CancellationToken token)
        {
            try
            {
                var result = await _healthCheckService.CheckHealthAsync(null, token);
                var isHealthy = result.Status == HealthStatus.Healthy;

                if (!isHealthy)
                {
                    _listener.Stop();

                    _logger.LogInformation("Service is unhealthy. Listener stopped.");
                    return;
                }

                _listener.Start();

                while (_listener.Server.IsBound && _listener.Pending())
                {
                    var client = await _listener.AcceptTcpClientAsync();

                    client.Close();

                    _logger.LogInformation("Successfully processed health check request.");
                }

                _logger.LogDebug("Heartbeat check executed.");
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An error occurred while checking heartbeat.");
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }
}
