using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Miniframework.Backgroundservice.HealthCheck;
using Miniframework.Backgroundservice.Abstractions;

namespace Miniframework.Backgroundservice
{
    public class HostedProcess : IHostedProcess
    {
        private const string APPSETTINGS_FILENAME = "appsettings.json";
        private const string NLOG_SECTION_NAME = "NLog";

        private IConfigurationBuilder configurationBuilder;
        private IConfigurationRoot configurationRoot;
        private IHostBuilder hostBuilder;
        private IStopwatchProcess stopwatch;

        public HostedProcess(string[] args)
        {
            configurationBuilder = new ConfigurationBuilder()
                                    .SetBasePath(AppContext.BaseDirectory);

            hostBuilder = Host.CreateDefaultBuilder(args);
            UseServices(s => s.AddSingleton<IHostedProcess>(this));
            stopwatch = new StopwatchProcess();
        }

        public IStopwatchProcess Stopwatch { get { return stopwatch; } }

        public HostedProcess UseLogging()
        {
            return UseLogging(NLOG_SECTION_NAME);
        }

        public HostedProcess UseHealthCheck(HealthCheckOptions options)
        {
            UseServices(s => s.AddSingleton(options));
            if (options.ProcessTimeoutSeconds.HasValue && options.ProcessTimeoutSeconds.Value > 0)
                stopwatch.Start();

            UseServices(s => s.AddHealthChecks().AddCheck<HealthCheckService>("health_check"));
            return this;

        }

        public HostedProcess UseLogging(string nlogSectionName)
        {
            if (configurationRoot == null)
                BuildConfiguration();

            hostBuilder.ConfigureLogging((hostContext, loggingBuilder) =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                var section = configurationRoot.GetSection(nlogSectionName);
                var nlogCofig = new NLogLoggingConfiguration(section);
                loggingBuilder.AddNLog(nlogCofig);
            });

            return this;
        }

        public HostedProcess UseServices(Action<IServiceCollection> action)
        {
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                action.Invoke(services);
            });

            return this;
        }

        public HostedProcess AddHostedService<T>() where T : class, IHostedService
        {
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<T>();
            });

            return this;
        }

        public HostedProcess UseAppsettings()
        {
            return UseAppsettings(APPSETTINGS_FILENAME);
        }

        public HostedProcess UseAppsettings(string appSettingsFileName)
        {
            configurationBuilder = new ConfigurationBuilder()
                                    .SetBasePath(AppContext.BaseDirectory)
                                    .AddJsonFile(appSettingsFileName, optional: true, reloadOnChange: true)
                                    .AddEnvironmentVariables();
            return this;
        }

        public void Run()
        {
            var host = hostBuilder
                .Build();

            host.Run();
        }

        public async Task RunAsync()
        {
            var host = hostBuilder
                .Build();

            await host.RunAsync();
        }

        private void BuildConfiguration()
        {
            configurationRoot = configurationBuilder.Build();
        }
    }
}
