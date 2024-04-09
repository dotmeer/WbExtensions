using System.Collections.Generic;
using System.Linq;

namespace WbExtensions.Infrastructure.Database.Settings;

internal sealed record DatabaseSettings
{
    private readonly HashSet<string> _storabelDevices = new(0);
    private readonly HashSet<string> _storabelControls = new(0);

    public IReadOnlyCollection<string> StorableDevices
    {
        get => _storabelDevices;
        init => _storabelDevices = value.Distinct().ToHashSet();
    }

    public IReadOnlyCollection<string> StorableControls
    {
        get => _storabelControls;
        init => _storabelControls = value.Distinct().ToHashSet();
    }
}