using System.Collections.Generic;

namespace WbExtensions.Application.UseCases.GetDevicesForAlice;

public sealed record GetDevicesForAliceRequest(
    IReadOnlyCollection<string>? Ids = null);