using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WbExtensions.Application.Interfaces.Home;

namespace WbExtensions.Application.UseCases.GetRooms;

internal sealed class GetRoomsHandler : IRequestHandler<GetRoomsRequest, IReadOnlyCollection<string>>
{
    private readonly IVirtualDevicesRepository _virtualDevicesRepository;

    public GetRoomsHandler(IVirtualDevicesRepository virtualDevicesRepository)
    {
        _virtualDevicesRepository = virtualDevicesRepository;
    }

    public Task<IReadOnlyCollection<string>> Handle(GetRoomsRequest request, CancellationToken cancellationToken)
    {
        var devices = _virtualDevicesRepository.GetDevices();
        var rooms = devices
            .Select(d => d.Room)
            .Where(r => !string.IsNullOrEmpty(r))
            .Distinct()
            .ToArray();

        return Task.FromResult<IReadOnlyCollection<string>>(rooms!);
    }
}