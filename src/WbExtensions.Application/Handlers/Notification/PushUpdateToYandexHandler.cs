using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using WbExtensions.Application.Helpers.Alice.Converters;
using WbExtensions.Application.Interfaces.Database;
using WbExtensions.Application.Interfaces.Yandex;
using WbExtensions.Application.Models.Notification;
using WbExtensions.Domain.Alice.Push;
using WbExtensions.Domain.Alice;
using WbExtensions.Domain.Home;

namespace WbExtensions.Application.Handlers.Notification;

internal sealed class PushUpdateToYandexHandler : INotificationHandler<PushUpdateToYandexNotification>
{
    private readonly IUserInfoRepository _userInfoRepository;
    private readonly IPushService _pushService;
    private readonly ILogger<PushUpdateToYandexHandler> _logger;

    public PushUpdateToYandexHandler(
        IUserInfoRepository userInfoRepository,
        IPushService pushService,
        ILogger<PushUpdateToYandexHandler> logger)
    {
        _userInfoRepository = userInfoRepository;
        _pushService = pushService;
        _logger = logger;
    }

    public async Task Handle(PushUpdateToYandexNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            if (!notification.Control.Reportable)
            {
                return;
            }

            var controlList = new List<Control>
            {
                notification.Control
            };

            var devices = new List<Device>
            {
                new Device
                {
                    Id = notification.VirtualDevice.Id,
                    Properties = controlList.ToProperties().ToList(),
                    Capabilities = controlList.ToCapabilities().ToList()
                }
            };

            foreach (var userInfo in await _userInfoRepository.GetAsync(cancellationToken))
            {
                var pushRequest = new PushRequest(
                    new PushRequestPayload
                    {
                        UserId = userInfo.Id,
                        Devices = devices
                    });

                await _pushService.PushAsync(pushRequest, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error while pushing to yandex for device '{Device}' and control '{Control}'",
                notification.VirtualDevice.VirtualDeviceName,
                notification.Control.VirtualControlName);
        }
    }
}