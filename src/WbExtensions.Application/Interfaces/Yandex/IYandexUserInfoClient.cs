using System.Threading.Tasks;
using System.Threading;
using WbExtensions.Application.Models.Yandex;

namespace WbExtensions.Application.Interfaces.Yandex;

public interface IYandexUserInfoClient
{
    Task<YandexUserInfo?> GetInfoAsync(string token, CancellationToken cancellationToken);
}