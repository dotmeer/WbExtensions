﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WbExtensions.Application.UseCases.ExecuteAliceCommands;
using WbExtensions.Application.UseCases.GetDevicesForAlice;
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
    [HttpGet("user/devices")]
    public async Task<IActionResult> GetUserDevices(
        [FromHeader(Name = "X-Request-Id")] string? requestId,
        CancellationToken cancellationToken,
        [FromServices] GetDevicesForAliceHandler handler)
    {
        var response = new AliceResponseWithPayload
        {
            RequestId = requestId!,
            Payload = new Payload
            {
                UserId = User.FindFirst(AuthConstants.UserIdClaim)!.Value,
                Devices = await handler.HandleAsync(
                    new GetDevicesForAliceRequest(),
                    cancellationToken)
            }
        };

        return Ok(response);
    }

    [HttpPost("user/devices/query")]
    public async Task<IActionResult> GetUserDevicesState(
        [FromHeader(Name = "X-Request-Id")] string? requestId,
        [FromBody] GetUserDevicesStateRequest request,
        CancellationToken cancellationToken,
        [FromServices] GetDevicesForAliceHandler handler)
    {
        var response = new AliceResponseWithPayload
        {
            RequestId = requestId!,
            Payload = new Payload
            {
                UserId = User.FindFirst(AuthConstants.UserIdClaim)!.Value,
                Devices = await handler.HandleAsync(
                    new GetDevicesForAliceRequest(request.Devices.Select(_ => _.Id).ToArray()),
                    cancellationToken)
            }
        };

        return Ok(response);
    }

    [HttpPost("user/devices/action")]
    public async Task<IActionResult> SetUSerDevicesState(
        [FromHeader(Name = "X-Request-Id")] string? requestId,
        [FromBody] SetUSerDevicesStateRequest request,
        CancellationToken cancellationToken,
        [FromServices] ExecuteAliceCommandsHandler handler)
    {
        var response = new AliceResponseWithPayload
        {
            RequestId = requestId!,
            Payload = new Payload
            {
                UserId = User.FindFirst(AuthConstants.UserIdClaim)!.Value,
                Devices = await handler.HandleAsync(
                    new ExecuteAliceCommandsRequest(request.Payload.Devices.ToList()),
                    cancellationToken)
            }
        };

        return Ok(response);
    }
}