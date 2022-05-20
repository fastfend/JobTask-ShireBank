using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using ShireBank.Server.Services;
using ShireBank.Shared;

namespace ShireBank.Server.Interceptors;

internal class InspectionInterceptor : Interceptor
{
    private readonly InspectionStateService _inspectionStateService;
    private readonly ILogger<InspectionInterceptor> _logger;

    public InspectionInterceptor(ILogger<InspectionInterceptor> logger, InspectionStateService inspectionStateService)
    {
        _logger = logger;
        _inspectionStateService = inspectionStateService;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            var requestType = request.GetType();

            if (requestType != typeof(FinishInspectionRequest) &&
                requestType != typeof(StartInspectionRequest))
                await _inspectionStateService.TryInspect(request, context.CancellationToken);

            return await continuation(request, context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error thrown by {context.Method}.");
            throw;
        }
    }
}