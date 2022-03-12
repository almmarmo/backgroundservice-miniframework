using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Miniframework.Backgroundservice
{
    public class HostedProcess
    {
        private const string APPSETTINGS_FILENAME = "appsettings.json";
        private const string NLOG_SECTION_NAME = "NLog";

        private IConfigurationBuilder configurationBuilder;
        private IConfigurationRoot configurationRoot;
        private IHostBuilder hostBuilder;

        public HostedProcess(string[] args)
        {
            configurationBuilder = new ConfigurationBuilder()
                                    .SetBasePath(AppContext.BaseDirectory);

            hostBuilder = Host.CreateDefaultBuilder(args);
        }

        public HostedProcess UseLogging()
        {
            return UseLogging(NLOG_SECTION_NAME);
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
            hostBuilder.ConfigureServices((hostContext, services) => {
                action.Invoke(services);
            });

            return this;
        }

        public HostedProcess AddHostedService<T>() where T : class, IHostedService
        {
            hostBuilder.ConfigureServices((hostContext, services) => {
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
            hostBuilder
                .Build()
                .Run();
        }

        public async Task RunAsync()
        {
            await hostBuilder
                .Build()
                .RunAsync();
        }

        private void BuildConfiguration()
        {
            configurationRoot = configurationBuilder.Build();
        }
    }
}
