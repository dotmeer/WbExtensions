using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WbExtensions.Application.UseCases.SendTelegramMessage;
using WbExtensions.Service.Authorization;

namespace WbExtensions.Service.Controllers;

[ApiController]
[Route("telegram")]
[AllowExternalAccess(false)]
public sealed class TelegramController : Controller
{
    [HttpPost("send")]
    public Task SendMessageAsync(
        [FromBody] SendTelegramMessageRequest request,
        CancellationToken cancellationToken,
        [FromServices] IMediator mediator)
    {
        return mediator.Send(request, cancellationToken);
    }
}