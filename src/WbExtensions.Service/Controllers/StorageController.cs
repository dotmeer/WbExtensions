using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WbExtensions.Application.Interfaces.Database;
using WbExtensions.Domain;

namespace WbExtensions.Service.Controllers;

[ApiController]
[Route("storage")]
[AllowAnonymous]
public sealed class StorageController : ControllerBase
{
    private readonly ITelemetryRepository _telemetryRepository;

    public StorageController(ITelemetryRepository telemetryRepository)
    {
        _telemetryRepository = telemetryRepository;
    }

    [HttpGet]
    public Task<IReadOnlyCollection<Telemetry>> GetAsync(CancellationToken cancellationToken)
    {
        return _telemetryRepository.GetAsync(cancellationToken);
    }
}