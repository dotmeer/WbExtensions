using Microsoft.AspNetCore.Mvc;
using WbExtensions.Service.Authorization;

namespace WbExtensions.Service.Controllers;

[ApiController]
[Route("aliceapi/v1.0")]
[AllowExternalAccess(true)]
[YandexAuthorization]
public sealed class HealthCheckController : ControllerBase
{
    [HttpHead]
    public IActionResult HealthCheck()
    {
        return Ok();
    }
}