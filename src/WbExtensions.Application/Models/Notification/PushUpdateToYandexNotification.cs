using MediatR;
using WbExtensions.Domain.Home;

namespace WbExtensions.Application.Models.Notification;

internal sealed record PushUpdateToYandexNotification(
    VirtualDevice VirtualDevice,
    Control Control)
    : INotification;