﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WbExtensions.Domain.Alice;
using WbExtensions.Domain.Alice.Requests;

namespace WbExtensions.Application.Interfaces.Alice;

public interface IAliceDevicesService
{
    Task<IList<Device>> GetAsync(
        CancellationToken cancellationToken);

    Task<IList<Device>> GetAsync(
        IReadOnlyCollection<string> ids, CancellationToken cancellationToken);

    Task<IList<Device>> UpdateDevicesStateAsync(
        List<SetUSerDevicesStateRequestItem> actions,
        CancellationToken cancellationToken);
}