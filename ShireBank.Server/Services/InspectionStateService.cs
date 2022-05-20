using Microsoft.Extensions.Logging;
using ShireBank.Server.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ShireBank.Server.Services
{
    public class InspectionStateService
    {
        private readonly ILogger<InspectionStateService> _logger;

        private readonly Channel<object> channel = Channel.CreateUnbounded<object>();
        public ChannelReader<object> InspectionReader { get { return channel.Reader; } }

        private readonly SemaphoreSlim semaphore = new(0);
        private volatile int inspectedCount = 0;
        private volatile bool underInspection = false;

        public InspectionStateService(ILogger<InspectionStateService> logger)
        {
            _logger = logger;
        }

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
            if(!underInspection) return false;

            await channel.Writer.WriteAsync(obj, cancellationToken);
            Interlocked.Increment(ref inspectedCount);
            await semaphore.WaitAsync(cancellationToken);
            Interlocked.Decrement(ref inspectedCount);
        
            return true;
        }
    }
}
