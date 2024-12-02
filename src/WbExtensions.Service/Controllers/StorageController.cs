using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WbExtensions.Application.Interfaces.Database;
using WbExtensions.Domain;
using WbExtensions.Service.Authorization;

namespace WbExtensions.Service.Controllers;

[ApiController]
[Route("storage")]
[AllowExternalAccess(false)]
public sealed class StorageController : ControllerBase
{
    [HttpGet(nameof(Telemetry))]
    public Task<IReadOnlyCollection<Telemetry>> GetTelemetryAsync(
        CancellationToken cancellationToken,
        [FromServices] ITelemetryRepository telemetryRepository)
    {
        return telemetryRepository.GetAsync(cancellationToken);
    }

    [HttpGet(nameof(UserInfo))]
    public Task<IReadOnlyCollection<UserInfo>> GetUserInfoAsync(
        CancellationToken cancellationToken,
        [FromServices] IUserInfoRepository userInfoRepository)
    {
        return userInfoRepository.GetAsync(cancellationToken);
    }

    [HttpGet(nameof(TelegramUser))]
    public Task<IReadOnlyCollection<TelegramUser>> GetTelegramUserAsync(
        CancellationToken cancellationToken,
        [FromServices] ITelegramUserRepository telegramUserRepository)
    {
        return telegramUserRepository.GetAsync(cancellationToken);
    }
}