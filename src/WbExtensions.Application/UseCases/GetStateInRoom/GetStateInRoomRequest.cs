using MediatR;

namespace WbExtensions.Application.UseCases.GetStateInRoom;

public sealed record GetStateInRoomRequest(
    string Room)
    : IRequest<string>;