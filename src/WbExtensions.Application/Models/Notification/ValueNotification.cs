using MediatR;

namespace WbExtensions.Application.Models.Notification;

internal sealed record ValueNotification(
    string VirtualDeviceName,
    string VirtualControlName,
    string? Value) : INotification;