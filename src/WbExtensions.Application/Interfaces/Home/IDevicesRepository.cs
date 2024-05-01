using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WbExtensions.Domain.Home;

namespace WbExtensions.Application.Interfaces.Home;

public interface IDevicesRepository
{
    Task InitAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<VirtualDevice>> GetAsync(
        CancellationToken cancellationToken)
        => GetAsync(Array.Empty<string>(), cancellationToken);

    Task<IReadOnlyCollection<VirtualDevice>> GetAsync(
        IReadOnlyCollection<string> ids,
        CancellationToken cancellationToken);

    Task UpdateStateAsync(
        IReadOnlyCollection<Command> commands,
        CancellationToken cancellationToken);
}