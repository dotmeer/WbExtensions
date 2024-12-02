using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WbExtensions.Application.Interfaces.Telegram;

namespace WbExtensions.Application.UseCases.SendTelegramMessage;

internal sealed class SendTelegramMessageHandler : IRequestHandler<SendTelegramMessageRequest>
{
    private readonly ITelegramService _telegramService;

    public SendTelegramMessageHandler(ITelegramService telegramService)
    {
        _telegramService = telegramService;
    }

    public Task Handle(SendTelegramMessageRequest request, CancellationToken cancellationToken)
    {
        return _telegramService.SendMessageAsync(request.Message, cancellationToken);
    }
}