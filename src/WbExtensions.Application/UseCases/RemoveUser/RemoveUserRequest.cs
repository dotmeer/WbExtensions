﻿using MediatR;

namespace WbExtensions.Application.UseCases.RemoveUser;

public sealed record RemoveUserRequest(string? Token) : IRequest;