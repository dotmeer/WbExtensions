using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WbExtensions.Application.Interfaces.Database;
using WbExtensions.Application.Interfaces.Yandex;

namespace WbExtensions.Application.UseCases.RemoveUser;

internal sealed class RemoveUserHandler : IRequestHandler<RemoveUserRequest>
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

    public async Task Handle(RemoveUserRequest request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(request.Token)
            && _usersCache.TryRemove(request.Token.Replace("Bearer ", ""), out var userInfo))
        {
            await _userInfoRepository.RemoveAsync(userInfo!.Id, cancellationToken);
        }
    }
}