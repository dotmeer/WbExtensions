using System;
using System.Collections.Generic;
using WbExtensions.Domain.Home.Enums;

namespace WbExtensions.Domain.Home;

public sealed class VirtualDevice
{
    private readonly string? _virtualDeviceName;

    public string Id { get; init; } = default!;

    public VirtualDeviceType Type { get; init; }

    public string Name { get; init; } = default!;

    public string? Description { get; init; }

    public string? Room { get; init; }

    public string VirtualDeviceName
    {
        get => _virtualDeviceName ?? Id;

        init => _virtualDeviceName = value;
    }

    public IReadOnlyCollection<Control> Controls { get; init; } = Array.Empty<Control>();
}