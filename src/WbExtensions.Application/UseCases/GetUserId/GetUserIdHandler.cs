using System;
using System.Threading;
using System.Threading.Tasks;
using WbExtensions.Application.Interfaces.Database;
using WbExtensions.Application.Interfaces.Yandex;
using WbExtensions.Application.Models.Yandex;
using WbExtensions.Domain;

namespace WbExtensions.Application.UseCases.GetUserId;

public sealed class GetUserIdHandler
{
    private readonly IUserInfoRepository _userInfoRepository;
    private readonly IUsersCache _usersCache;
    private readonly IAllowedUsersService _allowedUsersService;
    private readonly IYandexUserInfoClient _yandexUserInfoClient;

    public GetUserIdHandler(
        IUserInfoRepository userInfoRepository,
        IUsersCache usersCache,
        IAllowedUsersService allowedUsersService,
        IYandexUserInfoClient yandexUserInfoClient)
    {
        _userInfoRepository = userInfoRepository;
        _usersCache = usersCache;
        _allowedUsersService = allowedUsersService;
        _yandexUserInfoClient = yandexUserInfoClient;
    }

    public async Task<string?> HandleAsync(GetUserIdRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Token))
        {
            return null;
        }

        YandexUserInfo? userInfo;
        if (_usersCache.TryGetValue(request.Token, out userInfo))
        {
            return userInfo!.Id;
        }

        userInfo = await _yandexUserInfoClient.GetInfoAsync(request.Token, cancellationToken);

        if (userInfo is null)
        {
            return null;
        }

        if (!_allowedUsersService.IsUserAllowed(userInfo.DefaultEmail))
        {
            return null;
        }

        _usersCache.AddOrUpdate(request.Token, userInfo);

        await _userInfoRepository.UpsertAsync(
            new UserInfo(userInfo.Id, userInfo.DefaultEmail, DateTime.UtcNow),
            cancellationToken);

        return userInfo.Id;
    }
}