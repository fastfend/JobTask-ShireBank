using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ShireBank.Server.Services;

public class InspectionStateService
{
    private readonly ILogger<InspectionStateService> _logger;

    private readonly Channel<object> channel = Channel.CreateUnbounded<object>();

    private readonly SemaphoreSlim semaphore = new(0);
    private volatile int inspectedCount;
    private volatile bool underInspection;

    public InspectionStateService(ILogger<InspectionStateService> logger)
    {
        _logger = logger;
    }

    public ChannelReader<object> InspectionReader => channel.Reader;

    public void StartInspection()
    {
        _logger.LogInformation("Starting inspection mode");
        underInspection = true;
    }

    public void StopInspection()
    {
        _logger.LogInformation("Stopping inspection mode");
        underInspection = false;
        semaphore.Release(inspectedCount);
    }

    public bool IsInspectionEnabled()
    {
        return underInspection;
    }

    public async Task<bool> TryInspect(object obj, CancellationToken cancellationToken)
    {
        if (!underInspection) return false;

        await channel.Writer.WriteAsync(obj, cancellationToken);
        Interlocked.Increment(ref inspectedCount);
        await semaphore.WaitAsync(cancellationToken);
        Interlocked.Decrement(ref inspectedCount);

        return true;
    }
}