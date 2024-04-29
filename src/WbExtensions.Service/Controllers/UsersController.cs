using Microsoft.AspNetCore.Mvc;
using WbExtensions.Domain.Alice.Responses;
using WbExtensions.Service.Authorization;

namespace WbExtensions.Service.Controllers;

[ApiController]
[Route("aliceapi/v1.0")]
[AllowExternalAccess(true)]
[YandexAuthorization]
public sealed class UsersController : ControllerBase
{
    [HttpPost("user/unlink")]
    public IActionResult UnlinkUser(
        [FromHeader(Name = "X-Request-Id")] string? requestId)
    {
        return Ok(
            new AliceResponse
            {
                RequestId = requestId!
            });
    }
}