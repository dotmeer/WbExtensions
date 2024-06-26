﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WbExtensions.Domain;

namespace WbExtensions.Application.Interfaces.Database;

public interface ITelemetryRepository
{
    Task UpsertAsync(Telemetry model, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Telemetry>> GetAsync(CancellationToken cancellationToken);
}