﻿using System.Threading;
using System.Threading.Tasks;
using WbExtensions.Domain.Mqtt;

namespace WbExtensions.Application.MqttHandlers;

public interface IMqttHandler
{
    public Task HandleAsync(QueueMessage message, CancellationToken cancellationToken);
}