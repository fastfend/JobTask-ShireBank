using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ShireBank.Shared.Data.Interfaces;

namespace ShireBank.Server.Services;

/// <summary>
/// Manages inspection state and handles task suspension for inspection
/// </summary>
public class InspectionStateService
{
    private readonly ILogger<InspectionStateService> _logger;

    private readonly Channel<IInspectable> channel = Channel.CreateUnbounded<IInspectable>();

    private readonly SemaphoreSlim semaphore = new(0);
    private volatile int inspectedCount;
    private volatile bool underInspection;

    public InspectionStateService(ILogger<InspectionStateService> logger)
    {
        _logger = logger;
    }

    public ChannelReader<IInspectable> InspectionReader => channel.Reader;


    /// <summary>
    /// Enables inspection mode
    /// </summary>
    public void StartInspection()
    {
        _logger.LogInformation("Starting inspection mode");
        underInspection = true;
    }

    /// <summary>
    /// Disables inspection mode and resumes suspended tasks
    /// </summary>
    public void StopInspection()
    {
        _logger.LogInformation("Stopping inspection mode");
        underInspection = false;
        semaphore.Release(inspectedCount);
    }

    /// <summary>
    /// Returns if inspection mode is enabled
    /// </summary>
    /// <returns><seealso cref="bool">true</seealso> if enabled, <seealso cref="bool">false</seealso> if disabled</returns>
    public bool IsInspectionEnabled()
    {
        return underInspection;
    }

    /// <summary>
    /// If system is under inspection adds obj to inspection channel
    /// </summary>
    /// <param name="obj">Object to inspect</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task that returns if inspection was done</returns>
    public async Task<bool> TryInspect(IInspectable obj, CancellationToken cancellationToken)
    {
        if (!underInspection) return false;

        await channel.Writer.WriteAsync(obj, cancellationToken);
        Interlocked.Increment(ref inspectedCount);
        await semaphore.WaitAsync(cancellationToken);
        Interlocked.Decrement(ref inspectedCount);

        return true;
    }
}