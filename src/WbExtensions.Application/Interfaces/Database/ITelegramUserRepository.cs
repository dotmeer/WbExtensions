using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WbExtensions.Domain;

namespace WbExtensions.Application.Interfaces.Database;

public interface ITelegramUserRepository
{
    Task<IReadOnlyCollection<TelegramUser>> GetAsync(CancellationToken cancellationToken);

    Task AddAsync(TelegramUser telegramUser, CancellationToken cancellationToken);

    Task AllowUserAsync(long userId, CancellationToken cancellationToken);

    Task DisallowUserAsync(long userId, CancellationToken cancellationToken);
}