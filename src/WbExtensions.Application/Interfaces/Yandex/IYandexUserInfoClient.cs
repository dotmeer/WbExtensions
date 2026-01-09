using System.Threading;
using System.Threading.Tasks;
using WbExtensions.Application.Models.Yandex;

namespace WbExtensions.Application.Interfaces.Yandex;

public interface IYandexUserInfoClient
{
    Task<YandexUserInfo?> GetInfoAsync(string token, CancellationToken cancellationToken);
}