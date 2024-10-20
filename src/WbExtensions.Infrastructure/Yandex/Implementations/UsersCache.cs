using System.Collections.Concurrent;
using WbExtensions.Application.Interfaces.Yandex;
using WbExtensions.Application.Models.Yandex;

namespace WbExtensions.Infrastructure.Yandex.Implementations;

internal sealed class UsersCache : IUsersCache
{
    private readonly ConcurrentDictionary<string, YandexUserInfo?> _users = new();

    public bool TryGetValue(string key, out YandexUserInfo? userInfo)
    {
        return _users.TryGetValue(key, out userInfo);
    }

    public void AddOrUpdate(string key, YandexUserInfo userInfo)
    {
        _users.AddOrUpdate(
            key,
            _ => userInfo,
            (s, info) => userInfo);
    }

    public bool TryRemove(string key, out YandexUserInfo? userInfo)
    {
        return _users.TryRemove(key, out userInfo);
    }
}