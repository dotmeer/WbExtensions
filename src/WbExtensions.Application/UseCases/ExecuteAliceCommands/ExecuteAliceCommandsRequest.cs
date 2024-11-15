using System.Collections.Generic;
using MediatR;
using WbExtensions.Domain.Alice;
using WbExtensions.Domain.Alice.Requests;

namespace WbExtensions.Application.UseCases.ExecuteAliceCommands;

public sealed record ExecuteAliceCommandsRequest(
    IReadOnlyCollection<SetUSerDevicesStateRequestItem> Actions)
    : IRequest<IList<Device>>;