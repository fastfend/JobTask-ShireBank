using Grpc.Core;
using Grpc.Net.Client;
using NLog;
using ShireBank.Shared;
using ShireBank.Shared.Protos;

namespace ShireBank.Inspector;

/// <summary>
/// Inspector app which suspends client operations and prints them in console.
/// After finishing resumes suspended operations
/// </summary>
internal static class Program
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private static async Task Main()
    {
        try
        {
            logger.Info("Starting inspector...");

            using var channel = GrpcChannel.ForAddress(Constants.BankBaseAddress, new GrpcChannelOptions
            {
                HttpHandler = new SocketsHttpHandler
                {
                    EnableMultipleHttp2Connections = true // Enabled client to use more TCP connections 
                }
            });

            var inspector = new Shared.Protos.Inspector.InspectorClient(channel);
            await inspector.StartInspectionAsync(new StartInspectionRequest());

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            var call = inspector.GetFullSummary(new GetFullSummaryRequest(), cancellationToken: token);

            logger.Info("Inspecting. Press any key to stop...");

            _ = Task.Run(async () =>
            {
                await foreach (var response in call.ResponseStream.ReadAllAsync(token))
                    logger.Info($"Inspected {response.Summary}");
            }, token);

            Console.ReadKey();
            tokenSource.Cancel();

            await inspector.FinishInspectionAsync(new FinishInspectionRequest());
            logger.Info("Finished inspecting. Press any key to exit...");

            Console.ReadKey();
        }
        catch (Exception ex)
        {
            logger.Error("Fatal error", ex);
        }
    }
}