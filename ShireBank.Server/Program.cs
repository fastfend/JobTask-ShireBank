
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Web;
using System;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Threading;

namespace ShireBank.Server
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var logger = LogManager
                .Setup()
                .LoadConfigurationFromAppSettings()
                .GetCurrentClassLogger();

            try
            {
                var config = new ConfigurationBuilder()
                    .AddCommandLine(args)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile("appsettings.dev.json", optional: true)
                    .Build();

                var host = new WebHostBuilder()
                    .UseKestrel(options =>
                    {
                        options.ConfigureEndpointDefaults(lo => lo.Protocols = HttpProtocols.Http2);
                    })
                    .UseUrls(Shared.Constants.BankBaseAddress)
                    .UseConfiguration(config)
                    .UseNLog()
                    .ConfigureLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    })
                    .UseStartup<Startup>()
                    .Build();

                host.Run();
            }
            catch (Exception e)
            {
                logger.Error(e, "Stopped program because of exception");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }
    }
}