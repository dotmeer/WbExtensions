using MediatR;

namespace WbExtensions.Application.UseCases.GetUserId;

public sealed record GetUserIdRequest(string? Token) : IRequest<string?>;