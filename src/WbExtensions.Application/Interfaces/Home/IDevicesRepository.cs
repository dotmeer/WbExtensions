using System.Collections.Generic;
using WbExtensions.Domain.Home;

namespace WbExtensions.Application.Interfaces.Home;

public interface IDevicesRepository
{
    IReadOnlyCollection<VirtualDevice> VirtualDevices { get; }
}