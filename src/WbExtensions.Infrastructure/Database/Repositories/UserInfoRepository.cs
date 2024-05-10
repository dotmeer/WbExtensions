using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using WbExtensions.Application.Interfaces.Database;
using WbExtensions.Domain;

namespace WbExtensions.Infrastructure.Database.Repositories;

internal sealed class UserInfoRepository : IUserInfoRepository
{
    private readonly BaseRepository _baseRepository;

    public UserInfoRepository(BaseRepository baseRepository)
    {
        _baseRepository = baseRepository;
    }

    public Task<IReadOnlyCollection<UserInfo>> GetAsync(CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(
            $@"select * from {nameof(UserInfo)}",
            cancellationToken: cancellationToken);

        return _baseRepository.QueryAsync<UserInfo>(command);
    }

    public Task UpsertAsync(UserInfo user, CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(@$"
insert into {nameof(UserInfo)} ({nameof(UserInfo.Id)}, {nameof(UserInfo.Email)}, {nameof(UserInfo.Updated)})
values(@{nameof(UserInfo.Id)}, @{nameof(UserInfo.Email)}, @{nameof(UserInfo.Updated)})
on conflict ({nameof(UserInfo.Id)}) do nothing;",
            new
            {
                user.Id,
                user.Email,
                user.Updated
            },
            cancellationToken: cancellationToken);

        return _baseRepository.ExecuteAsync(command);
    }
}