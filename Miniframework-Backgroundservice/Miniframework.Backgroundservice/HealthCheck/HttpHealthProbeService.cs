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
    public sealed class HttpHealthProbeService : BackgroundService
    {

        private readonly HealthCheckService _healthCheckService;
        private readonly HttpListener _listener;
        private readonly ILogger _logger;

        public HttpHealthProbeService(HealthCheckService healthCheckService, ILogger<HttpHealthProbeService> logger, HealthCheckOptions options)
        {
            _healthCheckService = healthCheckService;
            _logger = logger;

            var port = options.Port > 0 ? options.Port : 5000;
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://localhost:{port}/");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Started health check service.");


            while (!stoppingToken.IsCancellationRequested)
            {
                _listener.Start();

                await UpdateHeartbeatAsync(stoppingToken);

                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            _listener.Stop();
            _logger.LogInformation("Health check http probe stoped.");
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

                if (_listener.IsListening)
                {
                    var context = await _listener.GetContextAsync();
                    var request = context.Request;

                    HttpListenerResponse response = context.Response;
                    string responseString = "HealthCheck";
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                    response.ContentLength64 = buffer.Length;

                    using (System.IO.Stream output = response.OutputStream)
                    {
                        output.Write(buffer, 0, buffer.Length);
                        output.Close();
                    }
                    //_listener.Stop();

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
