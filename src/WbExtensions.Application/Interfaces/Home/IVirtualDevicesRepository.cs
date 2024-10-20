using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WbExtensions.Domain.Home;

namespace WbExtensions.Application.Interfaces.Home;

public interface IVirtualDevicesRepository
{
    IReadOnlyCollection<VirtualDevice> GetDevices();

    bool TryGetControl(string virtualDeviceName, string virtualControlName, out VirtualDevice? virtualDevice, out Control? control);

    Task InitAsync(CancellationToken cancellationToken);
}