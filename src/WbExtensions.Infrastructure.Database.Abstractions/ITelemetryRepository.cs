using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WbExtensions.Domain;

namespace WbExtensions.Infrastructure.Database.Abstractions;

public interface ITelemetryRepository
{
    Task AddAsync(Telemetry model, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Telemetry>> GetAsync(CancellationToken cancellationToken);
}