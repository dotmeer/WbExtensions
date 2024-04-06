using System;
using System.Collections.Generic;

namespace WbExtensions.Infrastructure.Yandex.Settings;

internal sealed class UserServiceSettings
{
    public IReadOnlyCollection<string> AllowedUsers { get; init; } = Array.Empty<string>();
}