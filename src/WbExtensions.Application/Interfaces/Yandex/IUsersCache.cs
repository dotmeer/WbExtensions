using WbExtensions.Application.Models.Yandex;

namespace WbExtensions.Application.Interfaces.Yandex;

public interface IUsersCache
{
    bool TryGetValue(string key, out YandexUserInfo? userInfo);

    void AddOrUpdate(string key, YandexUserInfo  userInfo);

    bool TryRemove(string key, out YandexUserInfo? userInfo);
}