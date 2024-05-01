using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WbExtensions.Application.Implementations.Alice.Converters;
using WbExtensions.Application.Interfaces.Alice;
using WbExtensions.Application.Interfaces.Home;
using WbExtensions.Domain.Alice;
using WbExtensions.Domain.Alice.Requests;

namespace WbExtensions.Application.Implementations.Alice;

internal sealed class AliceDevicesService : IAliceDevicesService
{
    private readonly IDevicesRepository _devicesRepository;

    public AliceDevicesService(IDevicesRepository devicesRepository)
    {
        _devicesRepository = devicesRepository;
    }

    public async Task<IList<Device>> GetAsync(CancellationToken cancellationToken)
    {
        var virtualDevices = await _devicesRepository.GetAsync(cancellationToken);

        return virtualDevices.ToDevices().ToList();
    }

    public async Task<IList<Device>> GetAsync(IReadOnlyCollection<string> ids, CancellationToken cancellationToken)
    {
        var virtualDevices = await _devicesRepository.GetAsync(ids, cancellationToken);

        return virtualDevices.ToDevices().ToList();
    }

    public Task<IList<Device>> UpdateDevicesStateAsync(List<SetUSerDevicesStateRequestItem> actions, CancellationToken cancellationToken)
    {
        // TODO: реализовать
        throw new System.NotImplementedException();
    }
}