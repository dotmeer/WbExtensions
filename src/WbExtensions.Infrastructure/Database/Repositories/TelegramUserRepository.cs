using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using WbExtensions.Application.Interfaces.Database;
using WbExtensions.Domain.Telegram;

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

    public Task<TelegramUser?> FindAsync(long userId, CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(@$"
select *
from {nameof(TelegramUser)}
where {nameof(TelegramUser.UserId)} = @{nameof(userId)}",
            new
            {
                userId
            },
            cancellationToken: cancellationToken);

        return _baseRepository.FindAsync<TelegramUser>(command);
    }

    public Task AddAsync(TelegramUser telegramUser, CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(@$"
insert into {nameof(TelegramUser)} ({nameof(TelegramUser.UserId)}, {nameof(TelegramUser.UserName)}, {nameof(TelegramUser.Role)})
values(@{nameof(TelegramUser.UserId)}, @{nameof(TelegramUser.UserName)}, @{nameof(TelegramUser.Role)})",
            new
            {
                telegramUser.UserId,
                telegramUser.UserName,
                telegramUser.Role
            },
            cancellationToken: cancellationToken);

        return _baseRepository.ExecuteAsync(command);
    }

    public Task AllowUserAsync(long userId, CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(@$"
update {nameof(TelegramUser)}
set {nameof(TelegramUser.Role)} = @role
where {nameof(TelegramUser.UserId)} = @{nameof(userId)}",
            new
            {
                role = Role.Keeper,
                userId
            },
            cancellationToken: cancellationToken);

        return _baseRepository.ExecuteAsync(command);
    }

    public Task DeleteAsync(long userId, CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(@$"
delete from {nameof(TelegramUser)}
where {nameof(TelegramUser.UserId)} = @{nameof(userId)}",
            new
            {
                userId
            },
            cancellationToken: cancellationToken);

        return _baseRepository.ExecuteAsync(command);
    }
}