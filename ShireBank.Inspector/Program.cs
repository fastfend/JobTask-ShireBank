using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using ShireBank.Shared;

namespace ShireBank.Inspector
{
    internal class Program
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private static async Task Main(string[] args)
        {
            try
            {
                _logger.Info("Starting inspector...");

                using var channel = GrpcChannel.ForAddress(Constants.BankBaseAddress, new GrpcChannelOptions
                {
                    HttpHandler = new SocketsHttpHandler
                    {
                        EnableMultipleHttp2Connections = true,
                    }
                });

                var inspector = new Shared.Inspector.InspectorClient(channel);
                await inspector.StartInspectionAsync(new StartInspectionRequest());

                var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;
                var call = inspector.GetFullSummary(new GetFullSummaryRequest(), cancellationToken: token);

                _logger.Info("Inspecting. Press any key to stop...");

                _ = Task.Run(async () =>
                {
                    await foreach (var response in call.ResponseStream.ReadAllAsync())
                    {
                        _logger.Info($"Inspected {response.Summary}");
                    }
                });

                Console.ReadKey();
                tokenSource.Cancel();

                await inspector.FinishInspectionAsync(new FinishInspectionRequest());
                _logger.Info("Finished inspecting. Press any key to exit...");

                Console.ReadKey();
            }
            catch (Exception ex)
            {
                _logger.Error("Fatal error", ex);
            }
        }
    }
}