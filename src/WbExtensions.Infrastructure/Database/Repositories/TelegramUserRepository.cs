using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using WbExtensions.Application.Interfaces.Database;
using WbExtensions.Domain;

namespace WbExtensions.Infrastructure.Database.Repositories;

internal sealed class TelegramUserRepository : ITelegramUserRepository
{
    private readonly BaseRepository _baseRepository;

    public TelegramUserRepository(BaseRepository baseRepository)
    {
        _baseRepository = baseRepository;
    }

    public Task<IReadOnlyCollection<TelegramUser>> GetAsync(CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(
            $"select * from {nameof(TelegramUser)}",
            cancellationToken: cancellationToken);

        return _baseRepository.QueryAsync<TelegramUser>(command);
    }

    public Task AddAsync(TelegramUser telegramUser, CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(@$"
insert into {nameof(TelegramUser)} ({nameof(TelegramUser.UserId)}, {nameof(TelegramUser.UserName)}, {nameof(TelegramUser.IsAllowed)}, {nameof(TelegramUser.IsAdmin)})
values(@{nameof(TelegramUser.UserId)}, @{nameof(TelegramUser.UserName)}, @{nameof(TelegramUser.IsAllowed)}, @{nameof(TelegramUser.IsAdmin)})",
            new
            {
                telegramUser.UserId,
                telegramUser.UserName,
                telegramUser.IsAllowed,
                telegramUser.IsAdmin
            },
            cancellationToken: cancellationToken);

        return _baseRepository.ExecuteAsync(command);
    }

    public Task AllowUserAsync(long userId, CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(@$"
update {nameof(TelegramUser)}
set {nameof(TelegramUser.IsAllowed)} = 1
where {nameof(TelegramUser.UserId)} = @{nameof(userId)}",
            new
            {
                userId
            },
            cancellationToken: cancellationToken);

        return _baseRepository.ExecuteAsync(command);
    }

    public Task DisallowUserAsync(long userId, CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(@$"
update {nameof(TelegramUser)}
set {nameof(TelegramUser.IsAllowed)} = 0
where {nameof(TelegramUser.UserId)} = @{nameof(userId)}",
            new
            {
                userId
            },
            cancellationToken: cancellationToken);

        return _baseRepository.ExecuteAsync(command);
    }
}