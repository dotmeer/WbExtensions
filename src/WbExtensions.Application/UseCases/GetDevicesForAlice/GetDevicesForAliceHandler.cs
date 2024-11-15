using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WbExtensions.Application.Helpers.Alice.Converters;
using WbExtensions.Application.Interfaces.Home;
using WbExtensions.Domain.Alice;

namespace WbExtensions.Application.UseCases.GetDevicesForAlice;

internal sealed class GetDevicesForAliceHandler : IRequestHandler<GetDevicesForAliceRequest, IList<Device>>
{
    private readonly IVirtualDevicesRepository _virtualDevicesRepository;

    public GetDevicesForAliceHandler(IVirtualDevicesRepository virtualDevicesRepository)
    {
        _virtualDevicesRepository = virtualDevicesRepository;
    }

    public Task<IList<Device>> Handle(
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