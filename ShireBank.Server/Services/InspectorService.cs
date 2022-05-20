﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShireBank.Shared;
using ShireBank.Shared.Data.Models;
using ShireBank.Shared.Data.Repositories;

namespace ShireBank.Server.Services
{
    internal class InspectorService : Inspector.InspectorBase
    {
        private readonly ILogger<InspectorService> _logger;
        private readonly InspectionStateService _inspectionStateService;

        public InspectorService(ILogger<InspectorService> logger, InspectionStateService inspectionStateService)
        {
            _logger = logger;
            _inspectionStateService = inspectionStateService;
        }

        public override async Task GetFullSummary(GetFullSummaryRequest request, IServerStreamWriter<GetFullSummaryReply> responseStream, ServerCallContext context)
        {
            if (!_inspectionStateService.IsInspectionEnabled())
                throw new RpcException(new Status(StatusCode.Aborted, "System is not under inspection"));

            try
            {
                await foreach (var data in _inspectionStateService.InspectionReader.ReadAllAsync(context.CancellationToken))
                {
                    await responseStream.WriteAsync(new GetFullSummaryReply { Summary = $"{data.GetType().Name}: {data}" });
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogDebug("Client closed stream");
            }
        }

        public override Task<StartInspectionReply> StartInspection(StartInspectionRequest request, ServerCallContext context)
        {
            _inspectionStateService.StartInspection();
            return Task.FromResult(new StartInspectionReply());
        }

        public override Task<FinishInspectionReply> FinishInspection(FinishInspectionRequest request, ServerCallContext context)
        {
            _inspectionStateService.StopInspection();
            return Task.FromResult(new FinishInspectionReply());
        }
    }
}