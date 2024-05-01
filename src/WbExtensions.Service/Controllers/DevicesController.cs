using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WbExtensions.Application.Interfaces.Alice;
using WbExtensions.Domain.Alice.Requests;
using WbExtensions.Domain.Alice.Responses;
using WbExtensions.Service.Authorization;

namespace WbExtensions.Service.Controllers;

[ApiController]
[Route("aliceapi/v1.0")]
[AllowExternalAccess(true)]
[YandexAuthorization]
public sealed class DevicesController : ControllerBase
{
    private readonly IAliceDevicesService _aliceDevicesService;

    public DevicesController(IAliceDevicesService aliceDevicesService)
    {
        _aliceDevicesService = aliceDevicesService;
    }

    [HttpGet("user/devices")]
    public async Task<IActionResult> GetUserDevices(
        [FromHeader(Name = "X-Request-Id")] string? requestId,
        CancellationToken cancellationToken)
    {
        var response = new AliceResponseWithPayload
        {
            RequestId = requestId!,
            Payload = new Payload
            {
                UserId = User.FindFirst(AuthConstants.UserIdClaim)!.Value,
                Devices = await _aliceDevicesService.GetAsync(cancellationToken)
            }
        };

        return Ok(response);
    }

    [HttpPost("user/devices/query")]
    public async Task<IActionResult> GetUserDevicesState(
        [FromHeader(Name = "X-Request-Id")] string? requestId,
        [FromBody] GetUserDevicesStateRequest request,
        CancellationToken cancellationToken)
    {
        var response = new AliceResponseWithPayload
        {
            RequestId = requestId!,
            Payload = new Payload
            {
                UserId = User.FindFirst(AuthConstants.UserIdClaim)!.Value,
                Devices = await _aliceDevicesService.GetAsync(
                    request.Devices.Select(_ => _.Id).ToArray(),
                    cancellationToken)
            }
        };

        return Ok(response);
    }

    [HttpPost("user/devices/action")]
    public async Task<IActionResult> SetUSerDevicesState(
        [FromHeader(Name = "X-Request-Id")] string? requestId,
        [FromBody] SetUSerDevicesStateRequest request,
        CancellationToken cancellationToken)
    {
        var response = new AliceResponseWithPayload
        {
            RequestId = requestId!,
            Payload = new Payload
            {
                UserId = User.FindFirst(AuthConstants.UserIdClaim)!.Value,
                Devices = await _aliceDevicesService.UpdateDevicesStateAsync(
                    request.Payload.Devices.ToList(),
                    cancellationToken)
            }
        };

        return Ok(response);
    }
}