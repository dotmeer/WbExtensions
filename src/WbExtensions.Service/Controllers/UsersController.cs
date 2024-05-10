using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WbExtensions.Application.Interfaces.Yandex;
using WbExtensions.Domain.Alice.Responses;
using WbExtensions.Service.Authorization;

namespace WbExtensions.Service.Controllers;

[ApiController]
[Route("aliceapi/v1.0")]
[AllowExternalAccess(true)]
[YandexAuthorization]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("user/unlink")]
    public async Task<IActionResult> UnlinkUserAsync(
        [FromHeader(Name = "X-Request-Id")] string? requestId,
        [FromHeader(Name = AuthConstants.AuthHeaderName)] string? token,
        CancellationToken cancellationToken)
    {
        await _userService.RemoveAsync(token, cancellationToken);

        return Ok(
            new AliceResponse
            {
                RequestId = requestId!
            });
    }
}