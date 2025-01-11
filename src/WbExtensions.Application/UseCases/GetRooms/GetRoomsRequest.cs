using System.Collections.Generic;
using MediatR;

namespace WbExtensions.Application.UseCases.GetRooms;

public record GetRoomsRequest : IRequest<IReadOnlyCollection<string>>;