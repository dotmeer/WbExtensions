using System.Threading;
using System.Threading.Tasks;
using WbExtensions.Application.Interfaces.Database;
using WbExtensions.Application.Interfaces.Yandex;

namespace WbExtensions.Application.UseCases.RemoveUser;

public sealed class RemoveUserHandler
{
    private readonly IUsersCache _usersCache;
    private readonly IUserInfoRepository _userInfoRepository;

    public RemoveUserHandler(
        IUsersCache usersCache,
        IUserInfoRepository userInfoRepository)
    {
        _usersCache = usersCache;
        _userInfoRepository = userInfoRepository;
    }

    public async Task HandleAsync(RemoveUserRequest request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(request.Token)
            && _usersCache.TryRemove(request.Token.Replace("Bearer ", ""), out var userInfo))
        {
            await _userInfoRepository.RemoveAsync(userInfo!.Id, cancellationToken);
        }
    }
}