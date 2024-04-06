using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WbExtensions.Service.Controllers;

[ApiController]
[Route("aliceapi/v1.0")]
[Authorize]
public sealed class HealthCheckController : ControllerBase
{
    [HttpHead]
    public IActionResult HealthCheck()
    {
        return Ok();
    }
}