using System.Threading;
using System.Threading.Tasks;

namespace WbExtensions.Application.Interfaces.Yandex;

public interface IUserService
{
    /// <summary>
    /// Возвращает идентификатор пользователя в яндексе
    /// </summary>
    /// <param name="token">Токен авторизации</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    Task<string?> GetUserIdAsync(string? token, CancellationToken cancellationToken);

    Task RemoveAsync(string? token, CancellationToken cancellationToken);
}