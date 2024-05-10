using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using WbExtensions.Application.Interfaces.Home;
using WbExtensions.Domain.Home;

namespace WbExtensions.Infrastructure.Home;

internal sealed class DevicesRepository : IDevicesRepository
{
    public DevicesRepository(IConfiguration configuration)
    {
        VirtualDevices = configuration
                             .GetSection("Schema:Devices")
                             .Get<IReadOnlyCollection<VirtualDevice>>()
                         ?? throw new ArgumentNullException(nameof(VirtualDevices));
    }

    public IReadOnlyCollection<VirtualDevice> VirtualDevices { get; }
}