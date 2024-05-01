using System;
using System.Collections.Generic;
using WbExtensions.Domain.Home.Enums;

namespace WbExtensions.Domain.Home;

public sealed class VirtualDevice
{
    public string Id { get; init; } = default!;

    public VirtualDeviceType Type { get; init; }

    public string Name { get; init; } = default!;

    public string? Description { get; init; }

    // TODO: переделать на строку, зачем enum?
    public Room? Room { get; init; }

    public IReadOnlyCollection<Control> Controls { get; init; } = Array.Empty<Control>();
}