using MediatR;

namespace WbExtensions.Application.UseCases.SendTelegramMessage;

public sealed record SendTelegramMessageRequest(
    string Message)
    : IRequest;