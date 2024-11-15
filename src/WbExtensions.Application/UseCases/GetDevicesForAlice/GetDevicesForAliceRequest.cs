using System.Collections.Generic;
using MediatR;
using WbExtensions.Domain.Alice;

namespace WbExtensions.Application.UseCases.GetDevicesForAlice;

public sealed record GetDevicesForAliceRequest(
    IReadOnlyCollection<string>? Ids = null)
    : IRequest<IList<Device>>;