using System.Threading.Tasks;
using System.Threading;

namespace WbExtensions.Infrastructure.Yandex.Abstractions;

public interface IUserService
{
    /// <summary>
    /// Возвращает идентификатор пользователя в яндексе
    /// </summary>
    /// <param name="token">Токен авторизации</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    Task<string?> GetUserIdAsync(string? token, CancellationToken cancellationToken);
}