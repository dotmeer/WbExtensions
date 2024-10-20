using System;
using System.Linq;
using WbExtensions.Application.Interfaces.Yandex;
using WbExtensions.Infrastructure.Yandex.Settings;

namespace WbExtensions.Infrastructure.Yandex.Implementations;

internal sealed class AllowedUsersService : IAllowedUsersService
{
    private readonly UserServiceSettings _settings;

    public AllowedUsersService(UserServiceSettings settings)
    {
        _settings = settings;
    }

    public bool IsUserAllowed(string email)
    {
        return _settings.AllowedUsers.Contains(email, StringComparer.OrdinalIgnoreCase);
    }
}