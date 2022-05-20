using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using ShireBank.Shared;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace ShireBank.Server;

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
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile("appsettings.dev.json", true)
                .Build();

            var host = new WebHostBuilder()
                .UseKestrel(options => { options.ConfigureEndpointDefaults(lo => lo.Protocols = HttpProtocols.Http2); })
                .UseUrls(Constants.BankBaseAddress)
                .UseConfiguration(config)
                .UseNLog()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
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