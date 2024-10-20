using System.Collections.Generic;
using WbExtensions.Domain.Alice.Requests;
using WbExtensions.Domain.Home;

namespace WbExtensions.Application.UseCases.ExecuteAliceCommands;

public sealed record ExecuteAliceCommandsRequest(
    IReadOnlyCollection<SetUSerDevicesStateRequestItem> Actions);