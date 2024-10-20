﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WbExtensions.Application.Helpers.Alice.Converters;
using WbExtensions.Application.Interfaces.Home;
using WbExtensions.Domain.Alice;

namespace WbExtensions.Application.UseCases.GetDevicesForAlice;

public sealed class GetDevicesForAliceHandler
{
    private readonly IVirtualDevicesRepository _virtualDevicesRepository;

    public GetDevicesForAliceHandler(IVirtualDevicesRepository virtualDevicesRepository)
    {
        _virtualDevicesRepository = virtualDevicesRepository;
    }

    public Task<IList<Device>> HandleAsync(
        GetDevicesForAliceRequest request,
        CancellationToken cancellationToken)
    {
        var query = _virtualDevicesRepository.GetDevices()
            .AsEnumerable();

        if (request.Ids is not null && request.Ids.Any())
        {
            query = query
                .Where(d => request.Ids.Contains(d.Id));
        }

        return Task.FromResult<IList<Device>>(
            query
                .ToDevices()
                .ToList());
    }
}