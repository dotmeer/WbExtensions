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
    private readonly ITelemetryRepository _telemetryRepository;
    private readonly IUserInfoRepository _userInfoRepository;

    public StorageController(
        ITelemetryRepository telemetryRepository,
        IUserInfoRepository userInfoRepository)
    {
        _telemetryRepository = telemetryRepository;
        _userInfoRepository = userInfoRepository;
    }

    [HttpGet(nameof(Telemetry))]
    public Task<IReadOnlyCollection<Telemetry>> GetTelemetryAsync(CancellationToken cancellationToken)
    {
        return _telemetryRepository.GetAsync(cancellationToken);
    }

    [HttpGet(nameof(UserInfo))]
    public Task<IReadOnlyCollection<UserInfo>> GetUserInfoAsync(CancellationToken cancellationToken)
    {
        return _userInfoRepository.GetAsync(cancellationToken);
    }
}